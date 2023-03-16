using AutoFixture;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
	public class DecisionOutcomeIntegrationTests
	{
		private readonly HttpClient _client;
		private readonly Fixture _autoFixture;
		private readonly ApiTestFixture _testFixture;

		public DecisionOutcomeIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_autoFixture = new();
			_testFixture = apiTestFixture;
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			var request = _autoFixture.Create<CreateDecisionOutcomeRequest>();
			request.TotalAmount = 100;

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionToAdd = await CreateDecision(concernsCase.Id);

			var result = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionToAdd.DecisionId}/outcome", request.ConvertToJson());

			using var context = _testFixture.GetContext();

			var decision = context.Decisions
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

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionToAdd = await CreateDecision(concernsCase.Id);

			var result = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionToAdd.DecisionId}/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.Created);

			using var context = _testFixture.GetContext();

			var decision = context.Decisions
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
		public async Task When_PostWithExistingOutcome_returns_409Response()
		{
			var request = _autoFixture.Create<CreateDecisionOutcomeRequest>();
			request.TotalAmount = 100;

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionToAdd = await CreateDecision(concernsCase.Id);
			var decisionId = decisionToAdd.DecisionId;

			await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionId}/outcome", request.ConvertToJson());

			var result = await _client.PostAsync(
				$"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionId}/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.Conflict);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Conflict: Decision with id {decisionId} already has an outcome, Case {concernsCaseId}");
		}

		[Fact]
		public async Task When_PostWithMissingCase_Returns_404Response()
		{
			var request = new CreateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var result = await _client.PostAsync($"/v2/concerns-cases/-1/decisions/1/outcome", request.ConvertToJson());

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

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var result = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/-1/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Decision with id -1, Case {concernsCaseId}");
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

			var result = await _client.PostAsync($"/v2/concerns-cases/1/decisions/1/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();

			error.Should().Contain("Select a decision outcome status");
			error.Should().Contain("The field Authorizer is invalid");
			error.Should().Contain("The total amount requested must be zero or greater");
		}

		[Fact]
		public async Task When_Put_Returns_200Response()
		{
			var createOutcomeResponse = await SetupPutTest();
			var caseId = createOutcomeResponse.ConcernsCaseUrn;
			var decisionId = createOutcomeResponse.DecisionId;

			var request = new UpdateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Declined,
				Authorizer = DecisionOutcomeAuthorizer.DeputyDirector,
				DecisionEffectiveFromDate = new DateTimeOffset(2022, 1, 1, 0, 0, 0, new TimeSpan()),
				DecisionMadeDate = new DateTimeOffset(2022, 5, 5, 0, 0, 0, new TimeSpan()),
				TotalAmount = 500,
				BusinessAreasConsulted = new System.Collections.Generic.List<DecisionOutcomeBusinessArea>()
				{
					DecisionOutcomeBusinessArea.Capital,
					DecisionOutcomeBusinessArea.FinancialProviderMarketOversight
				}
			};

			var putResult = await _client.PutAsync($"/v2/concerns-cases/{caseId}/decisions/{decisionId}/outcome", request.ConvertToJson());
			putResult.StatusCode.Should().Be(HttpStatusCode.OK);

			var updateResult = await putResult.Content.ReadResponseFromWrapper<UpdateDecisionOutcomeResponse>();
			updateResult.ConcernsCaseUrn.Should().Be(caseId);
			updateResult.DecisionId.Should().Be(decisionId);
			updateResult.DecisionOutcomeId.Should().Be(createOutcomeResponse.DecisionOutcomeId);

			var updatedDecision = await GetDecision(caseId, decisionId);

			updatedDecision.Outcome.Status.Should().Be(request.Status);
			updatedDecision.Outcome.Authorizer.Should().Be(request.Authorizer);
			updatedDecision.Outcome.DecisionEffectiveFromDate.Should().Be(request.DecisionEffectiveFromDate);
			updatedDecision.Outcome.DecisionMadeDate.Should().Be(request.DecisionMadeDate);
			updatedDecision.Outcome.TotalAmount.Should().Be(request.TotalAmount);
			updatedDecision.Outcome.BusinessAreasConsulted.Should().BeEquivalentTo(request.BusinessAreasConsulted);
		}

		[Fact]
		public async Task When_Put_RemovesAllNonMandatoryFields_Returns_200Response()
		{
			var createOutcomeResponse = await SetupPutTest();
			var caseId = createOutcomeResponse.ConcernsCaseUrn;
			var decisionId = createOutcomeResponse.DecisionId;

			var request = new UpdateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Withdrawn
			};

			var putResult = await _client.PutAsync($"/v2/concerns-cases/{caseId}/decisions/{decisionId}/outcome", request.ConvertToJson());
			putResult.StatusCode.Should().Be(HttpStatusCode.OK);

			var updatedDecision = await GetDecision(caseId, decisionId);

			updatedDecision.Outcome.Status.Should().Be(request.Status);
			updatedDecision.Outcome.Authorizer.Should().BeNull();
			updatedDecision.Outcome.DecisionEffectiveFromDate.Should().BeNull();
			updatedDecision.Outcome.DecisionMadeDate.Should().BeNull();
			updatedDecision.Outcome.TotalAmount.Should().BeNull();
			updatedDecision.Outcome.BusinessAreasConsulted.Should().BeEmpty();
		}

		[Fact]
		public async Task When_PutWithMissingCase_Returns_404Response()
		{
			var request = new UpdateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var result = await _client.PutAsync($"/v2/concerns-cases/-1/decisions/1/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain("Not Found: Concern with id -1");
		}

		[Fact]
		public async Task When_PutWithMissingDecision_Returns_404Response()
		{
			var request = new UpdateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var result = await _client.PutAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/-1/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Decision with id -1, Case {concernsCaseId}");
		}

		[Fact]
		public async Task When_PutWithMissingOutcome_Returns_404Response()
		{
			var request = new UpdateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved
			};

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionToAdd = await CreateDecision(concernsCase.Id);

			var result = await _client.PutAsync(
				$"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionToAdd.DecisionId}/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Decision with id {decisionToAdd.DecisionId} does not have an outcome, Case {concernsCaseId}");
		}

		[Fact]
		public async Task When_PutWithInvalidRequest_Returns_400Response()
		{
			var request = new UpdateDecisionOutcomeRequest()
			{
				TotalAmount = -200,
				Authorizer = 0,
				Status = 0
			};

			var result = await _client.PutAsync($"/v2/concerns-cases/1/decisions/1/outcome", request.ConvertToJson());

			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();

			error.Should().Contain("Select a decision outcome status");
			error.Should().Contain("The field Authorizer is invalid");
			error.Should().Contain("The total amount requested must be zero or greater");
		}

		private async Task<ConcernsCase> CreateConcernsCase()
		{
			var toAdd = new ConcernsCase()
			{
				RatingId = 1,
				StatusId = 1
			};

			using var context = _testFixture.GetContext();

			var result = context.ConcernsCase.Add(toAdd);
			await context.SaveChangesAsync();

			return result.Entity;
		}

		private async Task<Decision> CreateDecision(int concernsCaseId)
		{
			var decisionToAdd = Decision.CreateNew("123456", false, false, "", new DateTimeOffset(), new DecisionType[] { }, 200, "Notes!", new DateTimeOffset());

			using var context = _testFixture.GetContext();

			decisionToAdd.ConcernsCaseId = concernsCaseId;
			context.Decisions.Add(decisionToAdd);

			await context.SaveChangesAsync();

			return decisionToAdd;
		}

		private async Task<GetDecisionResponse> GetDecision(int caseUrn, int decisionId)
		{
			var response = await _client.GetAsync($"/v2/concerns-cases/{caseUrn}/decisions/{decisionId}");

			var result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();

			return result.Data;
		}

		private async Task<CreateDecisionOutcomeResponse> SetupPutTest()
		{
			var request = new CreateDecisionOutcomeRequest()
			{
				Status = DecisionOutcomeStatus.Approved,
				Authorizer = DecisionOutcomeAuthorizer.G7,
				DecisionEffectiveFromDate = new DateTimeOffset(2022, 2, 2, 0, 0, 0, new TimeSpan()),
				DecisionMadeDate = new DateTimeOffset(2022, 8, 7, 0, 0, 0, new TimeSpan()),
				TotalAmount = 500,
				BusinessAreasConsulted = new System.Collections.Generic.List<DecisionOutcomeBusinessArea>()
				{
					DecisionOutcomeBusinessArea.RegionsGroup,
					DecisionOutcomeBusinessArea.SchoolsFinancialSupportAndOversight
				}
			};

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionToAdd = await CreateDecision(concernsCase.Id);

			var response = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionToAdd.DecisionId}/outcome", request.ConvertToJson());

			var result = await response.Content.ReadResponseFromWrapper<CreateDecisionOutcomeResponse>();

			return result;
		}
	}
}
