using AutoFixture;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
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
				.With(c => c.TrustUkprn = "100223")
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