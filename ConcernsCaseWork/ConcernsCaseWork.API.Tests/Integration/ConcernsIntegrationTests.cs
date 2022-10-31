using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoFixture;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
    [Collection("Database")]
    public class ConcernsIntegrationTests : IClassFixture<ConcernsDataApiFactory>, IDisposable
    {
        
        private readonly HttpClient _client;
        private readonly ConcernsDbContext _dbContext;
        private readonly Fixture _fixture;
        private readonly RandomGenerator _randomGenerator;

        public ConcernsIntegrationTests(ConcernsDataApiFactory fixture)
        {
            _client = fixture.CreateClient();
            _client.DefaultRequestHeaders.Add("ApiKey", "testing-api-key");
            _dbContext = fixture.Services.GetRequiredService<ConcernsDbContext>();
            _fixture = new Fixture();
            _randomGenerator = new RandomGenerator();
        }

        [Fact]
        public async Task CanCreateNewConcernCase()
        {
            var createRequest = Builder<ConcernCaseRequest>.CreateNew()
                .With(c => c.CreatedBy = "12345")
                .With(c => c.Description = "Description for case")
                .With(c => c.CrmEnquiry = "5678")
                .With(c => c.TrustUkprn = "100223")
                .With(c => c.ReasonAtReview = "We have concerns")
                .With(c => c.DeEscalation = new DateTime(2022,04,01))
                .With(c => c.Issue = "Here is the issue")
                .With(c => c.CurrentStatus = "Case status")
                .With(c => c.CaseAim = "Here is the aim")
                .With(c => c.DeEscalationPoint = "Point of de-escalation")
                .With(c => c.NextSteps = "Here are the next steps")
                .With(c => c.DirectionOfTravel = "Up")
                .With(c => c.StatusUrn = 1)
                .With(c => c.RatingUrn = 123)
                .Build();
            


            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                },
                Content =  JsonContent.Create(createRequest)
            };

            var caseToBeCreated = ConcernsCaseFactory.Create(createRequest);
            var expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(caseToBeCreated);
            
            var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(expectedConcernsCaseResponse);
            
            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(201);
            var result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();
            
            var createdCase = _dbContext.ConcernsCase.FirstOrDefault(c => c.Urn == result.Data.Urn);
            expected.Data.Urn = createdCase.Urn;
            
            result.Should().BeEquivalentTo(expected);
            createdCase.Description.Should().BeEquivalentTo(createRequest.Description);
        }

        [Fact]
        public async Task CanGetConcernCaseByUrn()
        {
            SetupConcernsCaseTestData("mockUkprn");
            var concernsCase = _dbContext.ConcernsCase.First();
            
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases/urn/{concernsCase.Urn}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            
            var expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(concernsCase);
            
            var expected = new ApiSingleResponseV2<ConcernsCaseResponse>(expectedConcernsCaseResponse);
            
            var response = await _client.SendAsync(httpRequestMessage);
            
            response.StatusCode.Should().Be(200);
            var result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();
            
            result.Should().BeEquivalentTo(expected);
            result.Data.Urn.Should().Be(concernsCase.Urn);
        }
        
        [Fact]
        public async Task CanGetConcernCaseByTrustUkprn()
        {
            var ukprn = "100008";
            SetupConcernsCaseTestData(ukprn);
            var concernsCase = _dbContext.ConcernsCase.First();

            var expectedData = new List<ConcernsCase> {concernsCase};
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases/ukprn/{ukprn}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            
            var expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(concernsCase);
            var expectedPaging = new PagingResponse {Page = 1, RecordCount = expectedData.Count};
            
            var expected = new ApiResponseV2<ConcernsCaseResponse>(new List<ConcernsCaseResponse>{expectedConcernsCaseResponse}, expectedPaging);
            
            var response = await _client.SendAsync(httpRequestMessage);
            
            response.StatusCode.Should().Be(200);
            var result = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();
            result.Should().BeEquivalentTo(expected);
            result.Data.First().Urn.Should().Be(concernsCase.Urn);
        }

        [Fact]
        public async Task CanGetMultipleConcernCasesByTrustUkprn()
        {
            var ukprn = "100008";
            SetupConcernsCaseTestData(ukprn, 2);
            var concernsCases = _dbContext.ConcernsCase;
            var expectedData = concernsCases.ToList();
            
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases/ukprn/{ukprn}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            var expectedPaging = new PagingResponse {Page = 1, RecordCount = expectedData.Count};

            var expectedConcernsCaseResponse = concernsCases.Select(c => ConcernsCaseResponseFactory.Create(c)).ToList();
            
            var expected = new ApiResponseV2<ConcernsCaseResponse>(expectedConcernsCaseResponse, expectedPaging);
            
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Should().BeEquivalentTo(expected);
            content.Data.Count().Should().Be(2);
        }

        [Fact]
        public async Task GettingMultipleConcernCasesByTrustUkprn_WhenFewerThanCount_ShouldReturnAllItems()
        {
            var ukprn = "100008";
            var count = 20;
            
            SetupConcernsCaseTestData(ukprn, 10);
            var concernsCases = _dbContext.ConcernsCase;
            var expectedData = concernsCases.ToList();
            
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Data.Count().Should().Be(10);
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
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Data.Count().Should().Be(count);
        }

        [Fact]
        public async Task IndexConcernsStatuses_ShouldReturnAllConcernsStatuses()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-statuses/"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsStatusResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Data.Count().Should().Be(3);
            
        }
        
        [Fact]
        public async Task IndexConcernsTypes_ShouldReturnAllConcernsTypes()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-types/"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsTypeResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Data.Count().Should().Be(13);
            
        }
        
        [Fact]
        public async Task UpdateConcernsCase_ShouldReturnTheUpdatedConcernsCase()
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
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                StatusUrn = 2,
                RatingUrn =  1
            };
            
            var currentConcernsCase =  _dbContext.ConcernsCase.Add(concernsCase);
            _dbContext.SaveChanges();
            var urn = currentConcernsCase.Entity.Urn;

            var updateRequest = Builder<ConcernCaseRequest>.CreateNew()
                .With(cr => cr.RatingUrn = 123).Build();

            var expectedConcernsCase = ConcernsCaseFactory.Create(updateRequest);
            expectedConcernsCase.Urn = urn;
            var expectedContent = ConcernsCaseResponseFactory.Create(expectedConcernsCase);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases/{urn}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                },
                Content =  JsonContent.Create(updateRequest)
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Data.Should().BeEquivalentTo(expectedContent);

        }

        [Fact]
        public async Task CanCreateNewConcernRecord()
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
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                StatusUrn = 2,
            };
            
            _dbContext.ConcernsCase.Add(concernsCase);
            _dbContext.SaveChanges();

            var linkedCase = _dbContext.ConcernsCase.First();
            var linkedType = _dbContext.ConcernsTypes.First();
            var linkedRating = _dbContext.ConcernsRatings.First();

            var createRequest = Builder<ConcernsRecordRequest>.CreateNew()
                .With(c => c.CaseUrn = linkedCase.Urn)
                .With(c => c.TypeUrn = linkedType.Urn)
                .With(c => c.RatingUrn = linkedRating.Urn)
                .Build();
            
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-records"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                },
                Content =  JsonContent.Create(createRequest)
            };
            
            var expectedRecordToBeCreated = ConcernsRecordFactory.Create(createRequest, linkedCase, linkedType, linkedRating, null);
            var expectedConcernsRecordResponse = ConcernsRecordResponseFactory.Create(expectedRecordToBeCreated);
            var expected = new ApiSingleResponseV2<ConcernsRecordResponse>(expectedConcernsRecordResponse);
            
            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(201);
            var result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
            
            var createdRecord = _dbContext.ConcernsRecord.FirstOrDefault(c => c.Urn == result.Data.Urn);
            expected.Data.Urn = createdRecord.Urn;
            
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public async Task UpdateConcernsRecord_ShouldReturnTheUpdatedConcernsRecord()
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
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                StatusUrn = 2
            };

            var concernsType = _dbContext.ConcernsTypes.FirstOrDefault(t => t.Id == 1);
            var concernsRating = _dbContext.ConcernsRatings.FirstOrDefault(r => r.Id == 1);
            var concernsMeansOfReferral = _dbContext.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 1);
            
            var currentConcernsCase =  _dbContext.ConcernsCase.Add(concernsCase);
            _dbContext.SaveChanges();

            var concernsRecord = new ConcernsRecord
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                Name = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                Reason = _randomGenerator.NextString(3, 10),
                StatusUrn = 1,
                ConcernsCase = currentConcernsCase.Entity,
                ConcernsType = concernsType,
                ConcernsRating = concernsRating,
                ConcernsMeansOfReferral = concernsMeansOfReferral
            };
            
            var currentConcernsRecord =  _dbContext.ConcernsRecord.Add(concernsRecord);
            _dbContext.SaveChanges();
            var currentRecordUrn = currentConcernsRecord.Entity.Urn;

            var updateRequest = Builder<ConcernsRecordRequest>.CreateNew()
                .With(r => r.CaseUrn = concernsCase.Urn)
                .With(r => r.TypeUrn = concernsType.Urn)
                .With(r => r.RatingUrn = concernsRating.Urn)
                .With(r => r.MeansOfReferralUrn = concernsMeansOfReferral.Urn).Build();

            var expectedConcernsRecord = ConcernsRecordFactory.Create(updateRequest, concernsCase, concernsType, concernsRating, concernsMeansOfReferral);
            expectedConcernsRecord.Urn = currentRecordUrn;
            var expectedContent = ConcernsRecordResponseFactory.Create(expectedConcernsRecord);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-records/{currentRecordUrn}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                },
                Content =  JsonContent.Create(updateRequest)
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Data.Should().BeEquivalentTo(expectedContent);
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
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                StatusUrn = 2
            };

            var currentConcernsCase =  _dbContext.ConcernsCase.Add(concernsCase);
            _dbContext.SaveChanges();
            
            var concernsType = _dbContext.ConcernsTypes.FirstOrDefault(t => t.Id == 1);
            var concernsRating = _dbContext.ConcernsRatings.FirstOrDefault(r => r.Id == 1);
            
            var currentMeansOfReferral = hasCurrentMeansOfReferral
                ? _dbContext.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 1)
                : null;
            
            var updateMeansOfReferral = isAddingMeansOfReferral
                ? _dbContext.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 2)
                : null;
            
            var concernsRecord = new ConcernsRecord
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                Name = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                Reason = _randomGenerator.NextString(3, 10),
                StatusUrn = 1,
                ConcernsCase = currentConcernsCase.Entity,
                ConcernsType = concernsType,
                ConcernsRating = concernsRating,
                ConcernsMeansOfReferral = currentMeansOfReferral
            };
            
            var currentConcernsRecord =  _dbContext.ConcernsRecord.Add(concernsRecord);
            _dbContext.SaveChanges();
            var currentRecordUrn = currentConcernsRecord.Entity.Urn;

            var updateRequest = Builder<ConcernsRecordRequest>.CreateNew()
                .With(r => r.CaseUrn = concernsCase.Urn)
                .With(r => r.TypeUrn = concernsType.Urn)
                .With(r => r.RatingUrn = concernsRating.Urn)
                .With(r => r.MeansOfReferralUrn = updateMeansOfReferral?.Urn)
                .Build();
            
            var expectedConcernsRecord = ConcernsRecordFactory.Create(updateRequest, concernsCase, concernsType, concernsRating, updateMeansOfReferral ?? currentMeansOfReferral);
            expectedConcernsRecord.Urn = currentRecordUrn;
            var expectedContent = ConcernsRecordResponseFactory.Create(expectedConcernsRecord);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-records/{currentRecordUrn}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                },
                Content =  JsonContent.Create(updateRequest)
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
            
            response.StatusCode.Should().Be(200);
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
                DirectionOfTravel = _randomGenerator.NextString(3, 10),
                StatusUrn = 2
            };
            
            var currentConcernsCase =  _dbContext.ConcernsCase.Add(concernsCase).Entity;
            _dbContext.SaveChanges();

            var concernsRating = _dbContext.ConcernsRatings.FirstOrDefault();
            var concernsType = _dbContext.ConcernsTypes.FirstOrDefault();
            var concernsMeansOfReferral = _dbContext.ConcernsMeansOfReferrals.FirstOrDefault();

            var recordCreateRequest1 = new ConcernsRecordRequest
            {
                CreatedAt = _randomGenerator.DateTime(),
                UpdatedAt = _randomGenerator.DateTime(),
                ReviewAt = _randomGenerator.DateTime(),
                ClosedAt = _randomGenerator.DateTime(),
                Name = _randomGenerator.NextString(3, 10),
                Description = _randomGenerator.NextString(3, 10),
                Reason = _randomGenerator.NextString(3, 10),
                CaseUrn = currentConcernsCase.Urn,
                TypeUrn = concernsType!.Urn,
                RatingUrn = concernsRating!.Urn,
                StatusUrn = 1,
                MeansOfReferralUrn = concernsMeansOfReferral!.Urn
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
                CaseUrn = currentConcernsCase.Urn,
                TypeUrn = concernsType.Urn,
                RatingUrn = concernsRating.Urn,
                StatusUrn = 1,
                MeansOfReferralUrn = concernsMeansOfReferral.Urn
            };

            var httpCreateRequestMessage1 = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-records/"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                },
                Content =  JsonContent.Create(recordCreateRequest1)
            };
            
            var createResponse1 = await _client.SendAsync(httpCreateRequestMessage1);
            var content1 = await createResponse1.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
            createResponse1.StatusCode.Should().Be(201);
            
            var httpCreateRequestMessage2 = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-records/"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                },
                Content =  JsonContent.Create(recordCreateRequest2)
            };
            
            var createResponse2 = await _client.SendAsync(httpCreateRequestMessage2);
            var content2 = await createResponse2.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
            createResponse2.StatusCode.Should().Be(201);
            
            var createdRecord1 = ConcernsRecordFactory
                .Create(recordCreateRequest1, currentConcernsCase, concernsType, concernsRating, concernsMeansOfReferral);
            createdRecord1.Urn = content1.Data.Urn;
            var createdRecord2 = ConcernsRecordFactory
                .Create(recordCreateRequest2, currentConcernsCase, concernsType, concernsRating, concernsMeansOfReferral);
            createdRecord2.Urn = content2.Data.Urn;
            var createdRecords = new List<ConcernsRecord> {createdRecord1, createdRecord2};
            var expected = createdRecords
                .Select(ConcernsRecordResponseFactory.Create).ToList();
            
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-records/case/urn/{currentConcernsCase.Urn}"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            
            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(200);
            
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsRecordResponse>>();
            content.Data.Count().Should().Be(2);
            content.Data.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetConcernsCaseByOwnerId()
        {
            var ownerId = "7583";

            var concernsCases = new List<ConcernsCase>();
            
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
                    DirectionOfTravel = _randomGenerator.NextString(3, 10),
                    StatusUrn = 2,
                };
                var currentCase = _dbContext.ConcernsCase.Add(concernsCase);
                _dbContext.SaveChanges();
                
                concernsCases.Add(currentCase.Entity);
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
                    DirectionOfTravel = _randomGenerator.NextString(3, 10),
                    StatusUrn = 3,
                };
                _dbContext.ConcernsCase.Add(concernsCase);
                _dbContext.SaveChanges();
            }

            var expected = concernsCases.Select(ConcernsCaseResponseFactory.Create).ToList();

            
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-cases/owner/{ownerId}?status=2"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            
            var response = await _client.SendAsync(httpRequestMessage);
            response.StatusCode.Should().Be(200);
            
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();
            content.Data.Count().Should().Be(5);
            content.Data.Should().BeEquivalentTo(expected);
        }

        private void SetupConcernsCaseTestData(string trustUkprn, int count = 1)
        {
            
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
                    StatusUrn = 2,
                };

                _dbContext.ConcernsCase.Add(concernsCase);
                _dbContext.SaveChanges();
            }
        }
        
        [Fact]
        public async Task IndexConcernsMeansOfReferral_ShouldReturnAllConcernsMeansOfReferral()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://trams-api.com/v2/concerns-meansofreferral/"),
                Headers =
                {
                    {"ApiKey", "testing-api-key"}
                }
            };
            var response = await _client.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsMeansOfReferralResponse>>();
            
            response.StatusCode.Should().Be(200);
            content.Data.Count().Should().Be(2);
            						
            content.Data.First().Name.Should().Be("Internal");
            content.Data.First().Description.Should().Be("ESFA activity, TFFT or other departmental activity");
            content.Data.First().Urn.Should().BeGreaterThan(1);
			
            content.Data.Last().Name.Should().Be("External");
            content.Data.Last().Description.Should().Be("CIU casework, whistleblowing, self reported, RSCs or other government bodies");
            content.Data.Last().Urn.Should().BeGreaterThan(1);
        }
        
        public void Dispose()
        {
            _dbContext.ConcernsCase.RemoveRange(_dbContext.ConcernsCase);
            _dbContext.ConcernsRecord.RemoveRange(_dbContext.ConcernsRecord);
            _dbContext.SaveChanges();
        }
    }
}