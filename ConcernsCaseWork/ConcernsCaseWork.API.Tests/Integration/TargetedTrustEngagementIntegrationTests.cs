using AutoFixture;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
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
	public class TargetedTrustEngagementIntegrationTests
	{
		private readonly HttpClient _client;
		private readonly Fixture _autoFixture;
		private readonly ApiTestFixture _testFixture;

		public TargetedTrustEngagementIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_autoFixture = new();
			_testFixture = apiTestFixture;
		}

		[Fact]
		public async Task When_Get_TTEByCaseUrn_Returns_200()
		{
			//Arrange
			var createdConcern = await CreateConcernsCase();
			var tteOne = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			tteOne.Notes = "TTE 1";
			tteOne.CaseUrn = createdConcern.Id;

			var tteTwo = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			tteTwo.Notes = "TTE 2";
			tteTwo.CaseUrn = createdConcern.Id;

			var createdTTEOne = await CreateTargetedTrustEngagement(createdConcern.Id, tteOne);
			var createdTTETwo = await CreateTargetedTrustEngagement(createdConcern.Id, tteTwo);

			var TTEOne = await GeTargetTrustEngagement(createdTTEOne);
			var TTETwo = await GeTargetTrustEngagement(createdTTETwo);

			List<GetTargetedTrustEngagementResponse> expected = new List<GetTargetedTrustEngagementResponse>() { TTEOne, TTETwo };

			//Act
			var result = await _client.GetAsync($"/v2/concerns-cases/{createdConcern.Id}/targetedtrustengagement");

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			ApiResponseV2<TargetedTrustEngagementSummaryResponse> content = await result.Content.ReadFromJsonAsync<ApiResponseV2<TargetedTrustEngagementSummaryResponse>>();
			content.Data.Count().Should().Be(expected.Count);

			foreach (var expectedItem in expected)
			{
				var item = content.Data.FirstOrDefault(f => f.TargetedTrustEngagementId == expectedItem.Id);
				item.ClosedAt.Should().Be(expectedItem.ClosedAt);
				item.CaseUrn.Should().Be(expectedItem.CaseUrn);
				item.CreatedAt.Should().Be(expectedItem.CreatedAt);
				item.Title.Should().Be(expectedItem.Title);
			}
		}

		[Fact]
		public async Task When_Get_CaseDoesNotExist_404()
		{
			var caseId = 1000000;
			var tteId = 100000;

			//Act
			var result = await _client.GetAsync($"/v2/concerns-cases/{caseId}/targetedtrustengagement/{tteId}");

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Concerns case with id {caseId}");
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var request = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			request.Notes = "TTE 1";
			request.CaseUrn = concernsCase.Id;

			request.ActivityId = Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivity.NoEngagementActivitiesWereTakenForward;
			request.ActivityTypes = [Contracts.TargetedTrustEngagement.TargetedTrustEngagementActivityType.Category1];

			var createResponse = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement", request.ConvertToJson());
			createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

			var createContent = await createResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<CreateTargetedTrustEngagementResponse>>();

			createContent.Data.TargetedTrustEngagementId.Should().BeGreaterThan(0);
			createContent.Data.ConcernsCaseUrn.Should().Be(concernsCase.Urn);

			var targetedTrustEngagementId = createContent.Data.TargetedTrustEngagementId;

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement/{targetedTrustEngagementId}");

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetTargetedTrustEngagementResponse>>();
			var createdDecision = wrapper.Data;

			createdDecision.Should().BeEquivalentTo(request);

			createdDecision.CaseUrn.Should().Be(request.CaseUrn);
			createdDecision.ActivityId.Should().Be(request.ActivityId);
			createdDecision.ActivityTypes.Should().BeEquivalentTo(request.ActivityTypes);

			await using ConcernsDbContext refreshedContext = _testFixture.GetContext();
			concernsCase = refreshedContext.ConcernsCase.FirstOrDefault(c => c.Id == concernsCaseId);
			concernsCase.CaseLastUpdatedAt.Value.Date.Should().Be(createdDecision.CreatedAt.Date);
		}

		[Fact]
		public async Task When_Post_CaseDoesNotExist_Returns_404()
		{
			var caseId = 1000000;

			var request = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			request.CaseUrn = caseId;

			var result = await _client.PostAsync($"/v2/concerns-cases/{caseId}/targetedtrustengagement", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Concerns case {caseId}");
		}

		[Fact]
		public async Task When_Post_InvalidRequest_Returns_400()
		{
			var concernsCase = await CreateConcernsCase();
			var caseId = concernsCase.Id;

			var createRequest = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			createRequest.Notes = new string('a', 2001);

			var result = await _client.PostAsync($"/v2/concerns-cases/{caseId}/targetedtrustengagement", createRequest.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var errorContent = await result.Content.ReadAsStringAsync();
			errorContent.Should().Contain("Notes must be 2000 characters or less");
		}

		[Fact]
		public async Task When_Put_Returns_200Response()
		{
			// arrange
			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var createRequest = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			createRequest.CaseUrn = concernsCaseId;
			createRequest.Notes = "Notes";

			var createResponse = await CreateTargetedTrustEngagement(concernsCaseId, createRequest);
			var targetedTrustEngagementId = createResponse.TargetedTrustEngagementId;

			var updateRequest = _autoFixture.Create<UpdateTargetedTrustEngagementRequest>();
			updateRequest.Notes = "Updated Notes";

			// act
			var updateResponse = await _client
				.PutAsync($"/v2/concerns-cases/{concernsCase.Urn}/targetedtrustengagement/{targetedTrustEngagementId}",
				updateRequest.ConvertToJson());

			// assert
			updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			var updateContent = await updateResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetTargetedTrustEngagementResponse>>();
			updateContent.Data.Id.Should().Be(targetedTrustEngagementId);
			updateContent.Data.CaseUrn.Should().Be(concernsCase.Urn);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement/{targetedTrustEngagementId}");

			var getContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetTargetedTrustEngagementResponse>>();
			var targetedTrustEngagement = getContent.Data;

			await using ConcernsDbContext refreshedContext = _testFixture.GetContext();
			concernsCase = refreshedContext.ConcernsCase.FirstOrDefault(c => c.Id == concernsCaseId);
			concernsCase.CaseLastUpdatedAt.Value.Should().Be(targetedTrustEngagement.UpdatedAt);
		}

		[Fact]
		public async Task When_Put_CaseDoesNotExist_Returns_404()
		{
			var caseId = 1000000;

			var request = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			request.Notes = "Notes";
			request.CaseUrn = caseId;

			var result = await _client.PutAsync($"/v2/concerns-cases/{caseId}/targetedtrustengagement/1", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Concerns case {caseId}");
		}

		[Fact]
		public async Task When_Put_DecisionDoesNotExist_Returns_404()
		{
			var concernsCase = await CreateConcernsCase();
			var caseId = concernsCase.Id;

			var tteId = 1000000;

			var request = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			request.Notes = "Notes";
			request.CaseUrn = caseId;

			var result = await _client.PutAsync($"/v2/concerns-cases/{caseId}/targetedtrustengagement/{tteId}", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: Targeted Trust Engagement {tteId}");
		}

		[Fact]
		public async Task When_Put_InvalidRequest_Returns_400()
		{
			var concernsCase = await CreateConcernsCase();
			var caseId = concernsCase.Id;

			var createRequest = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			createRequest.Notes = "Notes";
			createRequest.CaseUrn = caseId;

			var createdTTE = await CreateTargetedTrustEngagement(caseId, createRequest);

			var updateRequest = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			updateRequest.Notes = new string('a', 2001);

			var result = await _client.PutAsync($"/v2/concerns-cases/{caseId}/targetedtrustengagement/{createdTTE.TargetedTrustEngagementId}", updateRequest.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var errorContent = await result.Content.ReadAsStringAsync();
			errorContent.Should().Contain("Notes must be 2000 characters or less");
		}

		[Fact]
		public async Task When_Delete_HasNoResource_Returns_404()
		{
			var concernsCaseId = 987654321;
			var tteId = 123456789;

			HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement/{tteId}");
			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task When_Delete_HasResource_Returns_204()
		{

			var concernsCase = await CreateConcernsCase();
			var concernsCaseId = concernsCase.Id;

			var tteRequest = _autoFixture.Create<CreateTargetedTrustEngagementRequest>();
			tteRequest.Notes = "Notes";
			tteRequest.CaseUrn = concernsCaseId;

			var createdTTE = await CreateTargetedTrustEngagement(concernsCaseId, tteRequest);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement/{createdTTE.TargetedTrustEngagementId}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement/{createdTTE.TargetedTrustEngagementId}");

			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			var getResponseNotFound = await _client.GetAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement/{createdTTE.TargetedTrustEngagementId}");
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

		private async Task<CreateTargetedTrustEngagementResponse> CreateTargetedTrustEngagement(int concernsCaseId, CreateTargetedTrustEngagementRequest request)
		{
			var postResult = await _client.PostAsync($"/v2/concerns-cases/{concernsCaseId}/targetedtrustengagement/", request.ConvertToJson());
			postResult.StatusCode.Should().Be(HttpStatusCode.Created);

			var response = await postResult.Content.ReadFromJsonAsync<ApiSingleResponseV2<CreateTargetedTrustEngagementResponse>>();

			return response.Data;
		}

		private async Task<GetTargetedTrustEngagementResponse> GeTargetTrustEngagement(CreateTargetedTrustEngagementResponse createTargetedTrustEngagementResponse)
		{
			return await GeTargetTrustEngagement(createTargetedTrustEngagementResponse.ConcernsCaseUrn, createTargetedTrustEngagementResponse.TargetedTrustEngagementId);
		}

		private async Task<GetTargetedTrustEngagementResponse> GeTargetTrustEngagement(int caseID, int tteID)
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/{caseID}/targetedtrustengagement/{tteID}");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<GetTargetedTrustEngagementResponse>>();
			var result = wrapper.Data;

			return result;
		}
	}
}
