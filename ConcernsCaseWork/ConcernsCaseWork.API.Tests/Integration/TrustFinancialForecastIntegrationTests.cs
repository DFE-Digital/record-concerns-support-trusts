using AutoFixture;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;

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
		public async Task When_Delete_InvalidRequest_Returns_BadRequest()
		{
			var urn = "1";
			var noticeToImproveId = 0;
			var result = await _client.DeleteAsync($"/v2/concerns-cases/{urn}/trustfinancialforecast/{noticeToImproveId}");
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task When_Delete_NotCreatedResourceRequest_Returns_NotFound()
		{
			var urn = "1";
			var noticeToImproveId = 1000000;
			var result = await _client.DeleteAsync($"/v2/concerns-cases/{urn}/trustfinancialforecast/{noticeToImproveId}");
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
				.With(c => c.TrustCompaniesHouseNumber = "12345678")
				.Build();

			var createCaseResponse = await _client.PostAsync($"v2/concerns-cases", createCaseRequest.ConvertToJson());
			var response = await createCaseResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

			var CaseUrn = response.Data.Urn;
			//create the tff
			var request = Builder<CreateTrustFinancialForecastRequest>.CreateNew()
				.With(c => c.CaseUrn = CaseUrn)
				.With(c => c.SRMAOfferedAfterTFF = SRMAOfferedAfterTFF.Yes)
				.With(c => c.ForecastingToolRanAt = ForecastingToolRanAt.CurrentYearSpring)
				.With(c => c.WasTrustResponseSatisfactory = WasTrustResponseSatisfactory.Satisfactory)
				.With(c => c.Notes = "Here are the notes")
				.With(c => c.SFSOInitialReviewHappenedAt = System.DateTime.UtcNow)
				.With(c => c.TrustRespondedAt = System.DateTime.UtcNow)
				.Build();

			var result = await _client.PostAsync($"/v2/concerns-cases/{CaseUrn}/trustfinancialforecast", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.Created);

			var createdTFF = result.Headers.Location;
			//check for the tff
			var getResponse = await _client.GetAsync(createdTFF);
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			//delete the tff
			var deleteResponse = await _client.DeleteAsync(createdTFF);
			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			//check it is not returned
			var getResponseNotFound = await _client.GetAsync(createdTFF);
			getResponseNotFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

	}
}
