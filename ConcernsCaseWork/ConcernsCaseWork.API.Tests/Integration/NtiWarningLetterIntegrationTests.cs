using AutoFixture;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
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
	public class NtiWarningLetterIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly RandomGenerator _randomGenerator;

		public NtiWarningLetterIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_randomGenerator = new RandomGenerator();
		}

		[Fact]
		public async Task When_Post_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<CreateNTIWarningLetterRequest>();
			request.CreatedBy = new string('a', 301);
			request.Notes = new string('a', 2001);

			var result = await _client.PostAsync($"/v2/case-actions/nti-warning-letter", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field CreatedBy must be a string with a maximum length of 300.");
			error.Should().Contain("The field Notes must be a string with a maximum length of 2000.");
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			//Arrange
			var createdCase = await CreateCase();
			var request = CreateNoticeToImproveRequestForCase(createdCase.Urn);

			//Arrange and Act
			var createdNTI = await CreateAndGetNTI(request);

			//Assert
			createdNTI.Should().BeEquivalentTo(request);
			createdNTI.WarningLetterConditionsMapping.Should().HaveCount(2);
			createdNTI.WarningLetterReasonsMapping.Should().HaveCount(3);

			await AssertCaseLastUpdatedDateMatchesNTICreatedAt(createdCase, createdNTI);
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

		protected CreateNTIWarningLetterRequest CreateNoticeToImproveRequestForCase(Int32 caseUrn)
		{
			var request = new CreateNTIWarningLetterRequest()
			{
				CaseUrn = caseUrn,
				StatusId = 1,
				DateLetterSent = _fixture.Create<DateTime>(),
				Notes = _fixture.Create<String>(),
				CreatedAt = _fixture.Create<DateTime>(),
				UpdatedAt = _fixture.Create<DateTime>()
			};
			request.WarningLetterConditionsMapping = new List<int>();
			request.WarningLetterConditionsMapping.Add(1);
			request.WarningLetterConditionsMapping.Add(2);
			request.WarningLetterReasonsMapping = new List<int>();
			request.WarningLetterReasonsMapping.Add(1);
			request.WarningLetterReasonsMapping.Add(2);
			request.WarningLetterReasonsMapping.Add(3);

			return request;
		}
		private async Task<NTIWarningLetterResponse> CreateNTIforCase(Int32 caseUrn)
		{
			var request = CreateNoticeToImproveRequestForCase(caseUrn);
			return await CreateAndGetNTI(request);
		}
		private async Task<NTIWarningLetterResponse> CreateAndGetNTI(CreateNTIWarningLetterRequest request)
		{
			var result = await _client.PostAsync(Post.Create(), request.ConvertToJson());
			var resultContent = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<NTIWarningLetterResponse>>();
			var createdEntityUrl = result.Headers.Location;

			result.StatusCode.Should().Be(HttpStatusCode.Created);
			resultContent.Should().NotBeNull();
			createdEntityUrl.Should().NotBeNull();

			var getResponse = await _client.GetAsync(createdEntityUrl);
			var getResponseContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<NTIWarningLetterResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseContent.Data.Should().NotBeNull();

			return getResponseContent.Data;
		}

		protected PatchNTIWarningLetterRequest CreateNTIUpdateRequest(ConcernsCaseResponse createdConcern, NTIWarningLetterResponse createdNTI)
		{
			var request = new PatchNTIWarningLetterRequest();
			request.CaseUrn = createdConcern.Urn;
			request.Id = createdNTI.Id;

			request.StatusId = 2;
			request.WarningLetterConditionsMapping = new List<int>(createdNTI.WarningLetterConditionsMapping);
			request.WarningLetterConditionsMapping.Remove(1);
			request.WarningLetterReasonsMapping = new List<int>(createdNTI.WarningLetterReasonsMapping);
			request.WarningLetterReasonsMapping.Clear();

			request.ClosedStatusId = 5;
			request.ClosedAt = _fixture.Create<DateTime>();

			request.Notes = _fixture.Create<String>();
			request.DateLetterSent = _fixture.Create<DateTime>();
			request.UpdatedAt = _fixture.Create<DateTime>();
			

			return request;
		}

		protected async Task<NTIWarningLetterResponse> GetNTI(string url)
		{
			var getResponse = await _client.GetAsync(url);
			var getResponseContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<NTIWarningLetterResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseContent.Data.Should().NotBeNull();

			return getResponseContent.Data;
		}

		protected async Task AssertCaseLastUpdatedDateMatchesNTICreatedAt(ConcernsCaseResponse createdCase, NTIWarningLetterResponse createdNti)
		{
			await AssertCaseLastUpdatedDateValid(createdCase.Urn, createdNti.CreatedAt);
		}
		protected async Task AssertCaseLastUpdatedDateMatchesNTIUpdatedAt(ConcernsCaseResponse createdCase, NTIWarningLetterResponse updatedNti)
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


		public static class Post
		{
			public static string Create()
			{
				return $"/v2/case-actions/nti-warning-letter";
			}
		}


		public static class Patch
		{
			public static string Update()
			{
				return $"/v2/case-actions/nti-warning-letter";
			}
		}

		public static class Get
		{
			public static string ItemById(long id)
			{
				return $"/v2/case-actions/nti-warning-letter/{id}";
			}
		}

		[Fact]
		public async Task When_Patch_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<PatchNTIWarningLetterRequest>();
			request.CreatedBy = new string('a', 301);
			request.Notes = new string('a', 2001);

			var result = await _client.PatchAsync($"/v2/case-actions/nti-warning-letter", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field CreatedBy must be a string with a maximum length of 300.");
			error.Should().Contain("The field Notes must be a string with a maximum length of 2000.");
		}



		[Fact]
		public async Task When_Delete_InvalidRequest_Returns_BadRequest()
		{
			var warningLetterID = 0;

			var result = await _client.DeleteAsync($"/v2/case-actions/nti-warning-letter/{warningLetterID}");
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task When_Delete_NotCreatedResourceRequest_Returns_NotFound()
		{
			var warningLetterID = 10000000;

			var result = await _client.DeleteAsync($"/v2/case-actions/nti-warning-letter/{warningLetterID}");
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task When_Delete_ValidResourceRequest_Returns_NoContent()
		{
			//Create the case
			ConcernCaseRequest createCaseRequest = Builder<ConcernCaseRequest>.CreateNew()
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

			var createCaseResponse = await _client.PostAsync($"v2/concerns-cases", createCaseRequest.ConvertToJson());
			var response = await createCaseResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

			var CaseUrn = response.Data.Urn;
			//create the warning letter
			var request = _fixture.Create<CreateNTIWarningLetterRequest>();
			request.CaseUrn = CaseUrn;
			request.ClosedStatusId = 1;
			request.StatusId = 1;
			request.WarningLetterConditionsMapping.Clear();
			request.WarningLetterConditionsMapping.Add(1);
			request.WarningLetterReasonsMapping.Clear();
			request.WarningLetterReasonsMapping.Add(1);


			var result = await _client.PostAsync($"/v2/case-actions/nti-warning-letter", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.Created);

			var createdWarningLetterUri = result.Headers.Location;
			//check for the warning letter
			var getResponse = await _client.GetAsync(createdWarningLetterUri);
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			//delete the warning letter
			var deleteResponse = await _client.DeleteAsync(createdWarningLetterUri);
			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			//check it is not returned
			var getResponseNotFound = await _client.GetAsync(createdWarningLetterUri);
			getResponseNotFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}