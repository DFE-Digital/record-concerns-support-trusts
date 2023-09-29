using AutoFixture;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
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
	public class TrustFinancialForecastIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly RandomGenerator _randomGenerator;

		public TrustFinancialForecastIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_randomGenerator = new RandomGenerator();
		}



		[Fact]
		public async Task When_GET_TFFNotExistReturns_NotFound()
		{
			//Arrange
			var id = _fixture.Create<int>();
			var CaseUrn = _fixture.Create<int>();

			//Act
			var result = await _client.GetAsync(Get.ItemById(CaseUrn,id));

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}


		[Fact]
		public async Task When_Post_CaseWithInvalidCaseUrn_BadRequest()
		{
			//Arrange
			var request = CreateRequestWithInvalidCaseId();

			//Act
			var result = await _client.PostAsync(Post.CreateTFF(request.CaseUrn), request.ConvertToJson());

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task When_Post_CaseWithNotesFieldLongerThan2000_BadRequest()
		{
			//Arrange
			var request = CreateRequestWithInvalidNotes();

			//Act
			var result = await _client.PostAsync(Post.CreateTFF(request.CaseUrn), request.ConvertToJson());

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task When_Post_CaseNotExistReturns_NotFound()
		{
			//Arrange
			var request = _fixture.Create<CreateTrustFinancialForecastRequest>();
			request.CaseUrn = _fixture.Create<int>();

			//Act
			var result = await _client.PostAsync(Post.CreateTFF(request.CaseUrn), request.ConvertToJson());

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var request = _fixture.Create<CreateTrustFinancialForecastRequest>();
			//Todo Revisit why difference in date accuracy is an issue here. UI only displays to Date level so this is acceptable in short term.
			request.TrustRespondedAt = _fixture.Create<DateTime>().Date;
			request.SFSOInitialReviewHappenedAt = _fixture.Create<DateTime>().Date;
			request.CaseUrn = createdConcern.Urn;

			//Act
			var createdTFF = await CreateAndGetTFF(request);

			//Assert
			createdTFF.Should().BeEquivalentTo(request);
			await AssertCaseLastUpdatedDateMatchesTFFCreatedAt(createdConcern, createdTFF);
		}

		[Fact]
		public async Task When_Put_UpdateTFFWithInvalidCaseId_ReturnBadRequest()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdTFF = await CreateTFFforCase(createdConcern.Urn);
			var request = UpdateRequestWithInvalidCaseId();

			//Act
			var result = await _client.PutAsync(Put.UpdateTFF(request), request.ConvertToJson());

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task When_Put_UpdateTFFWithInvalidNotesLength_ReturnBadRequest()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdTFF = await CreateTFFforCase(createdConcern.Urn);
			var request = UpdateRequestWithInvalidNotes();

			//Act
			var result = await _client.PutAsync(Put.UpdateTFF(request), request.ConvertToJson());

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task When_Put_UpdateTFF_ReturnOK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdTFF = await CreateTFFforCase(createdConcern.Urn);
			var request = CreateTFFUpdateRequest(createdConcern, createdTFF);

			//Act
			var result = await _client.PutAsync(Put.UpdateTFF(request), request.ConvertToJson());
			var response = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<String>>();
			var updatedTFF = await GetTFF(Get.ItemById(request.CaseUrn, request.TrustFinancialForecastId));

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().NotBeNull();
			updatedTFF.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
			await AssertCaseLastUpdatedDateMatchesTFFUpdatedAt(createdConcern, updatedTFF);
		}

		[Fact]
		public async Task When_Patch_CloseAnOpenTFF_ReturnOK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdTFF = await CreateTFFforCase(createdConcern.Urn);
			var request = new CloseTrustFinancialForecastRequest { CaseUrn = createdConcern.Urn, TrustFinancialForecastId = createdTFF.TrustFinancialForecastId, Notes = _fixture.Create<String>() };

			//Act
			var result = await _client.PatchAsync(Patch.UpdateTFF(request), request.ConvertToJson());
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<String>>();
			var updatedTFF = await GetTFF(Get.ItemById(request.CaseUrn, request.TrustFinancialForecastId));

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			updatedTFF.Notes.Should().Be(request.Notes);
			updatedTFF.ClosedAt.Should().NotBeNull();
			updatedTFF.ClosedAt.Value.Date.Should().Be(System.DateTime.Now.Date);
			await AssertCaseLastUpdatedDateMatchesTFFUpdatedAt(createdConcern, updatedTFF);
		}


		[Fact]
		public async Task When_Put_ClosedTFF_ReturnInternalServerError()
		{
			//Observation: 08/08/2023 Api returns an Internal Server Error if we try to update and existing Closed TFF.
			//Todo: Consider amending to retur Bad Request as part of separate refactor

			//Arrange
			var createdConcern = await CreateCase();
			var createdTFF = await CreateTFFforCase(createdConcern.Urn);
			await CloseTFF(createdConcern, createdTFF);
			var request = new CloseTrustFinancialForecastRequest { CaseUrn = createdConcern.Urn, TrustFinancialForecastId = createdTFF.TrustFinancialForecastId, Notes = _fixture.Create<String>() };

			//Act
			var updateRequest = CreateTFFUpdateRequest(createdConcern, createdTFF);
			var updateResponse = await _client.PutAsync(Put.UpdateTFF(updateRequest), request.ConvertToJson());

			//Assert
			updateResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		}

		[Fact]
		public async Task When_Delete_InvalidRequest_Returns_BadRequest()
		{
			//Arrange
			var urn = 1;
			var TrustFinancialForecastId = 0;
			
			//Act
			var result = await _client.DeleteAsync(Delete.DeleteTFF(urn, TrustFinancialForecastId));
			
			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task When_Delete_NotCreatedResourceRequest_Returns_NotFound()
		{
			//Arrange
			var urn = 1;
			var TrustFinancialForecastId = 1000000;

			//Act
			var result = await _client.DeleteAsync(Delete.DeleteTFF(urn, TrustFinancialForecastId));

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task When_Delete_ValidResourceRequest_Returns_NoContent()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdTFF = await CreateTFFforCase(createdConcern.Urn);

			//Act
			var deleteResponse = await _client.DeleteAsync(Delete.DeleteTFF(createdTFF.CaseUrn, createdTFF.TrustFinancialForecastId));
			var getResponseNotFound = await _client.GetAsync(Delete.DeleteTFF(createdTFF.CaseUrn, createdTFF.TrustFinancialForecastId));

			//Assert
			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
			getResponseNotFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		protected UpdateTrustFinancialForecastRequest CreateTFFUpdateRequest(ConcernsCaseResponse createdConcern, TrustFinancialForecastResponse createdTFF)
		{
			var request = _fixture.Create<UpdateTrustFinancialForecastRequest>();
			request.CaseUrn = createdConcern.Urn;
			request.TrustFinancialForecastId = createdTFF.TrustFinancialForecastId;
			//Todo Revisit why difference in date accuracy is an issue here.
			request.TrustRespondedAt = _fixture.Create<DateTime>().Date;
			request.SFSOInitialReviewHappenedAt = _fixture.Create<DateTime>().Date;
			return request;
		}

		protected async Task<TrustFinancialForecastResponse> GetTFF(string url)
		{
			var getResponse = await _client.GetAsync(url);
			var getResponseContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<TrustFinancialForecastResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseContent.Data.Should().NotBeNull();

			return getResponseContent.Data;
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

		private async Task<TrustFinancialForecastResponse> CreateAndGetTFF(CreateTrustFinancialForecastRequest request)
		{
			var result = await _client.PostAsync($"/v2/concerns-cases/{request.CaseUrn}/trustfinancialforecast", request.ConvertToJson());
			var resultContent = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<String>>();
			var createdEntityUrl = result.Headers.Location;

			var getResponse = await _client.GetAsync(createdEntityUrl);
			var getResponseContent = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<TrustFinancialForecastResponse>>();

			result.StatusCode.Should().Be(HttpStatusCode.Created);
			resultContent.Should().NotBeNull();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseContent.Data.Should().NotBeNull();

			return getResponseContent.Data;
		}

		private async Task<TrustFinancialForecastResponse> CreateTFFforCase(Int32 caseUrn)
		{
			var request = _fixture.Create<CreateTrustFinancialForecastRequest>();
			//Todo Revisit why difference in date accuracy is an issue here.
			request.TrustRespondedAt = _fixture.Create<DateTime>().Date;
			request.SFSOInitialReviewHappenedAt = _fixture.Create<DateTime>().Date;
			request.CaseUrn = caseUrn;
			return await CreateAndGetTFF(request);
		}

		private async Task<ConcernsCaseResponse> GetCase(Int32 urn)
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/urn/{urn}");
			var getResponseCase = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseCase.Data.Should().NotBeNull();

			return getResponseCase.Data;
		}
		protected async Task CloseTFF(ConcernsCaseResponse createdConcern, TrustFinancialForecastResponse createdTFF)
		{
			var request = new CloseTrustFinancialForecastRequest { CaseUrn = createdConcern.Urn, TrustFinancialForecastId = createdTFF.TrustFinancialForecastId, Notes = _fixture.Create<String>() };
			var result = await _client.PatchAsync(Patch.UpdateTFF(request), request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		protected CreateTrustFinancialForecastRequest CreateRequestWithInvalidCaseId()
		{
			var request = _fixture.Create<CreateTrustFinancialForecastRequest>();
			request.CaseUrn = 0;
			return request;
		}

		protected CreateTrustFinancialForecastRequest CreateRequestWithInvalidNotes()
		{
			var request = _fixture.Create<CreateTrustFinancialForecastRequest>();
			request.Notes = string.Join("", _fixture.CreateMany<char>(2001));
			return request;
		}

		protected UpdateTrustFinancialForecastRequest UpdateRequestWithInvalidCaseId()
		{
			var request = _fixture.Create<UpdateTrustFinancialForecastRequest>();
			request.CaseUrn = 0;
			return request;
		}

		protected UpdateTrustFinancialForecastRequest UpdateRequestWithInvalidNotes()
		{
			var request = _fixture.Create<UpdateTrustFinancialForecastRequest>();
			request.Notes = string.Join("", _fixture.CreateMany<char>(2001));
			return request;
		}

		protected async Task AssertCaseLastUpdatedDateMatchesTFFCreatedAt(ConcernsCaseResponse createdCase, TrustFinancialForecastResponse createdTFF)
		{
			await AssertCaseLastUpdatedDateValid(createdCase.Urn, createdTFF.CreatedAt);
		}

		protected async Task AssertCaseLastUpdatedDateMatchesTFFUpdatedAt(ConcernsCaseResponse createdCase, TrustFinancialForecastResponse updatedTFF)
		{
			await AssertCaseLastUpdatedDateValid(createdCase.Urn, updatedTFF.UpdatedAt);
		}


		protected async Task AssertCaseLastUpdatedDateValid(Int32 caseUrn, DateTimeOffset date)
		{
			var updatedCase = await GetCase(caseUrn);
			updatedCase.CaseLastUpdatedAt.Should().Be(date.DateTime);
		}

		public static class Get
		{
			public static string ItemById(int urn, int id)
			{
				return $"/v2/concerns-cases/{urn}/trustfinancialforecast/{id}";
			}
		}

		public static class Put
		{
			public static string UpdateTFF(UpdateTrustFinancialForecastRequest request)
			{
				return UpdateTFF(request.CaseUrn, request.TrustFinancialForecastId);
			}

			public static string UpdateTFF(int urn, int id)
			{
				return $"/v2/concerns-cases/{urn}/trustfinancialforecast/{id}";
			}
		}

		public static class Patch
		{
			public static string UpdateTFF(CloseTrustFinancialForecastRequest request)
			{
				return UpdateTFF(request.CaseUrn, request.TrustFinancialForecastId);
			}

			public static string UpdateTFF(int urn, int id)
			{
				return $"/v2/concerns-cases/{urn}/trustfinancialforecast/{id}";
			}
		}

		public static class Post
		{
			public static string CreateTFF(int urn)
			{
				return $"/v2/concerns-cases/{urn}/trustfinancialforecast";
			}
		}

		public static class Delete
		{
			public static string DeleteTFF(int urn, int id)
			{
				return $"/v2/concerns-cases/{urn}/trustfinancialforecast/{id}";
			}
		}

	}
}
