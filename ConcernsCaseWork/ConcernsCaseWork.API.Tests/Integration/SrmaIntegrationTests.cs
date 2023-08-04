using AutoFixture;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.API.UseCases.CaseActions.FinancialPlan;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Models.CaseActions;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection(ApiTestCollection.ApiTestCollectionName)]
	public class SrmaIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly RandomGenerator _randomGenerator;

		public SrmaIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_randomGenerator = new RandomGenerator();
		}

		[Fact]
		public async Task When_Post_InvalidRequest_Returns_ValidationErrors()
		{
			var request = _fixture.Create<CreateSRMARequest>();
			request.CreatedBy = new string('a', 301);
			request.Notes = new string('a', 5001);

			var result = await _client.PostAsync($"/v2/case-actions/srma", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);


			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field CreatedBy must be a string with a maximum length of 300.");
			error.Should().Contain("The field Notes must be a string with a maximum length of 5000.");
		}

		[Fact]
		public async Task When_Patch_InvalidNotes_Returns_ValidationErrors()
		{
			var notes = new string('a', 5001);

			var result = await _client.PatchAsync($"/v2/case-actions/srma/1/update-notes?notes={notes}", null);
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field notes must be a string with a maximum length of 5000.");

		}


		[Fact]
		public async Task When_Post_Returns_201Response()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var request = _fixture.Create<CreateSRMARequest>();
			request.Id = 0;
			request.CaseUrn = createdConcern.Urn;
			request.Reason = Data.Enums.SRMAReasonOffered.RegionsGroupIntervention;

			//Act
			var createdSRMA = await CreateSRMA(request);

			//Assert
			createdSRMA.Should().BeEquivalentTo(request, 
				options => options.ExcludingMissingMembers()
					.Excluding(f=> f.Id)
				);
		}

		[Fact]
		public async Task When_UpdateStatus_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);

			var statusList = new Fixture().Create<Generator<SRMAStatus>>();
			var newStatus = statusList.Where(f => f != createdSRMA.Status).FirstOrDefault();

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-status?status={(int)newStatus}",null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.Status.Should().Be(newStatus);
		}

		[Fact]
		public async Task When_UpdateReason_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);

			var reasonList = new Fixture().Create<Generator<SRMAReasonOffered>>();
			var newReason = reasonList.Where(f => f != createdSRMA.Reason && f != SRMAReasonOffered.Unknown).FirstOrDefault();

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-reason?reason={(int)newReason}", null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.Reason.Should().Be(newReason);
		}

		[Fact]
		public async Task When_UpdateOfferedDate_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);

			var newOfferedDate = createdSRMA.DateOffered.AddDays(90);

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-offered-date?offeredDate={newOfferedDate.ToString("dd-MM-yyyy")}", null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.DateOffered.Date.Should().Be(newOfferedDate.Date);
		}

		[Fact]
		public async Task When_UpdateNotes_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);

			var newNotes = _fixture.Create<String>();

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-notes?notes={newNotes}", null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.Notes.Should().Be(newNotes);
		}

		[Fact]
		public async Task When_UpdateVisitDate_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);

			var newStartDate = createdSRMA.DateVisitStart.GetValueOrDefault(System.DateTime.Now).AddDays(90);
			var newEndDate = newStartDate.AddDays(30);

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-visit-dates?startDate={newStartDate.ToString("dd-MM-yyyy")}&&endDate={newEndDate.ToString("dd-MM-yyyy")}", null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.DateVisitStart.Value.Date.Should().Be(newStartDate.Date);
			patchResponse.Data.DateVisitEnd.Value.Date.Should().Be(newEndDate.Date);

		}

		[Fact]
		public async Task When_UpdateDateAccepted_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);

			var newDate = createdSRMA.DateAccepted.GetValueOrDefault(System.DateTime.Now).AddDays(90);

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-date-accepted?acceptedDate={newDate.ToString("dd-MM-yyyy")}", null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.DateAccepted.Value.Date.Should().Be(newDate.Date);
		}

		[Fact]
		public async Task When_UpdateDateReportSent_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);

			var newDate = createdSRMA.DateReportSentToTrust.GetValueOrDefault(System.DateTime.Now).AddDays(90);

			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-date-report-sent?dateReportSent={newDate.ToString("dd-MM-yyyy")}", null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.DateReportSentToTrust.Value.Date.Should().Be(newDate.Date);
		}

		[Fact]
		public async Task When_UpdateDateClosed_Patch_Return_OK()
		{
			//Arrange
			var createdConcern = await CreateCase();
			var createdSRMA = await CreateSRMAForCase(createdConcern.Urn);


			//Act
			var result = await _client.PatchAsync($"/v2/case-actions/srma/{createdSRMA.Id}/update-closed-date", null);
			var patchResponse = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			//Assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
			patchResponse.Data.Should().NotBeNull();
			patchResponse.Data.ClosedAt.Should().NotBe(createdSRMA.ClosedAt);
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
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/urn/{urn}");
			var getResponseCase = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseCase.Data.Should().NotBeNull();

			return getResponseCase.Data;
		}
		private async Task<SRMAResponse> CreateSRMAForCase(Int32 caseUrn)
		{
			var request = _fixture.Create<CreateSRMARequest>();
			request.Id = 0;
			request.CaseUrn = caseUrn;
			request.Reason = Data.Enums.SRMAReasonOffered.RegionsGroupIntervention;
			return await CreateSRMA(request);
		}

		private async Task<SRMAResponse> CreateSRMA(CreateSRMARequest request)
		{
			var result = await _client.PostAsync($"/v2/case-actions/srma", request.ConvertToJson());
			var SRMA = await result.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();
			var createdEntityUrl = result.Headers.Location;

			var getResponse = await _client.GetAsync(createdEntityUrl);
			var getResponseSRMA = await getResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<SRMAResponse>>();

			result.StatusCode.Should().Be(HttpStatusCode.Created);
			SRMA.Should().NotBeNull();

			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			getResponseSRMA.Should().NotBeNull();

			return getResponseSRMA.Data;
		}
	}
}
