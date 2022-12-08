using AutoFixture;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection("Database")]
	public class DecisionIntegrationTests : IClassFixture<ApiTestFixture>
	{
		private HttpClient _client;
		private Fixture _fixture;
		private ConcernsDbContext _context;

		private DateTimeOffset _decisionMadeDate = new DateTimeOffset(2022, 1, 1, 0, 0, 0, new TimeSpan());
		private DateTimeOffset _decisionEffectiveDate = new DateTimeOffset(2022, 5, 5, 0, 0, 0, new TimeSpan());

		public DecisionIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_context = apiTestFixture.DbContext;

			_fixture = new();
		}

		[Fact]
		public async Task When_Get_HasNoOutcome_Returns_200()
		{
			var request = _fixture.Create<CreateDecisionRequest>();
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
			var decisionRequest = _fixture.Create<CreateDecisionRequest>();
			decisionRequest.TotalAmountRequested = 100;

			var outcomeRequest = _fixture.Create<CreateDecisionOutcomeRequest>();

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var createdDecision = await CreateDecision(concernsCaseId, decisionRequest);

			await CreateDecisionOutcome(concernsCaseId, createdDecision.DecisionId, outcomeRequest);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var result = wrapper.Data;

			AssertDecision(result, decisionRequest);

			result.Outcome.Status.Should().Be(outcomeRequest.Status);
			result.Outcome.Authorizer.Should().Be(outcomeRequest.Authorizer);
			result.Outcome.TotalAmount.Should().Be(100);
			result.Outcome.DecisionMadeDate.Should().Be(_decisionMadeDate);
			result.Outcome.DecisionEffectiveFromDate.Should().Be(_decisionEffectiveDate);

			result.Outcome.BusinessAreasConsulted.Should().BeEquivalentTo(outcomeRequest.BusinessAreasConsulted);
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

			var result = _context.ConcernsCase.Add(toAdd);
			await _context.SaveChangesAsync();

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
