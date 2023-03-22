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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration;

[Collection(ApiTestCollection.ApiTestCollectionName)]
public class ConcernsIntegrationTests : IDisposable
{
	private readonly Fixture _autoFixture;
	private readonly HttpClient _client;
	private readonly RandomGenerator _randomGenerator;
	private readonly ApiTestFixture _testFixture;

	public ConcernsIntegrationTests(ApiTestFixture fixture)
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
	public async Task CanCreateNewConcernCase()
	{
		ConcernCaseRequest createRequest = Builder<ConcernCaseRequest>.CreateNew()
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


		HttpRequestMessage httpRequestMessage = new()
		{
			Method = HttpMethod.Post, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-cases"), Content = JsonContent.Create(createRequest)
		};

		ConcernsCase caseToBeCreated = ConcernsCaseFactory.Create(createRequest);
		ConcernsCaseResponse expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(caseToBeCreated);

		ApiSingleResponseV2<ConcernsCaseResponse> expected = new(expectedConcernsCaseResponse);

		HttpResponseMessage response;
		try
		{
			response = await _client.SendAsync(httpRequestMessage);
		}
		catch (Exception ex)
		{
			;
			throw;
		}

		response.StatusCode.Should().Be(HttpStatusCode.Created);
		ApiSingleResponseV2<ConcernsCaseResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

		using ConcernsDbContext context = _testFixture.GetContext();

		ConcernsCase createdCase = context.ConcernsCase.FirstOrDefault(c => c.Urn == result.Data.Urn);
		expected.Data.Urn = createdCase.Urn;

		result.Should().BeEquivalentTo(expected);
		createdCase.Description.Should().BeEquivalentTo(createRequest.Description);
	}

	[Fact]
	public async Task CanCreateNewConcernCase_WithNullCompaniesHouseNumber()
	{
		ConcernCaseRequest createRequest = Builder<ConcernCaseRequest>.CreateNew()
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
			.With(c => c.TrustCompaniesHouseNumber = null)
			.Build();


		HttpRequestMessage httpRequestMessage = new()
		{
			Method = HttpMethod.Post, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-cases"), Content = JsonContent.Create(createRequest)
		};

		ConcernsCase caseToBeCreated = ConcernsCaseFactory.Create(createRequest);
		ConcernsCaseResponse expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(caseToBeCreated);

		ApiSingleResponseV2<ConcernsCaseResponse> expected = new(expectedConcernsCaseResponse);

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		ApiSingleResponseV2<ConcernsCaseResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

		using ConcernsDbContext context = _testFixture.GetContext();

		ConcernsCase createdCase = context.ConcernsCase.FirstOrDefault(c => c.Urn == result.Data.Urn);
		expected.Data.Urn = createdCase.Urn;

		result.Should().BeEquivalentTo(expected);
		createdCase.Description.Should().BeEquivalentTo(createRequest.Description);
	}

	[Fact]
	public async Task PostInvalidConcernCaseRequest_Returns_ValidationErrors()
	{
		ConcernCaseRequest request = _autoFixture.Create<ConcernCaseRequest>();
		request.TrustUkprn = new string('a', 13);
		request.Issue = new string('a', 2001);
		request.CaseAim = new string('a', 1001);
		request.CurrentStatus = new string('a', 4001);
		request.DeEscalationPoint = new string('a', 1001);
		request.NextSteps = new string('a', 4001);
		request.CaseHistory = new string('a', 4301);
		request.DirectionOfTravel = new string('a', 101);
		request.CreatedBy = new string('a', 255);
		request.TrustCompaniesHouseNumber = new string('a', 9);

		HttpResponseMessage result = await _client.PostAsync("/v2/concerns-cases", request.ConvertToJson());
		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		string error = await result.Content.ReadAsStringAsync();
		error.Should().Contain("The field TrustUkprn must be a string with a maximum length of 12.");
		error.Should().Contain("The field Issue must be a string with a maximum length of 2000.");
		error.Should().Contain("The field CaseAim must be a string with a maximum length of 1000.");
		error.Should().Contain("The field CurrentStatus must be a string with a maximum length of 4000.");
		error.Should().Contain("The field DeEscalationPoint must be a string with a maximum length of 1000.");
		error.Should().Contain("The field NextSteps must be a string with a maximum length of 4000.");
		error.Should().Contain("The field CaseHistory must be a string with a maximum length of 4300.");
		error.Should().Contain("The field DirectionOfTravel must be a string with a maximum length of 100.");
		error.Should().Contain("The field CreatedBy must be a string with a maximum length of 254.");
		error.Should().Contain("The field TrustCompaniesHouseNumber must be a string with a maximum length of 8.");
	}

