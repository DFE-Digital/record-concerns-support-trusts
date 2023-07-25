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

namespace ConcernsCaseWork.API.Tests.Integration;

[Collection(ApiTestCollection.ApiTestCollectionName)]
public class ConcernsCaseIntegrationTests : IDisposable
{
	private readonly Fixture _autoFixture;
	private readonly HttpClient _client;
	private readonly RandomGenerator _randomGenerator;
	private readonly ApiTestFixture _testFixture;

	public ConcernsCaseIntegrationTests(ApiTestFixture fixture)
	{
		_autoFixture = new Fixture();
		_randomGenerator = new RandomGenerator();
		_testFixture = fixture;
		_client = fixture.Client;
	}

	private List<ConcernsCase> CasesToBeDisposedAtEndOfTests { get; } = new();

	public void Dispose()
	{
		using ConcernsDbContext context = _testFixture.GetContext();

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

		ConcernsCase caseToBeCreated = ConcernsCaseFactory.Create(createRequest);
		ConcernsCaseResponse expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(caseToBeCreated);

		ApiSingleResponseV2<ConcernsCaseResponse> expected = new(expectedConcernsCaseResponse);

		// call API
		var response = await _client.SendAsync(httpRequestMessage);

		response.StatusCode.Should().Be(HttpStatusCode.Created);
		ApiSingleResponseV2<ConcernsCaseResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

		await using ConcernsDbContext context = _testFixture.GetContext();

		ConcernsCase createdCase = context.ConcernsCase.FirstOrDefault(c => c.Urn == result.Data.Urn);

		createdCase.Should().NotBeNull();
		expected.Data.Urn = createdCase!.Urn;

		result.Should().BeEquivalentTo(expected);
		createdCase.Description.Should().BeEquivalentTo(createRequest.Description);
		createdCase.CaseLastUpdatedAt.Should().Be(createRequest.CreatedAt);
	}

	[Fact]
	public async Task CanCreateNewConcernCase_WithNullCompaniesHouseNumber()
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
			.With(c => c.TrustCompaniesHouseNumber = null)
			.Build();


		HttpRequestMessage httpRequestMessage = new()
		{
			Method = HttpMethod.Post,
			RequestUri = new Uri("https://notarealdomain.com/v2/concerns-cases"),
			Content = JsonContent.Create(createRequest)
		};

		ConcernsCase caseToBeCreated = ConcernsCaseFactory.Create(createRequest);
		ConcernsCaseResponse expectedConcernsCaseResponse = ConcernsCaseResponseFactory.Create(caseToBeCreated);

		ApiSingleResponseV2<ConcernsCaseResponse> expected = new(expectedConcernsCaseResponse);

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		ApiSingleResponseV2<ConcernsCaseResponse> result = await response.Content.ReadFromJsonAsync<ApiSingleResponseV2<ConcernsCaseResponse>>();

		await using ConcernsDbContext context = _testFixture.GetContext();

		ConcernsCase createdCase = context.ConcernsCase.FirstOrDefault(c => c.Urn == result.Data.Urn);
		createdCase.Should().NotBeNull();
		expected.Data.Urn = createdCase!.Urn;

		result.Should().BeEquivalentTo(expected);
		createdCase.Description.Should().BeEquivalentTo(createRequest.Description);
		createdCase.CaseLastUpdatedAt.Should().Be(createRequest.CreatedAt);
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
		await using ConcernsDbContext context = _testFixture.GetContext();

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
		result.Data.Should().BeEquivalentTo(expected.Data);
	}

	[Fact]
	public async Task CanGetMultipleConcernCasesByTrustUkprn()
	{
		string ukprn = "100008";

		List<ConcernsCase> concernsCases = SetupConcernsCaseTestData(ukprn, 2);

		HttpRequestMessage httpRequestMessage = new() { Method = HttpMethod.Get, RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}") };
		PagingResponse expectedPaging = new() { Page = 1, RecordCount = concernsCases.Count };

		List<ConcernsCaseResponse> expectedConcernsCaseResponse = concernsCases.Select(ConcernsCaseResponseFactory.Create).ToList();

		ApiResponseV2<ConcernsCaseResponse> expected = new(expectedConcernsCaseResponse, expectedPaging);

		HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
		ApiResponseV2<ConcernsCaseResponse> content = await response.Content.ReadFromJsonAsync<ApiResponseV2<ConcernsCaseResponse>>();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		content.Data.Count(d => d.Urn == concernsCases[0].Urn).Should().Be(1);
		content.Data.Count(d => d.Urn == concernsCases[1].Urn).Should().Be(1);
		content.Data.Should().BeEquivalentTo(expected.Data);
	}

	[Fact]
	public async Task GettingMultipleConcernCasesByTrustUkprn_WhenFewerThanCount_ShouldReturnAllItems()
	{
		string ukprn = "100005";
		int count = 20;

		List<ConcernsCase> concernsCases = SetupConcernsCaseTestData(ukprn, 10);

		HttpRequestMessage httpRequestMessage = new()
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}")
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
			Method = HttpMethod.Get,
			RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/ukprn/{ukprn}/?count={count}")
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
			Method = HttpMethod.Patch,
			RequestUri = new Uri($"https://notarealdomain.com/v2/concerns-cases/{urn}"),
			Content = JsonContent.Create(updateRequest)
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
	public async Task GetConcernsCaseByOwnerId()
	{
		string ownerId = _randomGenerator.NextString(3, 10);

		List<ConcernsCase> ownedOpenConcernsCases = new();

		foreach (var _ in Enumerable.Range(1, 5))
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
}