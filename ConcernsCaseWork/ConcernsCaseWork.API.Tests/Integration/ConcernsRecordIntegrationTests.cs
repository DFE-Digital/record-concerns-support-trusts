using AutoFixture;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
	public class ConcernsRecordIntegrationTests: IDisposable
	{

		private readonly Fixture _autoFixture;
		private readonly HttpClient _client;
		private readonly RandomGenerator _randomGenerator;
		private readonly ApiTestFixture _testFixture;

		public ConcernsRecordIntegrationTests(ApiTestFixture fixture)
		{
			_autoFixture = new Fixture();
			_randomGenerator = new RandomGenerator();
			_testFixture = fixture;
			_client = fixture.Client;
		}


		private List<ConcernsCase> CasesToBeDisposedAtEndOfTests { get; } = new();
		private List<ConcernsRecord> RecordsToBeDisposedAtEndOfTests { get; } = new();

		public void Dispose()
		{
			using ConcernsDbContext context = _testFixture.GetContext();

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


		[Fact]
		public async Task CanCreateNewConcernRecord()
		{
			await using ConcernsDbContext context = _testFixture.GetContext();

			ConcernsRating caseRating = context.ConcernsRatings.First();

			ConcernsCase concernsCase = BuildConcernsCase(caseRating.Id);

			AddConcernsCaseToDatabase(concernsCase);

			ConcernsCase linkedCase = concernsCase;
			ConcernsType linkedType = context.ConcernsTypes.First();
			ConcernsRating linkedRating = context.ConcernsRatings.First();
			ConcernsMeansOfReferral meansOfReferral = context.ConcernsMeansOfReferrals.First();

			ConcernsRecordRequest createRequest = Builder<ConcernsRecordRequest>.CreateNew()
				.With(c => c.CaseUrn = linkedCase.Urn)
				.With(c => c.TypeId = linkedType.Id)
				.With(c => c.RatingId = linkedRating.Id)
				.With(c => c.MeansOfReferralId = meansOfReferral.Id)
				.Build();

			HttpRequestMessage httpRequestMessage = new()
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://notarealdomain.com/v2/concerns-records"),
				Content = JsonContent.Create(createRequest)
			};

			ConcernsRecord expectedRecordToBeCreated = ConcernsRecordFactory.Create(createRequest, linkedCase, linkedType, linkedRating, meansOfReferral);
			ConcernsRecordResponse expectedConcernsRecordResponse = ConcernsRecordResponseFactory.Create(expectedRecordToBeCreated);
			ApiSingleResponseV2<ConcernsRecordResponse> expected = new(expectedConcernsRecordResponse);

			HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
			response.StatusCode.Should().Be(HttpStatusCode.Created);
			ApiSingleResponseV2<ConcernsRecordResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

			ConcernsRecord createdRecord = context.ConcernsRecord.FirstOrDefault(c => c.Id == result.Data.Id);
			createdRecord.Should().NotBeNull();
			expected.Data.Id = createdRecord!.Id;

			result.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public async Task PostInvalidConcernsRecordRequest_Returns_ValidationErrors()
		{
			ConcernsRecordRequest request = _autoFixture.Create<ConcernsRecordRequest>();
			request.Name = new string('a', 301);
			request.Description = new string('a', 301);
			request.Reason = new string('a', 301);

			HttpResponseMessage result = await _client.PostAsync("/v2/concerns-records", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			string error = await result.Content.ReadAsStringAsync();
			error.Should().Contain("The field Name must be a string with a maximum length of 300.");
			error.Should().Contain("The field Description must be a string with a maximum length of 300.");
			error.Should().Contain("The field Reason must be a string with a maximum length of 300.");
		}

		[Fact]
		public async Task UpdateConcernsRecord_ShouldReturnTheUpdatedConcernsRecord()
		{
			ConcernsCase currentConcernsCase = new()
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
				Territory = Territory.Midlands_And_West__West_Midlands,
				StatusId = 2,
				RatingId = 3
			};

			await using ConcernsDbContext context = _testFixture.GetContext();

			ConcernsType concernsType = context.ConcernsTypes.First(t => t.Id == 3);
			ConcernsRating concernsRating = context.ConcernsRatings.First(r => r.Id == 1);
			ConcernsMeansOfReferral concernsMeansOfReferral = context.ConcernsMeansOfReferrals.First(r => r.Id == 1);

			AddConcernsCaseToDatabase(currentConcernsCase);

			ConcernsRecord currentConcernsRecord = new()
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
			int currentRecordId = currentConcernsRecord.Id;

			ConcernsRecordRequest updateRequest = Builder<ConcernsRecordRequest>.CreateNew()
				.With(r => r.CaseUrn = currentConcernsCase.Urn)
				.With(r => r.TypeId = concernsType.Id)
				.With(r => r.RatingId = concernsRating.Id)
				.With(r => r.MeansOfReferralId = concernsMeansOfReferral.Id).Build();

			ConcernsRecord expectedConcernsRecord =
				ConcernsRecordFactory.Update(currentConcernsRecord, updateRequest, currentConcernsCase, concernsType, concernsRating, concernsMeansOfReferral);
			expectedConcernsRecord.Id = currentRecordId;
			ConcernsRecordResponse expectedContent = ConcernsRecordResponseFactory.Create(expectedConcernsRecord);

			HttpRequestMessage httpRequestMessage = new()
			{
				Method = HttpMethod.Patch,
				RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"),
				Content = JsonContent.Create(updateRequest)
			};
			HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
			ApiSingleResponseV2<ConcernsRecordResponse> content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			content.Data.Should().BeEquivalentTo(expectedContent);
		}

		[Fact]
		public async Task PatchInvalidConcernsRecordRequest_Returns_ValidationErrors()
		{
			ConcernsRecordRequest request = _autoFixture.Create<ConcernsRecordRequest>();
			request.Name = new string('a', 301);
			request.Description = new string('a', 301);
			request.Reason = new string('a', 301);

			HttpResponseMessage result = await _client.PatchAsync("/v2/concerns-records/1", request.ConvertToJson());
			result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

			string error = await result.Content.ReadAsStringAsync();
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
			ConcernsCase concernsCase = new()
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
				Territory = Territory.North_And_Utc__Yorkshire_And_Humber,
				StatusId = 2,
				RatingId = 3
			};

			AddConcernsCaseToDatabase(concernsCase);

			await using ConcernsDbContext context = _testFixture.GetContext();

			ConcernsType concernsType = context.ConcernsTypes.First(t => t.Id == 3);
			ConcernsRating concernsRating = context.ConcernsRatings.First(r => r.Id == 1);

			ConcernsMeansOfReferral currentMeansOfReferral = hasCurrentMeansOfReferral
				? context.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 1)
				: null;

			ConcernsMeansOfReferral updateMeansOfReferral = isAddingMeansOfReferral
				? context.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 2)
				: null;

			ConcernsRecord currentConcernsRecord = new()
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
			int currentRecordId = currentConcernsRecord.Id;

			ConcernsRecordRequest updateRequest = Builder<ConcernsRecordRequest>.CreateNew()
				.With(r => r.CaseUrn = concernsCase.Urn)
				.With(r => r.ClosedAt = null)
				.With(r => r.TypeId = concernsType.Id)
				.With(r => r.RatingId = concernsRating.Id)
				.With(r => r.MeansOfReferralId = updateMeansOfReferral?.Id)
				.Build();

			ConcernsRecord expectedConcernsRecord = ConcernsRecordFactory.Update(currentConcernsRecord, updateRequest, concernsCase, concernsType, concernsRating,
				updateMeansOfReferral ?? currentMeansOfReferral);
			expectedConcernsRecord.Id = currentRecordId;
			ConcernsRecordResponse expectedContent = ConcernsRecordResponseFactory.Create(expectedConcernsRecord);

			HttpRequestMessage httpRequestMessage = new()
			{
				Method = HttpMethod.Patch,
				RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"),
				Content = JsonContent.Create(updateRequest)
			};
			HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
			ApiSingleResponseV2<ConcernsRecordResponse> content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			content.Data.Should().BeEquivalentTo(expectedContent);
		}


		public ConcernsCase BuildConcernsCase(Int32 caseRatingID)
		{
			ConcernsCase concernsCase = new()
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
				Territory = Territory.North_And_Utc__Yorkshire_And_Humber,
				StatusId = 2,
				RatingId = caseRatingID
			};

			return concernsCase;
		}

		[Fact]
		public async Task DeleteMissingConcernsRecord_Returns_NotFound()
		{
			var currentRecordId = 987654321;

			HttpRequestMessage httpRequestMessage = new()
			{
				Method = HttpMethod.Delete,
				RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"),
			};
			HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task DeleteConcernsRecord_Returns_NoConcernsRecordForCase()
		{
			await using ConcernsDbContext context = _testFixture.GetContext();

			ConcernsRating caseRating = context.ConcernsRatings.First();

			ConcernsCase concernsCase = BuildConcernsCase(caseRating.Id);

			AddConcernsCaseToDatabase(concernsCase);

			ConcernsType linkedType = context.ConcernsTypes.First();
			ConcernsRating linkedRating = context.ConcernsRatings.First();
			ConcernsMeansOfReferral meansOfReferral = context.ConcernsMeansOfReferrals.First();

			ConcernsRecordRequest createConcernsRecordContent = Builder<ConcernsRecordRequest>.CreateNew()
				.With(c => c.CaseUrn = concernsCase.Urn)
				.With(c => c.TypeId = linkedType.Id)
				.With(c => c.RatingId = linkedRating.Id)
				.With(c => c.MeansOfReferralId = meansOfReferral.Id)
				.Build();

			HttpRequestMessage createConcernsRecordHttpRequest = new()
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://notarealdomain.com/v2/concerns-records"),
				Content = JsonContent.Create(createConcernsRecordContent)
			};

			HttpResponseMessage createdResponse = await _client.SendAsync(createConcernsRecordHttpRequest);
			createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);

			ApiSingleResponseV2<ConcernsRecordResponse> createdResult = await createdResponse.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
			var currentRecordId = createdResult.Data.Id;
			var urn = concernsCase.Urn;

			HttpRequestMessage httpRequestMessage = new()
			{
				Method = HttpMethod.Delete,
				RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"),
			};
			HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

			response.StatusCode.Should().Be(HttpStatusCode.NoContent);


			HttpRequestMessage listConcernsforCaseResponse = new()
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/case/urn/{urn}"),
			};

			HttpResponseMessage listResponse = await _client.SendAsync(listConcernsforCaseResponse);
			ApiResponseV2<ConcernsRecordResponse> concernsforCaseList = await listResponse.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsRecordResponse>>();

			listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
			concernsforCaseList.Data.Count().Should().Be(0);
		}


		[Fact]
		public async Task GetConcernsRecordsByConcernsCaseUid()
		{
			ConcernsCase concernsCase = new()
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
				Territory = Territory.North_And_Utc__North_West,
				StatusId = 2,
				RatingId = 4
			};

			AddConcernsCaseToDatabase(concernsCase);

			await using ConcernsDbContext context = _testFixture.GetContext();

			ConcernsRating concernsRating = context.ConcernsRatings.FirstOrDefault();
			ConcernsType concernsType = context.ConcernsTypes.FirstOrDefault();
			ConcernsMeansOfReferral concernsMeansOfReferral = context.ConcernsMeansOfReferrals.FirstOrDefault();

			ConcernsRecordRequest recordCreateRequest1 = new()
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

			ConcernsRecordRequest recordCreateRequest2 = new()
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

			HttpRequestMessage httpCreateRequestMessage1 = new()
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://notarealdomain.com/v2/concerns-records/"),
				Content = JsonContent.Create(recordCreateRequest1)
			};

			HttpResponseMessage createResponse1 = await _client.SendAsync(httpCreateRequestMessage1);
			ApiSingleResponseV2<ConcernsRecordResponse> content1 = await createResponse1.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
			createResponse1.StatusCode.Should().Be(HttpStatusCode.Created);

			HttpRequestMessage httpCreateRequestMessage2 = new()
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://notarealdomain.com/v2/concerns-records/"),
				Content = JsonContent.Create(recordCreateRequest2)
			};

			HttpResponseMessage createResponse2 = await _client.SendAsync(httpCreateRequestMessage2);
			ApiSingleResponseV2<ConcernsRecordResponse> content2 = await createResponse2.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
			createResponse2.StatusCode.Should().Be(HttpStatusCode.Created);

			ConcernsRecord createdRecord1 = ConcernsRecordFactory
				.Create(recordCreateRequest1, concernsCase, concernsType, concernsRating, concernsMeansOfReferral);
			createdRecord1.Id = content1.Data.Id;
			ConcernsRecord createdRecord2 = ConcernsRecordFactory
				.Create(recordCreateRequest2, concernsCase, concernsType, concernsRating, concernsMeansOfReferral);
			createdRecord2.Id = content2.Data.Id;
			List<ConcernsRecord> createdRecords = new() { createdRecord1, createdRecord2 };
			List<ConcernsRecordResponse> expected = createdRecords
				.Select(ConcernsRecordResponseFactory.Create).ToList();

			HttpRequestMessage httpRequestMessage = new()
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/case/urn/{concernsCase.Urn}")
			};

			HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			ApiResponseV2<ConcernsRecordResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsRecordResponse>>();
			content.Data.Count().Should().Be(2);
			content.Data.Should().BeEquivalentTo(expected);
		}


		private void AddConcernsCaseToDatabase(ConcernsCase concernsCase)
		{
			using ConcernsDbContext context = _testFixture.GetContext();

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
			using ConcernsDbContext context = _testFixture.GetContext();

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