	[Fact]
	public async Task CanGetConcernCaseByUrn()
	{
		using ConcernsDbContext context = _testFixture.GetContext();

		SetupConcernsCaseTestData("mockUkprn");
		ConcernsCase concernsCase = context.ConcernsCase.First();

		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/urn/{concernsCase.Urn}") };

		ConcernsCaseResponse expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(concernsCase);

		ApiSingleResponseV2<ConcernsCaseResponse> expected = new(expectedConcernsCaseResponse);

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		ApiSingleResponseV2<ConcernsCaseResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

		result.Should().BeEquivalentTo(expected);
		result.Data.Urn.Should().Be(concernsCase.Urn);
	}

	[Fact]
	public async Task CanGetConcernCaseByTrustUkprn()
	{
		string ukprn = "100008";

		List<ConcernsCase> expectedData = SetupConcernsCaseTestData(ukprn);
		ConcernsCase concernsCase = expectedData.First();

		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}") };

		ConcernsCaseResponse expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(concernsCase);
		PagingResponse expectedPaging = new() { Page = 1, RecordCount = expectedData.Count };

		ApiResponseV2<ConcernsCaseResponse> expected = new(new List<ConcernsCaseResponse> { expectedConcernsCaseResponse }, expectedPaging);

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		ApiResponseV2<ConcernsCaseResponse> result = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

