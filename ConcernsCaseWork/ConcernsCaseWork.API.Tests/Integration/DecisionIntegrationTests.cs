﻿using AutoFixture;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection(ApiTestCollection.ApiTestCollectionName)]
	public class DecisionIntegrationTests
	{
		private readonly HttpClient _client;
		private readonly Fixture _autoFixture;
		private readonly ApiTestFixture _testFixture;

		private readonly DateTimeOffset _decisionMadeDate = new DateTimeOffset(2022, 1, 1, 0, 0, 0, new TimeSpan());
		private readonly DateTimeOffset _decisionEffectiveDate = new DateTimeOffset(2022, 5, 5, 0, 0, 0, new TimeSpan());

		public DecisionIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_autoFixture = new();
			_testFixture = apiTestFixture;
		}

		[Fact]
		public async Task When_Get_HasNoOutcome_Returns_200()
		{
			var request = _autoFixture.Create<CreateDecisionRequest>();
			request.TotalAmountRequested = 100;

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var createdDecision = await CreateDecision(concernsCaseId, request);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var result = wrapper.Data;

			AssertDecision(result, request);

			result.ConcernsCaseUrn.Should().Be(concernsCaseId);
			result.Outcome.Should().BeNull();
		}

		[Fact]
		public async Task When_Get_HasOutcome_Returns_200()
		{
			var decisionRequest = _autoFixture.Create<CreateDecisionRequest>();
			decisionRequest.TotalAmountRequested = 100;

			var outcomeRequest = _autoFixture.Create<CreateDecisionOutcomeRequest>();

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var createdDecision = await CreateDecision(concernsCaseId, decisionRequest);

			_ = await CreateDecisionOutcome(concernsCaseId, createdDecision.DecisionId, outcomeRequest);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var result = wrapper.Data;

			AssertDecision(result, decisionRequest);

			result.Should().NotBeNull();
			result.Outcome.Should().NotBeNull();
			result.Outcome!.Status.Should().Be(outcomeRequest.Status);
			result.Outcome.Authorizer.Should().Be(outcomeRequest.Authorizer);
			result.Outcome.TotalAmount.Should().Be(100);
			result.Outcome.DecisionMadeDate.Should().Be(_decisionMadeDate);
			result.Outcome.DecisionEffectiveFromDate.Should().Be(_decisionEffectiveDate);

			result.Outcome.BusinessAreasConsulted.Should().BeEquivalentTo(outcomeRequest.BusinessAreasConsulted);
		}

		[Fact]
		public async Task When_Delete_HasNoResource_Returns_404()
		{
			var concernsCaseId = 987654321;
			var decisionId = 123456789;

			HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionId}");
			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}


		[Fact]
		public async Task When_Delete_HasResource_Returns_204()
		{
			//BuilderSetup.DisablePropertyNamingFor<ConcernsRecord, DateTime?>(f => f.DeletedAt);
			var decisionRequest = _autoFixture.Create<CreateDecisionRequest>();
			decisionRequest.TotalAmountRequested = 100;

			var outcomeRequest = _autoFixture.Create<CreateDecisionOutcomeRequest>();

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var createdDecision = await CreateDecision(concernsCaseId, decisionRequest);

			_ = await CreateDecisionOutcome(concernsCaseId, createdDecision.DecisionId, outcomeRequest);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);


			HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");

			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			var getResponseNotFound = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponseNotFound.StatusCode.Should().Be(HttpStatusCode.NotFound);

		}


		private void AssertDecision(GetDecisionResponse actual, CreateDecisionRequest expected)
		{
			actual.Should().BeEquivalentTo(expected, (options) =>
			{
				options.Excluding(r => r.ConcernsCaseUrn);

				return options;
			});
		}

		private async Task<ConcernsCase> CreateConcernsCase()
		{
			var toAdd = new ConcernsCase()
			{
				RatingId = 1,
				StatusId = 1
			};

			await using var context = _testFixture.GetContext();

			var result = context.ConcernsCase.Add(toAdd);
			await context.SaveChangesAsync();

			return result.Entity;
		}

		private async Task<CreateDecisionOutcomeResponse> CreateDecisionOutcome(int concernsCaseId, int decisionId, CreateDecisionOutcomeRequest request)
		{
			request.TotalAmount = 100;
			request.DecisionMadeDate = _decisionMadeDate;
			request.DecisionEffectiveFromDate = _decisionEffectiveDate;
		
			var postResult = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionId}/outcome", request.ConvertToJson());
			postResult.StatusCode.Should().Be(HttpStatusCode.Created);
		
			var response = await postResult.Content.ReadFromJsonAsync<ApiSingleResponseV2<CreateDecisionOutcomeResponse>>();
		
			return response.Data;
		}

		private async Task<CreateDecisionResponse> CreateDecision(int concernsCaseId, CreateDecisionRequest request)
		{
			var postResult = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/", request.ConvertToJson());
			postResult.StatusCode.Should().Be(HttpStatusCode.Created);

			var response = await postResult.Content.ReadFromJsonAsync<ApiSingleResponseV2<CreateDecisionResponse>>();

			return response.Data;
		}
	}
}
