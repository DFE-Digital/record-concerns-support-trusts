using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection(ApiTestCollection.ApiTestCollectionName)]
	public class NtiUnderConsiderationIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly RandomGenerator _randomGenerator;

		public NtiUnderConsiderationIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_randomGenerator = new RandomGenerator();
		}

		[Fact]
		public async Task When_Post_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<CreateNTIUnderConsiderationRequest>();
			request.CreatedBy = new string('a', 301);
			request.Notes = new string('a', 2001);

			var result = await _client.PostAsync($"/v2/case-actions/nti-under-consideration", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field CreatedBy must be a string with a maximum length of 300.");
			error.Should().Contain("The field Notes must be a string with a maximum length of 2000.");
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var request = CreateNoticeToImproveRequestForCase(createdConcern.Urn);

			//Act
			var createdNTI = await CreateAndGetNTI(request);

			//Assert
			createdNTI.Should().BeEquivalentTo(request);
			await AssertCaseLastUpdatedDateMatchesNTICreatedAt(createdConcern, createdNTI);
		}

		[Fact]
		public async Task When_Patch_UpdateNti_ReturnOK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdNTI = await CreateNTIforCase(createdConcern.Urn);
			var request = CreateNTIUpdateRequest(createdConcern, createdNTI);

			//Act
			var result = await _client.PatchAsync(Patch.Update(), request.ConvertToJson());
			var response = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<NoticeToImproveResponse>>();
			var updatedNTI = await GetNTI(Get.ItemById(request.Id));

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().NotBeNull();
			updatedNTI.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
			await AssertCaseLastUpdatedDateMatchesNTIUpdatedAt(createdConcern, updatedNTI);
		}

		[Fact]
		public async Task When_Patch_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<PatchNTIUnderConsiderationRequest>();
			request.CreatedBy = new string('a', 301);
			request.Notes = new string('a', 2001);

			var result = await _client.PatchAsync($"/v2/case-actions/nti-under-consideration", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field CreatedBy must be a string with a maximum length of 300.");
			error.Should().Contain("The field Notes must be a string with a maximum length of 2000.");
		}

		[Fact]
		public async Task When_Delete_Return_204Response()
		{
			//Arrange
			var createdCase = await CreateCase();
			var createdNTI = await CreateNTIforCase(createdCase.Urn);

			//Act
			var result = await _client.DeleteAsync($"/v2/case-actions/nti-under-consideration/{createdNTI.Id}");
			var getResponseNotFound = await _client.GetAsync($"/v2/case-actions/nti-under-consideration/{createdNTI.Id}");

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.NoContent);
			getResponseNotFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task When_Delete_WithMissingNTIUnderConsideration_Returns_404Response()
		{
			//Arrange
			var createdSRMAId = -1;

			//Act
			var result = await _client.DeleteAsync($"/v2/case-actions/nti-under-consideration/{createdSRMAId}");

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);

			var message = await result.Content.ReadAsStringAsync();
			message.Should().Contain($"Not Found: NTI Under Consideration with id {createdSRMAId}");
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

		protected CreateNTIUnderConsiderationRequest CreateNoticeToImproveRequestForCase(Int32 caseUrn)
		{
			var request = new CreateNTIUnderConsiderationRequest()
			{
				CaseUrn = caseUrn,
				Notes = _fixture.Create<String>(),
				CreatedAt = _fixture.Create<DateTime>(),
				UpdatedAt = _fixture.Create<DateTime>()
			};
			request.UnderConsiderationReasonsMapping = new List<int>();
			request.UnderConsiderationReasonsMapping.Add(1);
			request.UnderConsiderationReasonsMapping.Add(2);

			return request;
		}
		private async Task<NTIUnderConsiderationResponse> CreateNTIforCase(Int32 caseUrn)
		{
			var request = CreateNoticeToImproveRequestForCase(caseUrn);
			return await CreateAndGetNTI(request);
		}

		private async Task<NTIUnderConsiderationResponse> CreateAndGetNTI(CreateNTIUnderConsiderationRequest request)
		{
			var result = await _client.PostAsync(Post.Create(), request.ConvertToJson());
			var resultContent = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<NTIUnderConsiderationResponse>>();
			var createdEntityUrl = result.Headers.Location;

			result.StatusCode.Should().Be(HttpStatusCode.Created);
			resultContent.Should().NotBeNull();
			createdEntityUrl.Should().NotBeNull();

			var getResponse = await _client.GetAsync(createdEntityUrl);
			var getResponseContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<NTIUnderConsiderationResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseContent.Data.Should().NotBeNull();

			return getResponseContent.Data;
		}

		protected PatchNTIUnderConsiderationRequest CreateNTIUpdateRequest(ConcernsCaseResponse createdConcern, NTIUnderConsiderationResponse createdNTI)
		{
			var request = new PatchNTIUnderConsiderationRequest();
			request.CaseUrn = createdConcern.Urn;
			request.Id = createdNTI.Id;

			request.UnderConsiderationReasonsMapping = new List<int>(createdNTI.UnderConsiderationReasonsMapping);
			request.UnderConsiderationReasonsMapping.Remove(1);

			request.ClosedStatusId = 2;
			request.ClosedAt = _fixture.Create<DateTime>();

			request.UpdatedAt = _fixture.Create<DateTime>();
			return request;
		}
		protected async Task<NTIUnderConsiderationResponse> GetNTI(string url)
		{
			var getResponse = await _client.GetAsync(url);
			var getResponseContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<NTIUnderConsiderationResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseContent.Data.Should().NotBeNull();

			return getResponseContent.Data;
		}

		protected async Task AssertCaseLastUpdatedDateMatchesNTICreatedAt(ConcernsCaseResponse createdCase, NTIUnderConsiderationResponse createdNti)
		{
			await AssertCaseLastUpdatedDateValid(createdCase.Urn, createdNti.CreatedAt);
		}

		protected async Task AssertCaseLastUpdatedDateMatchesNTIUpdatedAt(ConcernsCaseResponse createdCase, NTIUnderConsiderationResponse updatedNti)
		{
			await AssertCaseLastUpdatedDateValid(createdCase.Urn, updatedNti.UpdatedAt.Value);
		}

		protected async Task AssertCaseLastUpdatedDateValid(Int32 caseUrn, DateTime date)
		{
			var updatedCase = await GetCase(caseUrn);
			updatedCase.CaseLastUpdatedAt.Should().Be(date);
		}

		private async Task<ConcernsCaseResponse> GetCase(Int32 urn)
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/urn/{urn}");
			var getResponseCase = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseCase.Data.Should().NotBeNull();

			return getResponseCase.Data;
		}

		public static class Get
		{
			public static string ItemById(long id)
			{
				return $"/v2/case-actions/nti-under-consideration/{id}";
			}
		}

		public static class Post
		{
			public static string Create()
			{
				return $"/v2/case-actions/nti-under-consideration";
			}
		}

		public static class Patch
		{
			public static string Update()
			{
				return $"/v2/case-actions/nti-under-consideration";
			}
		}

	}
}
