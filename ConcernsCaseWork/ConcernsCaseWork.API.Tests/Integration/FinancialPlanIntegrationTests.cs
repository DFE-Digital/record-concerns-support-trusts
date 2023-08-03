﻿using AutoFixture;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection(ApiTestCollection.ApiTestCollectionName)]
	public class FinancialPlanIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly RandomGenerator _randomGenerator;

		public FinancialPlanIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_randomGenerator = new RandomGenerator();
		}

		[Fact]
		public async Task When_Post_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<CreateFinancialPlanRequest>();
			request.Name = new string('a', 301);
			request.CreatedBy = new string('a', 301);
			request.Notes = new string('a', 2001);

			var result = await _client.PostAsync($"/v2/case-actions/financial-plan", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field Name must be a string with a maximum length of 300.");
			error.Should().Contain("The field CreatedBy must be a string with a maximum length of 300.");
			error.Should().Contain("The field Notes must be a string with a maximum length of 2000.");
		}

		[Fact]
		public async Task When_Patch_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<PatchFinancialPlanRequest>();
			request.Name = new string('a', 301);
			request.CreatedBy = new string('a', 301);
			request.Notes = new string('a', 2001);

			var result = await _client.PatchAsync($"/v2/case-actions/financial-plan", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field Name must be a string with a maximum length of 300.");
			error.Should().Contain("The field CreatedBy must be a string with a maximum length of 300.");
			error.Should().Contain("The field Notes must be a string with a maximum length of 2000.");
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var request = _fixture.Create<CreateFinancialPlanRequest>();
			request.StatusId = 1;

			//Act
			var createdFinancialPlan = await CreateFinancialPlan(request);

			//Assert
			createdFinancialPlan.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
			createdFinancialPlan.Status.Id.Should().Be(request.StatusId.Value);
		}

		[Fact]
		public async Task When_Patch_Returns_200Response()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdFinancialPlan = await CreateFinancialPlan();

			var request = _fixture.Build<PatchFinancialPlanModel>()
							.With(x => x.Id, createdFinancialPlan.Id)
							.With(x => x.CaseUrn, createdConcern.Urn)
							.With(x => x.StatusId, 2)
							.Create();

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/financial-plan", request.ConvertToJson());

			//Assert
			var updatedFinancialPlan = await GetFinancialPlanByID(request.Id);
			updatedFinancialPlan.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
		}


		private async Task<ConcernsCaseResponse> CreateCase()
		{
			ConcernCaseRequest createRequest = Builder<ConcernCaseRequest>.CreateNew()
				.With(c => c.CreatedBy = _randomGenerator.NextString(3, 10))
				.With(c => c.Description = "")
				.With(c => c.CrmEnquiry = "")
				.With(c => c.TrustUkprn = DatabaseModelBuilder.CreateUkPrn())
				.With(c => c.ReasonAtReview = "")
				.With(c => c.DeEscalation = new DateTime(2022, 04, 01))
				.With(c => c.Issue = "Here is the issue")
				.With(c => c.CurrentStatus = "Case status")
				.With(c => c.CaseAim = "Here is the aim")
				.With(c => c.DeEscalationPoint = "Point of de-escalation")
				.With(c => c.CaseHistory = "Case history")
				.With(c => c.NextSteps = "Here are the next steps")
				.With(c => c.DirectionOfTravel = "Up")
				.With(c => c.StatusId = 1)
				.With(c => c.RatingId = 2)
				.With(c => c.TrustCompaniesHouseNumber = DatabaseModelBuilder.CreateUkPrn())
				.Build();


			HttpRequestMessage httpRequestMessage = new()
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://notarealdomain.com/v2/concerns-cases"),
				Content = JsonContent.Create(createRequest)
			};

			var response = await _client.SendAsync(httpRequestMessage);

			response.StatusCode.Should().Be(HttpStatusCode.Created);
			ApiSingleResponseV2<ConcernsCaseResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();
			return result.Data;
		}

		private async Task<ConcernsCaseResponse> GetCase(Int32 urn)
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{urn}");
			var x = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();


			return x.Data;
		}

		private async Task<FinancialPlanResponse> CreateFinancialPlan()
		{
			var request = _fixture.Create<CreateFinancialPlanRequest>();
			request.StatusId = 1;
			return await CreateFinancialPlan(request);
		}

		private async Task<FinancialPlanResponse> CreateFinancialPlan(CreateFinancialPlanRequest request)
		{
			var result = await _client.PostAsync($"/v2/case-actions/financial-plan", request.ConvertToJson());
			var createdFinancialPlan = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<FinancialPlanResponse>>();
			var createdFinancialPlanUrl = result.Headers.Location;

			var getResponse = await _client.GetAsync(createdFinancialPlanUrl);
			var getResponseFinancialPlan = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<FinancialPlanResponse>>();

			result.StatusCode.Should().Be(HttpStatusCode.Created);
			createdFinancialPlan.Should().NotBeNull();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseFinancialPlan.Should().NotBeNull();

			return getResponseFinancialPlan.Data;
		}

		protected async Task<FinancialPlanResponse> GetFinancialPlanByID(long id)
		{
			return await GetFinancialPlan($"/v2/case-actions/financial-plan/{id}");
		}

		protected async Task<FinancialPlanResponse> GetFinancialPlan(string url)
		{
			var getResponse = await _client.GetAsync(url);
			var getResponseFinancialPlan = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<FinancialPlanResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseFinancialPlan.Should().NotBeNull();

			return getResponseFinancialPlan.Data;
		}
	}
}
