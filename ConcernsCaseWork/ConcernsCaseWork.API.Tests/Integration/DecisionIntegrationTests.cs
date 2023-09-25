using AutoFixture;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Migrations;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
		public async Task When_Get_HasNoOutcome_Returns_200()
		{
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var request = _autoFixture.Create<CreateDecisionRequest>();
			request.TotalAmountRequested = 100;
			request.ConcernsCaseUrn = concernsCaseId;

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
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var decisionRequest = _autoFixture.Create<CreateDecisionRequest>();
			decisionRequest.TotalAmountRequested = 100;
			decisionRequest.ConcernsCaseUrn = concernsCaseId;

			var outcomeRequest = _autoFixture.Create<CreateDecisionOutcomeRequest>();

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

		protected async Task<GetDecisionResponse> GetDecision(CreateDecisionResponse createDecisionResponse)
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


		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var request = _autoFixture.Create<CreateDecisionRequest>();
			request.TotalAmountRequested = 100;
			request.ConcernsCaseUrn = concernsCaseId;

			var expectedDecisionFrameworkCategory = (Contracts.Enums.DecisionFrameworkCategory)1;

			request.DecisionTypes.ToList().First().DecisionFrameworkCategoryId = expectedDecisionFrameworkCategory;

			
			var decisionToAdd = await CreateDecision(concernsCase.Id, request);

			var result = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/decisions", request.ConvertToJson());
			await using var context = _testFixture.GetContext();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionToAdd.DecisionId}");

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var createdDecision = wrapper.Data;

			result.StatusCode.Should().Be(HttpStatusCode.Created);
			createdDecision.ConcernsCaseUrn.Should().Be(request.ConcernsCaseUrn);
			createdDecision.DecisionTypes.Should().BeEquivalentTo(request.DecisionTypes, (options) =>
			{
				options.Excluding(r => r.Id);

				return options;
			});

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
		public async Task When_Put_Returns_200Response()
		{
			// arrange
			var request = _autoFixture.Create<UpdateDecisionRequest>();
			request.TotalAmountRequested = 100;

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var originalDecisionTypes = new List<DecisionType>(){ new DecisionType(Data.Enums.Concerns.DecisionType.EsfaApproval, API.Contracts.Decisions.DrawdownFacilityAgreed.No, API.Contracts.Decisions.FrameworkCategory.FacilitatingTransferFinanciallyAgreed) };
			var decisionId = await CreateDecision(concernsCaseId, originalDecisionTypes);

			request.DecisionTypes = null;
			request.DecisionTypes = new List<Contracts.Decisions.DecisionTypeQuestion>() { new Contracts.Decisions.DecisionTypeQuestion()
				{
					Id = Contracts.Enums.DecisionType.EsfaApproval,
					DecisionDrawdownFacilityAgreedId = Contracts.Enums.DecisionDrawdownFacilityAgreed.PaymentUnderExistingArrangement,
					DecisionFrameworkCategoryId = Contracts.Enums.DecisionFrameworkCategory.BuildingFinancialCapacity
				}
			}.ToArray();
			
			// act
			var result = await _client
				.PutAsync($"/v2/concerns-cases/{concernsCase.Urn}/decisions/{decisionId}", 
				request.ConvertToJson());

			// assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			var response = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<UpdateDecisionResponse>>();
			response.Data.DecisionId.Should().Be(decisionId);
			response.Data.ConcernsCaseUrn.Should().Be(concernsCase.Urn);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/decisions/{decisionId}");

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetDecisionResponse>>();
			var decision = wrapper.Data;

			decision.DecisionTypes.Should().BeEquivalentTo(request.DecisionTypes, (options) =>
			{
				options.Excluding(r => r.Id);

				return options;
			});

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

		private void AssertDecision(GetDecisionResponse actual, CreateDecisionRequest expected)
		{
			actual.Should().BeEquivalentTo(expected, (options) =>
			{
				options.Excluding(r => r.ConcernsCaseUrn);
				options.Excluding(r => r.DecisionTypes);

				return options;
			});
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
				Urn = 123456,
				StatusId = 2,
				RatingId = 3
			};

			await using var context = _testFixture.GetContext();

			var result = context.ConcernsCase.Add(toAdd);
			await context.SaveChangesAsync();

			return result.Entity;
		}

		private async Task<int> CreateDecision(int cCaseId, List<DecisionType> decisionTypes)
		{
			var decision = new Decision()
			{
				ConcernsCaseId = cCaseId,
				DecisionTypes = decisionTypes,
				TotalAmountRequested = 1,
				SupportingNotes = "Sample supporting notes",
				ReceivedRequestDate = DateTimeOffset.Now,
				SubmissionDocumentLink = "https://example.com/document.pdf",
				SubmissionRequired = true,
				RetrospectiveApproval = false,
				CrmCaseNumber = "CRM123456",
				CreatedAt = DateTimeOffset.Now.AddDays(-7),
				UpdatedAt = DateTimeOffset.Now,
				Status = Data.Enums.Concerns.DecisionStatus.InProgress,
				ClosedAt = null, 
				Outcome  = new Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome()
				{
					DecisionOutcomeId = 1,
					DecisionId = 2,
					Status = DecisionOutcomeStatus.Approved,
					TotalAmount = 1000.50m,
					DecisionMadeDate = DateTimeOffset.Now,
					DecisionEffectiveFromDate = DateTimeOffset.Now.AddDays(7),
					CreatedAt = DateTimeOffset.Now.AddDays(-7),
					UpdatedAt = DateTimeOffset.Now
				}
			};
			decision.ConcernsCaseId = cCaseId;
			decision.Outcome = null;

			await using (var ctxt = _testFixture.GetContext())
			{
				ctxt.Decisions.Add(decision);
				await ctxt.SaveChangesAsync();
			}

			return decision.DecisionId;
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
