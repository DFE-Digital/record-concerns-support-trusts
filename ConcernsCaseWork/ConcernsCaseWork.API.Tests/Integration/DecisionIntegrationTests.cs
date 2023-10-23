using AutoFixture;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
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
		public async Task When_Get_HasOutcome_Returns_200()
		{
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionRequest = _autoFixture.Create<CreateDecisionRequest>();
			decisionRequest.TotalAmountRequested = 100;
			decisionRequest.ConcernsCaseUrn = concernsCaseId;

			var outcomeRequest = _autoFixture.Create<CreateDecisionOutcomeRequest>();

			var createdDecision = await CreateDecision(concernsCaseId, decisionRequest);

			await CreateDecisionOutcome(concernsCaseId, createdDecision.DecisionId, outcomeRequest);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var result = wrapper.Data;

			result.Should().BeEquivalentTo(decisionRequest);

			result.Should().NotBeNull();
			result.Outcome.Should().NotBeNull();
			result.Outcome.Status.Should().Be(outcomeRequest.Status);
			result.Outcome.Authorizer.Should().Be(outcomeRequest.Authorizer);
			result.Outcome.TotalAmount.Should().Be(100);
			result.Outcome.DecisionMadeDate.Should().Be(_decisionMadeDate);
			result.Outcome.DecisionEffectiveFromDate.Should().Be(_decisionEffectiveDate);

			result.Outcome.BusinessAreasConsulted.Should().BeEquivalentTo(outcomeRequest.BusinessAreasConsulted);
		}

		[Fact]
		public async Task When_Get_DecisionsByCaseUrn_Returns_200()
		{
			//Arrange
			var createdConcern = await CreateConcernsCase();
			var decisionRequestOne = _autoFixture.Create<CreateDecisionRequest>();
			decisionRequestOne.TotalAmountRequested = 100;
			decisionRequestOne.ConcernsCaseUrn = createdConcern.Id;

			var decisionRequestTwo = _autoFixture.Create<CreateDecisionRequest>();
			decisionRequestTwo.TotalAmountRequested = 200;
			decisionRequestTwo.ConcernsCaseUrn = createdConcern.Id;

			var createdDecisionOne = await CreateDecision(createdConcern.Id, decisionRequestOne);
			var createdDecisionTwo = await CreateDecision(createdConcern.Id, decisionRequestTwo);

			var DecisionOne = await GetDecision(createdDecisionOne);
			var DecisionTwo = await GetDecision(createdDecisionTwo);

			List<GetDecisionResponse> expected = new List<GetDecisionResponse>() { DecisionOne, DecisionTwo };

			//Act
			var result = await _client.GetAsync($"/v2/concerns-cases/{createdConcern.Id}/decisions");

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			ApiResponseV2<DecisionSummaryResponse> content = await result.Content.ReadFromJsonAsync<ApiResponseV2<DecisionSummaryResponse>>();
			content.Data.Count().Should().Be(expected.Count);

			foreach (var expectedItem in expected)
			{
				var item = content.Data.FirstOrDefault(f => f.DecisionId == expectedItem.DecisionId);
				item.ClosedAt.Should().Be(expectedItem.ClosedAt);
				item.ConcernsCaseUrn.Should().Be(expectedItem.ConcernsCaseUrn);
				item.CreatedAt.Should().Be(expectedItem.CreatedAt);
				item.Outcome.Should().BeNull();
				item.Status.Should().Be(expectedItem.DecisionStatus);
				item.Title.Should().Be(expectedItem.Title);
				item.UpdatedAt.Should().Be(expectedItem.UpdatedAt);
			}
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var request = _autoFixture.Create<CreateDecisionRequest>();
			request.TotalAmountRequested = 100;
			request.ConcernsCaseUrn = concernsCaseId;

			request.DecisionTypes.ToList().First().DecisionFrameworkCategoryId = FrameworkCategory.EnablingFinancialRecovery;

			var createResponse = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions", request.ConvertToJson());
			createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

			var createContent = await createResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<CreateDecisionResponse>>();

			createContent.Data.DecisionId.Should().BeGreaterThan(0);
			createContent.Data.ConcernsCaseUrn.Should().Be(concernsCase.Urn);

			var decisionId = createContent.Data.DecisionId;

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionId}");

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var createdDecision = wrapper.Data;

			createdDecision.Should().BeEquivalentTo(request);
			createdDecision.Outcome.Should().BeNull();

			createdDecision.ConcernsCaseUrn.Should().Be(request.ConcernsCaseUrn);
			createdDecision.DecisionTypes.Should().BeEquivalentTo(request.DecisionTypes);

			await using ConcernsDbContext refreshedContext = _testFixture.GetContext();
			concernsCase = refreshedContext.ConcernsCase.FirstOrDefault(c => c.Id == concernsCaseId);
			concernsCase.CaseLastUpdatedAt.Value.Date.Should().Be(createdDecision.CreatedAt.DateTime.Date);
		}

		[Fact]
		public async Task When_Post_CaseDoesNotExist_Returns_404()
		{
			var caseId = 1000000;

			var request = _autoFixture.Create<CreateDecisionRequest>();
			request.TotalAmountRequested = 100;
			request.ConcernsCaseUrn = caseId;

			var result = await _client.PostAsync($"/v2/concerns-cases/{caseId}/decisions", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Concerns case {caseId}");
		}

		[Fact]
		public async Task When_Post_InvalidRequest_Returns_400()
		{
			var concernsCase = await CreateConcernsCase();
			var caseId = concernsCase.Id;

			var createRequest = _autoFixture.Create<CreateDecisionRequest>();
			createRequest.TotalAmountRequested = -1;
			createRequest.SupportingNotes = new string('a', 2001);
			createRequest.SubmissionDocumentLink = new string('a', 2049);

			var result = await _client.PostAsync($"/v2/concerns-cases/{caseId}/decisions", createRequest.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var errorContent = await result.Content.ReadAsStringAsync();
			errorContent.Should().Contain("The total amount requested must be zero or greater");
			errorContent.Should().Contain("Notes must be 2000 characters or less");
			errorContent.Should().Contain("Submission document link must be 2048 or less");
		}

		[Fact]
		public async Task When_Put_Returns_200Response()
		{
			// arrange
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var createRequest = _autoFixture.Create<CreateDecisionRequest>();
			createRequest.ConcernsCaseUrn = concernsCaseId;
			createRequest.TotalAmountRequested = 200;
			createRequest.HasCrmCase = true;
			createRequest.RetrospectiveApproval = true;
			createRequest.SubmissionRequired = true;

			var createResponse = await CreateDecision(concernsCaseId, createRequest);
			var decisionId = createResponse.DecisionId;

			var updateRequest = _autoFixture.Create<UpdateDecisionRequest>();
			updateRequest.TotalAmountRequested = 100;
			updateRequest.HasCrmCase = false;
			updateRequest.RetrospectiveApproval = false;
			updateRequest.SubmissionRequired = false;

			updateRequest.DecisionTypes = new List<DecisionTypeQuestion>() { new DecisionTypeQuestion()
				{
					Id = DecisionType.EsfaApproval,
					DecisionDrawdownFacilityAgreedId = DrawdownFacilityAgreed.PaymentUnderExistingArrangement,
					DecisionFrameworkCategoryId = FrameworkCategory.BuildingFinancialCapability
				}
			}.ToArray();
			
			// act
			var updateResponse = await _client
				.PutAsync($"/v2/concerns-cases/{concernsCase.Urn}/decisions/{decisionId}", 
				updateRequest.ConvertToJson());

			// assert
			updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			var updateContent = await updateResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<UpdateDecisionResponse>>();
			updateContent.Data.DecisionId.Should().Be(decisionId);
			updateContent.Data.ConcernsCaseUrn.Should().Be(concernsCase.Urn);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionId}");

			var getContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var decision = getContent.Data;

			decision.Should().BeEquivalentTo(updateRequest);

			await using ConcernsDbContext refreshedContext = _testFixture.GetContext();
			concernsCase = refreshedContext.ConcernsCase.FirstOrDefault(c => c.Id == concernsCaseId);
			concernsCase.CaseLastUpdatedAt.Value.Should().Be(decision.UpdatedAt.DateTime);
		}

		[Fact]
		public async Task When_Put_CaseDoesNotExist_Returns_404()
		{
			var caseId = 1000000;

			var request = _autoFixture.Create<CreateDecisionRequest>();
			request.TotalAmountRequested = 100;
			request.ConcernsCaseUrn = caseId;

			var result = await _client.PutAsync($"/v2/concerns-cases/{caseId}/decisions/1", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Concerns case {caseId}");
		}

		[Fact]
		public async Task When_Put_DecisionDoesNotExist_Returns_404()
		{
			var concernsCase = await CreateConcernsCase();
			var caseId = concernsCase.Id;

			var decisionId = 1000000;

			var request = _autoFixture.Create<CreateDecisionRequest>();
			request.TotalAmountRequested = 100;
			request.ConcernsCaseUrn = caseId;

			var result = await _client.PutAsync($"/v2/concerns-cases/{caseId}/decisions/{decisionId}", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Decision {decisionId}");
		}

		[Fact]
		public async Task When_Put_InvalidRequest_Returns_400()
		{
			var concernsCase = await CreateConcernsCase();
			var caseId = concernsCase.Id;

			var createRequest = _autoFixture.Create<CreateDecisionRequest>();
			createRequest.TotalAmountRequested = 100;
			createRequest.ConcernsCaseUrn = caseId;

			var createdDecision = await CreateDecision(caseId, createRequest);

			var updateRequest = _autoFixture.Create<CreateDecisionRequest>();
			updateRequest.TotalAmountRequested = -1;
			updateRequest.SupportingNotes = new string('a', 2001);
			updateRequest.SubmissionDocumentLink = new string('a', 2049);

			var result = await _client.PutAsync($"/v2/concerns-cases/{caseId}/decisions/{createdDecision.DecisionId}", updateRequest.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var errorContent = await result.Content.ReadAsStringAsync();
			errorContent.Should().Contain("The total amount requested must be zero or greater");
			errorContent.Should().Contain("Notes must be 2000 characters or less");
			errorContent.Should().Contain("Submission document link must be 2048 or less");
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
			var outcomeRequest = _autoFixture.Create<CreateDecisionOutcomeRequest>();

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionRequest = _autoFixture.Create<CreateDecisionRequest>();
			decisionRequest.TotalAmountRequested = 100;
			decisionRequest.ConcernsCaseUrn = concernsCaseId;

			var createdDecision = await CreateDecision(concernsCaseId, decisionRequest);

			_ = await CreateDecisionOutcome(concernsCaseId, createdDecision.DecisionId, outcomeRequest);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");

			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			var getResponseNotFound = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{createdDecision.DecisionId}");
			getResponseNotFound.StatusCode.Should().Be(HttpStatusCode.NotFound);

		}

		private async Task<ConcernsCase> CreateConcernsCase()
		{
			var toAdd = new ConcernsCase()
			{
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
				ReviewAt = DateTime.Now.AddDays(7),
				ClosedAt = null,
				CreatedBy = "John Doe",
				Description = "Sample description",
				CrmEnquiry = "Sample CRM enquiry",
				TrustUkprn = "12345",
				ReasonAtReview = "Sample reason at review",
				DeEscalation = DateTime.Now.AddDays(2),
				Issue = "Sample issue",
				CurrentStatus = "Open",
				CaseAim = "Sample case aim",
				DeEscalationPoint = "Sample de-escalation point",
				NextSteps = "Sample next steps",
				DirectionOfTravel = "Sample direction of travel",
				CaseHistory = "Sample case history",
				StatusId = 2,
				RatingId = 3
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

		private async Task<GetDecisionResponse> GetDecision(CreateDecisionResponse createDecisionResponse)
		{
			return await GetDecision(createDecisionResponse.ConcernsCaseUrn, createDecisionResponse.DecisionId);
		}

		private async Task<GetDecisionResponse> GetDecision(int caseID, int decisionID)
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{caseID}/decisions/{decisionID}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var result = wrapper.Data;

			return result;
		}
	}
}
