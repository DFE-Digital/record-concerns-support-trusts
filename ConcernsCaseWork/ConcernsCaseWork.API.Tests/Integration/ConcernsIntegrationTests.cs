using AutoFixture;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
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
	public class ConcernsIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly Fixture _autoFixture;
        private readonly RandomGenerator _randomGenerator;
		private ApiTestFixture _testFixture;

		private List<ConcernsCase> CasesToBeDisposedAtEndOfTests { get; } = new List<ConcernsCase>();
        private List<ConcernsRecord> RecordsToBeDisposedAtEndOfTests { get; } = new List<ConcernsRecord>();

        public ConcernsIntegrationTests(ApiTestFixture fixture)
        {
            _autoFixture = new Fixture();
            _randomGenerator = new RandomGenerator();
			_testFixture = fixture;
			_client = fixture.Client;
        }

        [Fact]
        public async Task CanCreateNewConcernCase()
        {
            var createRequest = Builder<ConcernCaseRequest>.CreateNew()
                .With(c => c.CreatedBy = "12345")
                .With(c => c.Description = "")
                .With(c => c.CrmEnquiry = "")
                .With(c => c.TrustUkprn = "100223")
                .With(c => c.ReasonAtReview = "")
                .With(c => c.DeEscalation = new DateTime(2022,04,01))
                .With(c => c.Issue = "Here is the issue")
                .With(c => c.CurrentStatus = "Case status")
                .With(c => c.CaseAim = "Here is the aim")
                .With(c => c.DeEscalationPoint = "Point of de-escalation")
                .With(c => c.CaseHistory = "Case history")
                .With(c => c.NextSteps = "Here are the next steps")
                .With(c => c.DirectionOfTravel = "Up")
                .With(c => c.StatusId = 1)
                .With(c => c.RatingId = 2)
                .Build();


            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases"),
                Content =  JsonContent.Create(createRequest)
            };

            var caseToBeCreated = ConcernsCaseFactory.Create(createRequest);
            var expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(caseToBeCreated);

            var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(expectedConcernsCaseResponse);

            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

			using var context = _testFixture.GetContext();

			var createdCase = context.ConcernsCase.FirstOrDefault(c => c.Urn == result.Data.Urn);
            expected.Data.Urn = createdCase.Urn;

            result.Should().BeEquivalentTo(expected);
            createdCase.Description.Should().BeEquivalentTo(createRequest.Description);
        }

		[Fact]
		public async Task When_PostInvalidConcernCaseRequest_Returns_ValidationErrors()
		{
			var request = _autoFixture.Create<ConcernCaseRequest>();
			request.TrustUkprn = new string('a', 51);
			request.Issue = new string('a', 2001);
			request.CaseAim = new string('a', 1001);
			request.CurrentStatus = new string('a', 4001);
			request.DeEscalationPoint = new string('a', 1001);
			request.NextSteps = new string('a', 4001);
			request.CaseHistory = new string('a', 4001);
			request.DirectionOfTravel = new string('a', 101);

			var result = await _client.PostAsync($"/v2/concerns-cases", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field TrustUkprn must be a string with a maximum length of 50.");
			error.Should().Contain("The field Issue must be a string with a maximum length of 2000.");
			error.Should().Contain("The field CaseAim must be a string with a maximum length of 1000.");
			error.Should().Contain("The field CurrentStatus must be a string with a maximum length of 4000.");
			error.Should().Contain("The field DeEscalationPoint must be a string with a maximum length of 1000.");
			error.Should().Contain("The field NextSteps must be a string with a maximum length of 4000.");
			error.Should().Contain("The field CaseHistory must be a string with a maximum length of 4000.");
			error.Should().Contain("The field DirectionOfTravel must be a string with a maximum length of 100.");
		}

		[Fact]
        public async Task CanGetConcernCaseByUrn()
        {
			using var context = _testFixture.GetContext();

			SetupConcernsCaseTestData("mockUkprn");
            var concernsCase = context.ConcernsCase.First();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/urn/{concernsCase.Urn}")
            };

            var expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(concernsCase);

            var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(expectedConcernsCaseResponse);

            var response = await _client.SendAsync(httpRequestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

            result.Should().BeEquivalentTo(expected);
            result.Data.Urn.Should().Be(concernsCase.Urn);
        }

        [Fact]
        public async Task CanGetConcernCaseByTrustUkprn()
        {
            var ukprn = "100008";

            var expectedData = SetupConcernsCaseTestData(ukprn);
            var concernsCase = expectedData.First();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}")
            };

            var expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(concernsCase);
            var expectedPaging = new PagingResponse {Page = 1, RecordCount = expectedData.Count};

            var expected = new ApiResponseV2<ConcernsCaseResponse>(new List<ConcernsCaseResponse>{expectedConcernsCaseResponse}, expectedPaging);

            var response = await _client.SendAsync(httpRequestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

            result.Data.Count(d => d.Urn == concernsCase.Urn).Should().Be(1);
        }

        [Fact]
        public async Task CanGetMultipleConcernCasesByTrustUkprn()
        {
            var ukprn = "100008";

            var concernsCases = SetupConcernsCaseTestData(ukprn, 2);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}")
            };
            var expectedPaging = new PagingResponse {Page = 1, RecordCount = concernsCases.Count};

            var expectedConcernsCaseResponse = concernsCases.Select(c => ConcernsCaseResponseFactory.Create(c)).ToList();

            var expected = new ApiResponseV2<ConcernsCaseResponse>(expectedConcernsCaseResponse, expectedPaging);

            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Count(d => d.Urn == concernsCases[0].Urn).Should().Be(1);
            content.Data.Count(d => d.Urn == concernsCases[1].Urn).Should().Be(1);
        }

        [Fact]
        public async Task GettingMultipleConcernCasesByTrustUkprn_WhenFewerThanCount_ShouldReturnAllItems()
        {
            var ukprn = "100005";
            var count = 20;

            var concernsCases = SetupConcernsCaseTestData(ukprn, 10);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}")
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Count().Should().Be(concernsCases.Count);
        }

        [Fact]
        public async Task GettingMultipleConcernCasesByTrustUkprn_WhenGreaterThanCount_ShouldReturnCountNumberOfItems()
        {
            var ukprn = "100008";
            var count = 5;

            SetupConcernsCaseTestData(ukprn, 10);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}")
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Count().Should().Be(count);
        }

        [Fact]
        public async Task IndexConcernsStatuses_ShouldReturnAllConcernsStatuses()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-statuses/")
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsStatusResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Count().Should().Be(3);
        }

        [Fact]
        public async Task IndexConcernsTypes_ShouldReturnAllConcernsTypes()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-types/")
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsTypeResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Count().Should().Be(13);
        }

        [Fact]
        public async Task UpdateConcernsCase_ShouldReturnTheUpdatedConcernsCase()
        {
            var currentConcernsCase = new ConcernsCase
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                CreatedBy = _randomGenerator.NextString(3, 10),
                Description = "", // not used
                CrmEnquiry = "", // not used
                TrustUkprn = _randomGenerator.NextString(3, 10),
                ReasonAtReview = "", // not used
                DeEscalation = _randomGenerator.DateTime(),
                Issue = _randomGenerator.NextString(3, 10),
                CurrentStatus = _randomGenerator.NextString(3, 10),
                CaseAim = _randomGenerator.NextString(3, 10),
                DeEscalationPoint = _randomGenerator.NextString(3, 10),
                NextSteps = _randomGenerator.NextString(3, 10),
                CaseHistory = _randomGenerator.NextString(3, 10),
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                Territory = TerritoryEnum.Midlands_And_West__SouthWest,
                StatusId = 2,
                RatingId =  1
            };

            AddConcernsCase(currentConcernsCase);

            var urn = currentConcernsCase.Urn;

            var updateRequest = Builder<ConcernCaseRequest>.CreateNew()
	            .With(cr => cr.Description = "")
	            .With(cr => cr.CrmEnquiry = "")
	            .With(cr => cr.ReasonAtReview = "")
                .With(cr => cr.RatingId = 1).Build();

            var expectedConcernsCase = ConcernsCaseFactory.Create(updateRequest);
            expectedConcernsCase.Urn = urn;
            var expectedContent = ConcernsCaseResponseFactory.Create(expectedConcernsCase);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/{urn}"),
                Content =  JsonContent.Create(updateRequest)
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Should().BeEquivalentTo(expectedContent);
        }

		[Fact]
		public async Task When_PatchInvalidConcernCaseRequest_Returns_ValidationErrors()
		{
			var request = _autoFixture.Create<ConcernCaseRequest>();
			request.TrustUkprn = new string('a', 51);
			request.Issue = new string('a', 2001);
			request.CaseAim = new string('a', 1001);
			request.CurrentStatus = new string('a', 4001);
			request.DeEscalationPoint = new string('a', 1001);
			request.NextSteps = new string('a', 4001);
			request.CaseHistory = new string('a', 4001);
			request.DirectionOfTravel = new string('a', 101);

			var result = await _client.PatchAsync($"/v2/concerns-cases/1", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field TrustUkprn must be a string with a maximum length of 50.");
			error.Should().Contain("The field Issue must be a string with a maximum length of 2000.");
			error.Should().Contain("The field CaseAim must be a string with a maximum length of 1000.");
			error.Should().Contain("The field CurrentStatus must be a string with a maximum length of 4000.");
			error.Should().Contain("The field DeEscalationPoint must be a string with a maximum length of 1000.");
			error.Should().Contain("The field NextSteps must be a string with a maximum length of 4000.");
			error.Should().Contain("The field CaseHistory must be a string with a maximum length of 4000.");
			error.Should().Contain("The field DirectionOfTravel must be a string with a maximum length of 100.");
		}

		[Fact]
        public async Task CanCreateNewConcernRecord()
        {
			using var context = _testFixture.GetContext();

			var caseRating = context.ConcernsRatings.First();

            var concernsCase = new ConcernsCase
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                CreatedBy = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                CrmEnquiry = _randomGenerator.NextString(3, 10),
                TrustUkprn = _randomGenerator.NextString(3, 10),
                ReasonAtReview = _randomGenerator.NextString(3, 10),
                DeEscalation = _randomGenerator.DateTime(),
                Issue = _randomGenerator.NextString(3, 10),
                CurrentStatus = _randomGenerator.NextString(3, 10),
                CaseAim = _randomGenerator.NextString(3, 10),
                DeEscalationPoint = _randomGenerator.NextString(3, 10),
                NextSteps = _randomGenerator.NextString(3, 10),
                CaseHistory = _randomGenerator.NextString(3, 10),
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                Territory = TerritoryEnum.North_And_Utc__Yorkshire_And_Humber,
                StatusId = 2,
                RatingId = caseRating.Id
            };

            AddConcernsCase(concernsCase);

            var linkedCase = context.ConcernsCase.First();
            var linkedType = context.ConcernsTypes.First();
            var linkedRating = context.ConcernsRatings.First();
            var meansOfReferral = context.ConcernsMeansOfReferrals.First();

            var createRequest = Builder<ConcernsRecordRequest>.CreateNew()
                .With(c => c.CaseUrn = linkedCase.Urn)
                .With(c => c.TypeId = linkedType.Id)
                .With(c => c.RatingId = linkedRating.Id)
                .With(c => c.MeansOfReferralId = meansOfReferral.Id)
                .Build();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records"),
                Content =  JsonContent.Create(createRequest)
            };

            var expectedRecordToBeCreated = ConcernsRecordFactory.Create(createRequest, linkedCase, linkedType, linkedRating, meansOfReferral);
            var expectedConcernsRecordResponse = ConcernsRecordResponseFactory.Create(expectedRecordToBeCreated);
            var expected = new ApiSingleResponseV2<ConcernsRecordResponse>(expectedConcernsRecordResponse);

            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

            var createdRecord = context.ConcernsRecord.FirstOrDefault(c => c.Id == result.Data.Id);
            expected.Data.Id = createdRecord.Id;

            result.Should().BeEquivalentTo(expected);
        }

		[Fact]
		public async Task When_PostInvalidConcernsRecordRequest_Returns_ValidationErrors()
		{
			var request = _autoFixture.Create<ConcernsRecordRequest>();
			request.Name = new string('a', 301);
			request.Description = new string('a', 301);
			request.Reason = new string('a', 301);

			var result = await _client.PostAsync($"/v2/concerns-records", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field Name must be a string with a maximum length of 300.");
			error.Should().Contain("The field Description must be a string with a maximum length of 300.");
			error.Should().Contain("The field Reason must be a string with a maximum length of 300.");
		}

		[Fact]
        public async Task UpdateConcernsRecord_ShouldReturnTheUpdatedConcernsRecord()
        {
            var currentConcernsCase = new ConcernsCase
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                CreatedBy = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                CrmEnquiry = _randomGenerator.NextString(3, 10),
                TrustUkprn = _randomGenerator.NextString(3, 10),
                ReasonAtReview = _randomGenerator.NextString(3, 10),
                DeEscalation = _randomGenerator.DateTime(),
                Issue = _randomGenerator.NextString(3, 10),
                CurrentStatus = _randomGenerator.NextString(3, 10),
                CaseAim = _randomGenerator.NextString(3, 10),
                DeEscalationPoint = _randomGenerator.NextString(3, 10),
                NextSteps = _randomGenerator.NextString(3, 10),
                CaseHistory = _randomGenerator.NextString(3, 10),
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                Territory = TerritoryEnum.Midlands_And_West__West_Midlands,
                StatusId = 2,
                RatingId = 3
            };

			using var context = _testFixture.GetContext();

			var concernsType = context.ConcernsTypes.FirstOrDefault(t => t.Id == 1);
            var concernsRating = context.ConcernsRatings.FirstOrDefault(r => r.Id == 1);
            var concernsMeansOfReferral = context.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 1);

            AddConcernsCase(currentConcernsCase);

            var currentConcernsRecord = new ConcernsRecord
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                Name = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                Reason = _randomGenerator.NextString(3, 10),
                StatusId = 1,
                CaseId = currentConcernsCase.Id,
                TypeId = concernsType.Id,
                RatingId = concernsRating.Id,
                MeansOfReferralId = concernsMeansOfReferral.Id
            };

            AddConcernsRecord(currentConcernsRecord);
            var currentRecordId = currentConcernsRecord.Id;

            var updateRequest = Builder<ConcernsRecordRequest>.CreateNew()
                .With(r => r.CaseUrn = currentConcernsCase.Urn)
                .With(r => r.TypeId = concernsType.Id)
                .With(r => r.RatingId = concernsRating.Id)
                .With(r => r.MeansOfReferralId = concernsMeansOfReferral.Id).Build();

            var expectedConcernsRecord = ConcernsRecordFactory.Update(currentConcernsRecord, updateRequest, currentConcernsCase, concernsType, concernsRating, concernsMeansOfReferral);
            expectedConcernsRecord.Id = currentRecordId;
            var expectedContent = ConcernsRecordResponseFactory.Create(expectedConcernsRecord);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"),
                Content =  JsonContent.Create(updateRequest)
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Should().BeEquivalentTo(expectedContent);
        }

		[Fact]
		public async Task When_PatchInvalidConcernsRecordRequest_Returns_ValidationErrors()
		{
			var request = _autoFixture.Create<ConcernsRecordRequest>();
			request.Name = new string('a', 301);
			request.Description = new string('a', 301);
			request.Reason = new string('a', 301);

			var result = await _client.PatchAsync($"/v2/concerns-records/1", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			var error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field Name must be a string with a maximum length of 300.");
			error.Should().Contain("The field Description must be a string with a maximum length of 300.");
			error.Should().Contain("The field Reason must be a string with a maximum length of 300.");
		}

		[Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task UpdateConcernsRecord_MeansOfReferral_ShouldReturnTheUpdatedConcernsRecord(bool hasCurrentMeansOfReferral, bool isAddingMeansOfReferral)
        {
            var concernsCase = new ConcernsCase
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                CreatedBy = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                CrmEnquiry = _randomGenerator.NextString(3, 10),
                TrustUkprn = _randomGenerator.NextString(3, 10),
                ReasonAtReview = _randomGenerator.NextString(3, 10),
                DeEscalation = _randomGenerator.DateTime(),
                Issue = _randomGenerator.NextString(3, 10),
                CurrentStatus = _randomGenerator.NextString(3, 10),
                CaseAim = _randomGenerator.NextString(3, 10),
                DeEscalationPoint = _randomGenerator.NextString(3, 10),
                NextSteps = _randomGenerator.NextString(3, 10),
                CaseHistory = _randomGenerator.NextString(3, 10),
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                Territory = TerritoryEnum.North_And_Utc__Yorkshire_And_Humber,
                StatusId = 2,
                RatingId = 3
            };

            AddConcernsCase(concernsCase);

			using var context = _testFixture.GetContext();

			var concernsType = context.ConcernsTypes.FirstOrDefault(t => t.Id == 1);
            var concernsRating = context.ConcernsRatings.FirstOrDefault(r => r.Id == 1);

            var currentMeansOfReferral = hasCurrentMeansOfReferral
                ? context.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 1)
                : null;

            var updateMeansOfReferral = isAddingMeansOfReferral
                ? context.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 2)
                : null;

            var currentConcernsRecord = new ConcernsRecord
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                Name = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                Reason = _randomGenerator.NextString(3, 10),
                StatusId = 1,
                CaseId = concernsCase.Id,
                TypeId = concernsType.Id,
                RatingId = concernsRating.Id,
                MeansOfReferralId = currentMeansOfReferral?.Id
            };

            AddConcernsRecord(currentConcernsRecord);
            var currentRecordId = currentConcernsRecord.Id;

            var updateRequest = Builder<ConcernsRecordRequest>.CreateNew()
                .With(r => r.CaseUrn = concernsCase.Urn)
                .With(r => r.ClosedAt = null)
                .With(r => r.TypeId = concernsType.Id)
                .With(r => r.RatingId = concernsRating.Id)
                .With(r => r.MeansOfReferralId = updateMeansOfReferral?.Id)
                .Build();

            var expectedConcernsRecord = ConcernsRecordFactory.Update(currentConcernsRecord, updateRequest, concernsCase, concernsType, concernsRating, updateMeansOfReferral ?? currentMeansOfReferral);
            expectedConcernsRecord.Id = currentRecordId;
            var expectedContent = ConcernsRecordResponseFactory.Create(expectedConcernsRecord);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"),
                Content =  JsonContent.Create(updateRequest)
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Should().BeEquivalentTo(expectedContent);
        }

        [Fact]
        public async Task GetConcernsRecordsByConcernsCaseUid()
        {
            var concernsCase = new ConcernsCase
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                CreatedBy = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                CrmEnquiry = _randomGenerator.NextString(3, 10),
                TrustUkprn = _randomGenerator.NextString(3, 10),
                ReasonAtReview = _randomGenerator.NextString(3, 10),
                DeEscalation = _randomGenerator.DateTime(),
                Issue = _randomGenerator.NextString(3, 10),
                CurrentStatus = _randomGenerator.NextString(3, 10),
                CaseAim = _randomGenerator.NextString(3, 10),
                DeEscalationPoint = _randomGenerator.NextString(3, 10),
                NextSteps = _randomGenerator.NextString(3, 10),
                CaseHistory = _randomGenerator.NextString(3, 10),
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                Territory = TerritoryEnum.North_And_Utc__North_West,
                StatusId = 2,
                RatingId = 4
            };

            AddConcernsCase(concernsCase);

			using var context = _testFixture.GetContext();

			var concernsRating = context.ConcernsRatings.FirstOrDefault();
            var concernsType = context.ConcernsTypes.FirstOrDefault();
            var concernsMeansOfReferral = context.ConcernsMeansOfReferrals.FirstOrDefault();

            var recordCreateRequest1 = new ConcernsRecordRequest
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                Name = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                Reason = _randomGenerator.NextString(3, 10),
                CaseUrn = concernsCase.Urn,
                TypeId = concernsType!.Id,
                RatingId = concernsRating!.Id,
                StatusId = 1,
                MeansOfReferralId = concernsMeansOfReferral!.Id
            };

            var recordCreateRequest2 = new ConcernsRecordRequest
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                Name = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                Reason = _randomGenerator.NextString(3, 10),
                CaseUrn = concernsCase.Urn,
                TypeId = concernsType.Id,
                RatingId = concernsRating.Id,
                StatusId = 1,
                MeansOfReferralId = concernsMeansOfReferral.Id
            };

            var httpCreateRequestMessage1 = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/"),
                Content =  JsonContent.Create(recordCreateRequest1)
            };

            var createResponse1 = await _client.SendAsync(httpCreateRequestMessage1);
            var content1 = await createResponse1.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
            createResponse1.StatusCode.Should().Be(HttpStatusCode.Created);

            var httpCreateRequestMessage2 = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/"),
                Content =  JsonContent.Create(recordCreateRequest2)
            };

            var createResponse2 = await _client.SendAsync(httpCreateRequestMessage2);
            var content2 = await createResponse2.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
            createResponse2.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdRecord1 = ConcernsRecordFactory
                .Create(recordCreateRequest1, concernsCase, concernsType, concernsRating, concernsMeansOfReferral);
            createdRecord1.Id = content1.Data.Id;
            var createdRecord2 = ConcernsRecordFactory
                .Create(recordCreateRequest2, concernsCase, concernsType, concernsRating, concernsMeansOfReferral);
            createdRecord2.Id = content2.Data.Id;
            var createdRecords = new List<ConcernsRecord> {createdRecord1, createdRecord2};
            var expected = createdRecords
                .Select(ConcernsRecordResponseFactory.Create).ToList();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/case/urn/{concernsCase.Urn}")
            };

            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsRecordResponse>>();
            content.Data.Count().Should().Be(2);
            content.Data.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetConcernsCaseByOwnerId()
        {
            var ownerId = "7583";

            var openConcernsCases = new List<ConcernsCase>();

            for (var i = 0; i < 5; i++)
            {
                var concernsCase = new ConcernsCase
                {
                    CreatedAt = _randomGenerator.DateTime(),
                    UpdatedAt = _randomGenerator.DateTime(),
                    ReviewAt = _randomGenerator.DateTime(),
                    ClosedAt = _randomGenerator.DateTime(),
                    CreatedBy = ownerId,
                    Description = _randomGenerator.NextString(3, 10),
                    CrmEnquiry = _randomGenerator.NextString(3, 10),
                    TrustUkprn = _randomGenerator.NextString(3, 10),
                    ReasonAtReview = _randomGenerator.NextString(3, 10),
                    DeEscalation = _randomGenerator.DateTime(),
                    Issue = _randomGenerator.NextString(3, 10),
                    CurrentStatus = _randomGenerator.NextString(3, 10),
                    CaseAim = _randomGenerator.NextString(3, 10),
                    DeEscalationPoint = _randomGenerator.NextString(3, 10),
                    NextSteps = _randomGenerator.NextString(3, 10),
                    CaseHistory = _randomGenerator.NextString(3, 10),
                    DirectionOfTravel = _randomGenerator.NextString(3, 10),
                    Territory = TerritoryEnum.North_And_Utc__Utc,
                    StatusId = 2,
                    RatingId = 1
                };

                AddConcernsCase(concernsCase);
                openConcernsCases.Add(concernsCase);
            }

            for (var i = 0; i < 5; i++)
            {
                var concernsCase = new ConcernsCase
                {
                    CreatedAt = _randomGenerator.DateTime(),
                    UpdatedAt = _randomGenerator.DateTime(),
                    ReviewAt = _randomGenerator.DateTime(),
                    ClosedAt = _randomGenerator.DateTime(),
                    CreatedBy = "9876",
                    Description = _randomGenerator.NextString(3, 10),
                    CrmEnquiry = _randomGenerator.NextString(3, 10),
                    TrustUkprn = _randomGenerator.NextString(3, 10),
                    ReasonAtReview = _randomGenerator.NextString(3, 10),
                    DeEscalation = _randomGenerator.DateTime(),
                    Issue = _randomGenerator.NextString(3, 10),
                    CurrentStatus = _randomGenerator.NextString(3, 10),
                    CaseAim = _randomGenerator.NextString(3, 10),
                    DeEscalationPoint = _randomGenerator.NextString(3, 10),
                    NextSteps = _randomGenerator.NextString(3, 10),
                    CaseHistory = _randomGenerator.NextString(3, 10),
                    DirectionOfTravel = _randomGenerator.NextString(3, 10),
                    Territory = TerritoryEnum.South_And_South_East__East_Of_England,
                    StatusId = 3,
                    RatingId = 3
                };

                AddConcernsCase(concernsCase);
            }

            var expected = openConcernsCases.Select(ConcernsCaseResponseFactory.Create).ToList();


            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/owner/{ownerId}?status=2")
            };

            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();
            content.Data.Count().Should().Be(5);
            content.Data.Should().BeEquivalentTo(expected);
        }

        private List<ConcernsCase> SetupConcernsCaseTestData(string trustUkprn, int count = 1)
        {
	        var listOfCases = new List<ConcernsCase>();
            for (var i = 0; i < count; i++)
            {
                var concernsCase = new ConcernsCase
                {
                    CreatedAt = _randomGenerator.DateTime(),
                    UpdatedAt = _randomGenerator.DateTime(),
                    ReviewAt = _randomGenerator.DateTime(),
                    ClosedAt = _randomGenerator.DateTime(),
                    CreatedBy = _randomGenerator.NextString(3, 10),
                    Description = _randomGenerator.NextString(3, 10),
                    CrmEnquiry = _randomGenerator.NextString(3, 10),
                    TrustUkprn = trustUkprn,
                    ReasonAtReview = _randomGenerator.NextString(3, 10),
                    DeEscalation = _randomGenerator.DateTime(),
                    Issue = _randomGenerator.NextString(3, 10),
                    CurrentStatus = _randomGenerator.NextString(3, 10),
                    CaseAim = _randomGenerator.NextString(3, 10),
                    DeEscalationPoint = _randomGenerator.NextString(3, 10),
                    NextSteps = _randomGenerator.NextString(3, 10),
                    DirectionOfTravel = _randomGenerator.NextString(3, 10),
                    CaseHistory = _randomGenerator.NextString(3, 10),
                    Territory = TerritoryEnum.Midlands_And_West__East_Midlands,
                    StatusId = 2,
                    RatingId = 3
                };

                AddConcernsCase(concernsCase);

                listOfCases.Add(concernsCase);
            }

	        return listOfCases;
        }

        [Fact]
        public async Task IndexConcernsMeansOfReferral_ShouldReturnAllConcernsMeansOfReferral()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-meansofreferral/")
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsMeansOfReferralResponse>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Data.Count().Should().Be(2);

            content.Data.First().Name.Should().Be("Internal");
            content.Data.First().Description.Should().Be("ESFA activity, TFFT or other departmental activity");
            content.Data.First().Id.Should().BeGreaterThan(0);

            content.Data.Last().Name.Should().Be("External");
            content.Data.Last().Description.Should().Be("CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies");
            content.Data.Last().Id.Should().BeGreaterThan(0);
        }

        public void Dispose()
        {
			using var context = _testFixture.GetContext();

			if (RecordsToBeDisposedAtEndOfTests.Any())
	        {
		        context.ConcernsRecord.RemoveRange(RecordsToBeDisposedAtEndOfTests);
		        context.SaveChanges();
		        RecordsToBeDisposedAtEndOfTests.Clear();
	        }

	        if (CasesToBeDisposedAtEndOfTests.Any())
	        {
				context.ConcernsCase.RemoveRange(CasesToBeDisposedAtEndOfTests);
				context.SaveChanges();
				CasesToBeDisposedAtEndOfTests.Clear();
	        }
        }

        private void AddConcernsCase(ConcernsCase concernsCase)
        {
			using var context = _testFixture.GetContext();

			try
	        {
		        context.ConcernsCase.Add(concernsCase);
		        context.SaveChanges();

		        CasesToBeDisposedAtEndOfTests.Add(concernsCase);
	        }
	        catch (Exception)
	        {
		        context.ConcernsCase.Remove(concernsCase);
		        context.SaveChanges();
		        throw;
	        }
        }

        private void AddConcernsRecord(ConcernsRecord concernsRecord)
        {
			using var context = _testFixture.GetContext();

			try
	        {
		        context.ConcernsRecord.Add(concernsRecord);
		        context.SaveChanges();

		        RecordsToBeDisposedAtEndOfTests.Add(concernsRecord);
	        }
	        catch (Exception)
	        {
		        context.ConcernsRecord.Remove(concernsRecord);
		        context.SaveChanges();
		        throw;
	        }
        }
    }
}