		result.Data.Count(d => d.Urn == concernsCase.Urn).Should().Be(1);
	}

	[Fact]
	public async Task CanGetMultipleConcernCasesByTrustUkprn()
	{
		string ukprn = "100008";

		List<ConcernsCase> concernsCases = SetupConcernsCaseTestData(ukprn, 2);

		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}") };
		PagingResponse expectedPaging = new() { Page = 1, RecordCount = concernsCases.Count };

		List<ConcernsCaseResponse> expectedConcernsCaseResponse = concernsCases.Select(c => ConcernsCaseResponseFactory.Create(c)).ToList();

		ApiResponseV2<ConcernsCaseResponse> expected = new(expectedConcernsCaseResponse, expectedPaging);

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiResponseV2<ConcernsCaseResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Count(d => d.Urn == concernsCases[0].Urn).Should().Be(1);
		content.Data.Count(d => d.Urn == concernsCases[1].Urn).Should().Be(1);
	}

	[Fact]
	public async Task GettingMultipleConcernCasesByTrustUkprn_WhenFewerThanCount_ShouldReturnAllItems()
	{
		string ukprn = "100005";
		int count = 20;

		List<ConcernsCase> concernsCases = SetupConcernsCaseTestData(ukprn, 10);

		HttpRequestMessage httpRequestMessage = new()
		{
			Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}")
		};
		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiResponseV2<ConcernsCaseResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Count().Should().Be(concernsCases.Count);
	}

	[Fact]
	public async Task GettingMultipleConcernCasesByTrustUkprn_WhenGreaterThanCount_ShouldReturnCountNumberOfItems()
	{
		string ukprn = "100008";
		int count = 5;

		SetupConcernsCaseTestData(ukprn, 10);

		HttpRequestMessage httpRequestMessage = new()
		{
			Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}")
		};
		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiResponseV2<ConcernsCaseResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Count().Should().Be(count);
	}

	[Fact]
	public async Task IndexConcernsStatuses_ShouldReturnAllConcernsStatuses()
	{
		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-statuses/") };
		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiResponseV2<ConcernsStatusResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsStatusResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Count().Should().Be(3);
	}

	[Fact]
	public async Task IndexConcernsTypes_ShouldReturnAllConcernsTypes()
	{
		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-types/") };
		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiResponseV2<ConcernsTypeResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsTypeResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Count().Should().Be(9);
	}

	[Fact]
	public async Task UpdateConcernsCase_ShouldReturnTheUpdatedConcernsCase()
	{
		ConcernsCase currentConcernsCase = new()
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
			Territory = Territory.Midlands_And_West__SouthWest,
			StatusId = 2,
			RatingId = 1,
			TrustCompaniesHouseNumber = "12345678"
		};

		AddConcernsCaseToDatabase(currentConcernsCase);

		int urn = currentConcernsCase.Urn;

		ConcernCaseRequest updateRequest = Builder<ConcernCaseRequest>.CreateNew()
			.With(cr => cr.Description = "")
			.With(cr => cr.CrmEnquiry = "")
			.With(cr => cr.ReasonAtReview = "")
			.With(cr => cr.TrustCompaniesHouseNumber = "87654321")
			.With(cr => cr.RatingId = 1).Build();

		ConcernsCase expectedConcernsCase = ConcernsCaseFactory.Create(updateRequest);
		expectedConcernsCase.Urn = urn;
		ConcernsCaseResponse expectedContent = ConcernsCaseResponseFactory.Create(expectedConcernsCase);

		HttpRequestMessage httpRequestMessage = new()
		{
			Method = HttpMethod.Patch, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/{urn}"), Content = JsonContent.Create(updateRequest)
		};
		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiSingleResponseV2<ConcernsCaseResponse> content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Should().BeEquivalentTo(expectedContent);
	}

	[Fact]
	public async Task PatchInvalidConcernCaseRequest_Returns_ValidationErrors()
	{
		ConcernCaseRequest request = _autoFixture.Create<ConcernCaseRequest>();
		request.TrustUkprn = new string('a', 13);
		request.Issue = new string('a', 2001);
		request.CaseAim = new string('a', 1001);
		request.CurrentStatus = new string('a', 4001);
		request.DeEscalationPoint = new string('a', 1001);
		request.NextSteps = new string('a', 4001);
		request.CaseHistory = new string('a', 4301);
		request.DirectionOfTravel = new string('a', 101);
		request.ReasonAtReview = new string('a', 201);
		request.CreatedBy = new string('a', 255);
		request.TrustCompaniesHouseNumber = new string('1', 9);

		HttpResponseMessage result = await _client.PatchAsync("/v2/concerns-cases/1", request.ConvertToJson());
		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		string error = await result.Content.ReadAsStringAsync();
		error.Should().Contain("The field TrustUkprn must be a string with a maximum length of 12.");
		error.Should().Contain("The field Issue must be a string with a maximum length of 2000.");
		error.Should().Contain("The field CaseAim must be a string with a maximum length of 1000.");
		error.Should().Contain("The field CurrentStatus must be a string with a maximum length of 4000.");
		error.Should().Contain("The field DeEscalationPoint must be a string with a maximum length of 1000.");
		error.Should().Contain("The field NextSteps must be a string with a maximum length of 4000.");
		error.Should().Contain("The field CaseHistory must be a string with a maximum length of 4300.");
		error.Should().Contain("The field DirectionOfTravel must be a string with a maximum length of 100.");
		error.Should().Contain("The field ReasonAtReview must be a string with a maximum length of 200.");
		error.Should().Contain("The field CreatedBy must be a string with a maximum length of 254.");
		error.Should().Contain("The field TrustCompaniesHouseNumber must be a string with a maximum length of 8.");
	}

	[Fact]
	public async Task CanCreateNewConcernRecord()
	{
		using ConcernsDbContext context = _testFixture.GetContext();

		ConcernsRating caseRating = context.ConcernsRatings.First();

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
			RatingId = caseRating.Id
		};

		AddConcernsCaseToDatabase(concernsCase);

		ConcernsCase linkedCase = context.ConcernsCase.First();
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
			Method = HttpMethod.Post, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-records"), Content = JsonContent.Create(createRequest)
		};

		ConcernsRecord expectedRecordToBeCreated = ConcernsRecordFactory.Create(createRequest, linkedCase, linkedType, linkedRating, meansOfReferral);
		ConcernsRecordResponse expectedConcernsRecordResponse = ConcernsRecordResponseFactory.Create(expectedRecordToBeCreated);
		ApiSingleResponseV2<ConcernsRecordResponse> expected = new(expectedConcernsRecordResponse);

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		ApiSingleResponseV2<ConcernsRecordResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

		ConcernsRecord createdRecord = context.ConcernsRecord.FirstOrDefault(c => c.Id == result.Data.Id);
		expected.Data.Id = createdRecord.Id;

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

		using ConcernsDbContext context = _testFixture.GetContext();

		ConcernsType concernsType = context.ConcernsTypes.FirstOrDefault(t => t.Id == 3);
		ConcernsRating concernsRating = context.ConcernsRatings.FirstOrDefault(r => r.Id == 1);
		ConcernsMeansOfReferral concernsMeansOfReferral = context.ConcernsMeansOfReferrals.FirstOrDefault(r => r.Id == 1);

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
			Method = HttpMethod.Patch, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"), Content = JsonContent.Create(updateRequest)
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

		using ConcernsDbContext context = _testFixture.GetContext();

		ConcernsType concernsType = context.ConcernsTypes.FirstOrDefault(t => t.Id == 3);
		ConcernsRating concernsRating = context.ConcernsRatings.FirstOrDefault(r => r.Id == 1);

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
			Method = HttpMethod.Patch, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/{currentRecordId}"), Content = JsonContent.Create(updateRequest)
		};
		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiSingleResponseV2<ConcernsRecordResponse> content = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Should().BeEquivalentTo(expectedContent);
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

		using ConcernsDbContext context = _testFixture.GetContext();

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
			Method = HttpMethod.Post, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-records/"), Content = JsonContent.Create(recordCreateRequest1)
		};

		HttpResponseMessage createResponse1 = await _client.SendAsync(httpCreateRequestMessage1);
		ApiSingleResponseV2<ConcernsRecordResponse> content1 = await createResponse1.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsRecordResponse>>();
		createResponse1.StatusCode.Should().Be(HttpStatusCode.Created);

		HttpRequestMessage httpCreateRequestMessage2 = new()
		{
			Method = HttpMethod.Post, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-records/"), Content = JsonContent.Create(recordCreateRequest2)
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
			Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-records/case/urn/{concernsCase.Urn}")
		};

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		ApiResponseV2<ConcernsRecordResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsRecordResponse>>();
		content.Data.Count().Should().Be(2);
		content.Data.Should().BeEquivalentTo(expected);
	}

	[Fact]
	public async Task GetConcernsCaseByOwnerId()
	{
		string ownerId = _randomGenerator.NextString(3, 10);

		List<ConcernsCase> ownedOpenConcernsCases = new();

		foreach (int i in Enumerable.Range(1, 5))
		{
			ConcernsCase ownedOpenConcernsCase = new()
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
				Territory = Territory.North_And_Utc__Utc,
				StatusId = 2,
				RatingId = 1,
				TrustCompaniesHouseNumber = _randomGenerator.NextString(8, 8)
			};

			ConcernsCase otherUsersConcernsCase = new()
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
				Territory = Territory.South_And_South_East__East_Of_England,
				StatusId = 3,
				RatingId = 3,
				TrustCompaniesHouseNumber = _randomGenerator.NextString(8, 8)
			};

			ownedOpenConcernsCases.Add(ownedOpenConcernsCase);
			AddConcernsCaseToDatabase(ownedOpenConcernsCase);
			AddConcernsCaseToDatabase(otherUsersConcernsCase);
		}

		List<ConcernsCaseResponse> expected = ownedOpenConcernsCases.Select(ConcernsCaseResponseFactory.Create).ToList();

		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/owner/{ownerId}?status=2") };

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		ApiResponseV2<ConcernsCaseResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();
		content.Data.Count().Should().Be(5);
		content.Data.Should().BeEquivalentTo(expected);
	}

	private List<ConcernsCase> SetupConcernsCaseTestData(string trustUkprn, int count = 1)
	{
		List<ConcernsCase> listOfCases = new();
		for (int i = 0; i < count; i++)
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
				Territory = Territory.Midlands_And_West__East_Midlands,
				StatusId = 2,
				RatingId = 3,
				TrustCompaniesHouseNumber = _randomGenerator.NextString(8, 8)
			};

			AddConcernsCaseToDatabase(concernsCase);

			listOfCases.Add(concernsCase);
		}

		return listOfCases;
	}

	[Fact]
	public async Task IndexConcernsMeansOfReferral_ShouldReturnAllConcernsMeansOfReferral()
	{
		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri("https://notarealdomain.com/v2/concerns-meansofreferral/") };
		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiResponseV2<ConcernsMeansOfReferralResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsMeansOfReferralResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Count().Should().Be(2);

		content.Data.First().Name.Should().Be("Internal");
		content.Data.First().Description.Should().Be("ESFA activity, TFF or other departmental activity");
		content.Data.First().Id.Should().BeGreaterThan(0);

		content.Data.Last().Name.Should().Be("External");
		content.Data.Last().Description.Should().Be("CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies");
		content.Data.Last().Id.Should().BeGreaterThan(0);
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