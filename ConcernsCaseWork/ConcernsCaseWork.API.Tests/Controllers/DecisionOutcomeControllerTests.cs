﻿using AutoFixture;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
	public class DecisionOutcomeControllerTests : IDisposable
	{
		private WebApplicationFactory<Startup> _application;
		private HttpClient _client;
		private Fixture _fixture;
		private ConcernsDbContext _context;

		public DecisionOutcomeControllerTests()
		{
			_fixture = new();
			_application = new WebApplicationFactory<Startup>()
				.WithWebHostBuilder(builder =>
				{
					var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json");

					builder.ConfigureAppConfiguration((context, config) =>
					{
						config.AddJsonFile(configPath);
					});
				});

			_client = _application.CreateClient();
			_client.DefaultRequestHeaders.Add("ConcernsApiKey", "app-key");

			var contextOptions = new DbContextOptionsBuilder<ConcernsDbContext>()
				.UseSqlServer("Server=localhost;Database=integrationtests;Integrated Security=true")
				.Options;

			_context = new ConcernsDbContext(contextOptions);
			_context.Database.Migrate();
		}

		public void Dispose()
		{
			_application.Dispose();
			_client.Dispose();
			_context.Dispose();
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			var request = _fixture.Create<CreateDecisionOutcomeRequest>();
			request.TotalAmount = 100;

			var body = JsonConvert.SerializeObject(request);

			var decisionToAdd = Decision.CreateNew("123456", false, false, "", new DateTimeOffset(), new DecisionType[] { }, 200, "Notes!", new DateTimeOffset());

			var toAdd = new ConcernsCase()
			{
				RatingId = 1,
				StatusId = 1
			};

			var concernsCaseToAdd = _context.ConcernsCase.Add(toAdd);
			await _context.SaveChangesAsync();

			var concernsCaseId = concernsCaseToAdd.Entity.Id;

			decisionToAdd.ConcernsCaseId = concernsCaseId;
			toAdd.Decisions.Add(decisionToAdd);

			await _context.SaveChangesAsync();

			var result = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionToAdd.DecisionId}/outcome", CreateJsonPayload(body));

			var decision = _context.Decisions
				.Include(d => d.Outcome)
				.Include(d => d.Outcome.BusinessAreasConsulted)
				.First(d => d.DecisionId == decisionToAdd.DecisionId);

			result.StatusCode.Should().Be(HttpStatusCode.Created);

			var response = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<CreateDecisionOutcomeResponse>>();
			response.Data.DecisionId.Should().Be(decision.DecisionId);
			response.Data.ConcernsCaseUrn.Should().Be(concernsCaseId);
			response.Data.DecisionOutcomeId.Should().Be(decision.Outcome.DecisionOutcomeId);

			decision.Outcome.DecisionId.Should().Be(decisionToAdd.DecisionId);
			decision.Outcome.Status.Should().Be(request.Status);
			decision.Outcome.TotalAmount.Should().Be(request.TotalAmount);
			decision.Outcome.DecisionMadeDate.Should().NotBeNull();
			decision.Outcome.DecisionEffectiveFromDate.Should().NotBeNull();
			decision.Outcome.Authorizer.Should().Be(request.Authorizer);

			var areasConsulted = decision.Outcome.BusinessAreasConsulted.Select(b => b.DecisionOutcomeBusinessId).ToList();
			areasConsulted.Should().BeEquivalentTo(request.BusinessAreasConsulted);
		}

		[Fact]
		public async Task When_PostWithMinFieldsSet_Returns_201Response()
		{
			var request = new CreateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var decisionToAdd = Decision.CreateNew("123456", false, false, "", new DateTimeOffset(), new DecisionType[] { }, 200, "Notes!", new DateTimeOffset());

			var toAdd = new ConcernsCase()
			{
				RatingId = 1,
				StatusId = 1
			};

			var concernsCaseToAdd = _context.ConcernsCase.Add(toAdd);
			await _context.SaveChangesAsync();

			var concernsCaseId = concernsCaseToAdd.Entity.Id;

			decisionToAdd.ConcernsCaseId = concernsCaseId;
			toAdd.Decisions.Add(decisionToAdd);

			await _context.SaveChangesAsync();

			var body = JsonConvert.SerializeObject(request);

			var result = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionToAdd.DecisionId}/outcome", CreateJsonPayload(body));

			result.StatusCode.Should().Be(HttpStatusCode.Created);

			var decision = _context.Decisions
				.Include(d => d.Outcome)
				.Include(d => d.Outcome.BusinessAreasConsulted)
				.First(d => d.DecisionId == decisionToAdd.DecisionId);

			decision.Outcome.DecisionId.Should().Be(decisionToAdd.DecisionId);
			decision.Outcome.Status.Should().Be(request.Status);
			decision.Outcome.TotalAmount.Should().BeNull();
			decision.Outcome.DecisionMadeDate.Should().BeNull();
			decision.Outcome.DecisionEffectiveFromDate.Should().BeNull();
			decision.Outcome.Authorizer.Should().BeNull();
			decision.Outcome.BusinessAreasConsulted.Should().BeEmpty();
		}

		[Fact]
		public async Task When_PostWithMissingUrn_Returns_404Response()
		{
			var request = new CreateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var body = JsonConvert.SerializeObject(request);

			var result = await _client.PostAsync($"/v2/concerns-cases/-1/decisions/1/outcome", CreateJsonPayload(body));

			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain("Not Found: Concern with id -1");
		}

		[Fact]
		public async Task When_PostWithMissingDecision_Returns_404Response()
		{
			var request = new CreateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var toAdd = new ConcernsCase()
			{
				RatingId = 1,
				StatusId = 1
			};

			var concernsCaseToAdd = _context.ConcernsCase.Add(toAdd);
			await _context.SaveChangesAsync();
			var concernsCaseId = concernsCaseToAdd.Entity.Id;

			var body = JsonConvert.SerializeObject(request);

			var result = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/-1/outcome", CreateJsonPayload(body));

			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain("Not Found: Decision with id -1");
		}

		[Fact]
		public async Task When_PostWithInvalidRequest_Returns_400Response()
		{
			var request = new CreateDecisionOutcomeRequest()
			{
				TotalAmount = -200,
				Authorizer = 0,
				Status = 0
			};

			var body = JsonConvert.SerializeObject(request);

			var result = await _client.PostAsync($"/v2/concerns-cases/1/decisions/1/outcome", CreateJsonPayload(body));

			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();

			error.Should().Contain("The field Status is invalid");
			error.Should().Contain("The field Authorizer is invalid");
			error.Should().Contain("The total amount requested must be zero or greater");
		}

		[Fact]
		public async Task When_PutDecisionOutcome_Returns_200Response()
		{
			var request = new CreateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var body = JsonConvert.SerializeObject(request);

			var result = await _client.PutAsync($"/v2/concerns-cases/1/decisions/1/outcome", CreateJsonPayload(body));

			result.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		private StringContent CreateJsonPayload(string body)
		{
			return new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
		}
	}
}
