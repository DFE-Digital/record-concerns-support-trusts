using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Utils.Extensions;
using FluentAssertions;
using MoreLinq;
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
	public class GetCasesByTrustIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly ApiTestFixture _testFixture;

		public GetCasesByTrustIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_testFixture = apiTestFixture;
		}

		[Theory]
		[InlineData(Division.SFSO, "National Operations")]
		[InlineData(Division.RegionsGroup, "London")]
		public async Task When_HasActiveCasesWithCaseActions_Returns_CorrectInformation_200(Division division, string expectedArea)
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();

			var expectedCase = CreateConcernsCase(ukPrn);
			expectedCase.DivisionId = division;
			expectedCase.Territory = Territory.National_Operations;
			expectedCase.RegionId = Region.London;

			var closedConcern = DatabaseModelBuilder.BuildConcernsRecord();
			closedConcern.TypeId = (int)ConcernType.Irregularity;
			closedConcern.StatusId = (int)CaseStatus.Close;
			expectedCase.ConcernsRecords.Add(closedConcern);

			cases.Add(expectedCase);

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.CreateOpenCaseActions(expectedCase.Id);
			await context.CreateClosedCaseActions(expectedCase.Id);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
			actualCase.CaseUrn.Should().Be(expectedCase.Id);
			actualCase.CreatedAt.Should().Be(expectedCase.CreatedAt);
			actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
			actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
			actualCase.StatusName.Should().Be(CaseStatus.Live.ToString());
			actualCase.Division.Should().Be(division);
			actualCase.Area.Should().Be(expectedArea);

			actualCase.ActiveConcerns.Should().HaveCount(1);
			var concern = actualCase.ActiveConcerns.First();
			concern.Name.Should().Be(ConcernType.FinancialDeficit.Description());
			concern.Rating.Id.Should().Be((int)ConcernRating.AmberGreen);
			concern.Rating.Name.Should().Be(ConcernRating.AmberGreen.Description());

			actualCase.Rating.Id.Should().Be((int)ConcernRating.RedPlus);
			actualCase.Rating.Name.Should().Be(ConcernRating.RedPlus.Description());

			CaseSummaryAssert.AssertCaseActions(actualCase);
		}

		[Fact]
		public async Task When_HasActiveCases_Returns_AllCases_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();
			var differentUkPrn = DatabaseModelBuilder.CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();
			List<ConcernsCase> casesDifferentUkPrn = new List<ConcernsCase>();
			List<ConcernsCase> closedCases = new List<ConcernsCase>();

			for (var idx = 0; idx < 5; idx++)
			{
				cases.Add(CreateNonConcernsCase(ukPrn));
				cases.Add(CreateConcernsCase(ukPrn));
				casesDifferentUkPrn.Add(CreateNonConcernsCase(differentUkPrn));
				closedCases.Add(DatabaseModelBuilder.CloseCase(CreateNonConcernsCase(ukPrn)));
			}

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.SaveCases(casesDifferentUkPrn);
			await context.SaveCases(closedCases);

			var expectedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>> ();
			var result = wrapper.Data.ToList();

			wrapper.Paging.Should().BeNull();

			result.Should().HaveCount(10);

			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
		}

		[Fact]
		public async Task When_HasNoActiveCases_Returns_Empty_200()
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/NoExist/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data;

			result.Should().HaveCount(0);
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationOnlyNext_Returns_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			var cases = await BulkCreateActiveCases(ukPrn);

			var expectedCases = cases.Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active?page=1&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.TotalPages.Should().Be(5);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeFalse();
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationNextAndPrevious_Returns_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			var cases = await BulkCreateActiveCases(ukPrn);

			var expectedCases = cases.Skip(4).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active?page=3&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationPreviousOnly_Returns_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			var cases = await BulkCreateActiveCases(ukPrn);

			var expectedCases = cases.Skip(8).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active?page=5&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeFalse();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasActiveCases_RequestPageGreaterThanAvailable_Returns_Empty_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			await BulkCreateActiveCases(ukPrn);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active?page=6&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(0);
		}

		[Theory]
		[InlineData(Division.SFSO, "National Operations")]
		[InlineData(Division.RegionsGroup, "London")]
		public async Task When_HasClosedCasesWithCaseActions_Returns_CorrectInformation_200(Division division, string expectedArea)
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();

			var expectedCase = DatabaseModelBuilder.CloseCase(CreateConcernsCase(ukPrn));
			expectedCase.DivisionId = division;
			expectedCase.Territory = Territory.National_Operations;
			expectedCase.RegionId = Region.London;

			cases.Add(expectedCase);

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.CreateClosedCaseActions(expectedCase.Id);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
			actualCase.CaseUrn.Should().Be(expectedCase.Id);
			actualCase.CreatedAt.Should().Be(expectedCase.CreatedAt);
			actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
			actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
			actualCase.StatusName.Should().Be(CaseStatus.Close.ToString());
			actualCase.Division.Should().Be(division);
			actualCase.Area.Should().Be(expectedArea);

			actualCase.ClosedConcerns.Should().HaveCount(1);
			var concern = actualCase.ClosedConcerns.First();
			concern.Name.Should().Be(ConcernType.FinancialDeficit.Description());
			concern.Rating.Id.Should().Be((int)ConcernRating.AmberGreen);
			concern.Rating.Name.Should().Be(ConcernRating.AmberGreen.Description());

			CaseSummaryAssert.AssertCaseActions(actualCase);
		}

		[Fact]
		public async Task When_HasClosedCases_Returns_AllCases_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();
			var differentUkPrn = DatabaseModelBuilder.CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();
			List<ConcernsCase> casesDifferentUkPrn = new List<ConcernsCase>();
			List<ConcernsCase> openCases = new List<ConcernsCase>();

			for (var idx = 0; idx < 5; idx++)
			{
				var nonConcernsCase = CreateNonConcernsCase(ukPrn);
				var concernsCase = CreateConcernsCase(ukPrn);
				var nonTrustCase = CreateNonConcernsCase(differentUkPrn);

				cases.Add(DatabaseModelBuilder.CloseCase(nonConcernsCase));
				cases.Add(DatabaseModelBuilder.CloseCase(concernsCase));
				casesDifferentUkPrn.Add(DatabaseModelBuilder.CloseCase(nonTrustCase));
				openCases.Add(CreateNonConcernsCase(ukPrn));
			}

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.SaveCases(casesDifferentUkPrn);
			await context.SaveCases(openCases);

			var expectedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			wrapper.Paging.Should().BeNull();

			result.Should().HaveCount(10);

			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationOnlyNext_Returns_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			var cases = await BulkCreateClosedCases(ukPrn);

			var expectedCases = cases.Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed?page=1&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.TotalPages.Should().Be(5);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeFalse();
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationNextAndPrevious_Returns_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			var cases = await BulkCreateClosedCases(ukPrn);

			var expectedCases = cases.Skip(4).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed?page=3&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationPreviousOnly_Returns_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			var cases = await BulkCreateClosedCases(ukPrn);

			var expectedCases = cases.Skip(8).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed?page=5&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeFalse();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasClosedCases_RequestPageGreaterThanAvailable_Returns_Empty_200()
		{
			var ukPrn = DatabaseModelBuilder.CreateUkPrn();

			await BulkCreateClosedCases(ukPrn);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed?page=6&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(0);
		}

		[Fact]
		public async Task When_HasNoClosedCases_Returns_Empty_200()
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/NoExist/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data;

			result.Should().HaveCount(0);
		}

		private ConcernsCase CreateNonConcernsCase(string ukPrn)
		{
			var result = DatabaseModelBuilder.BuildCase();
			result.TrustUkprn = ukPrn;

			return result;
		} 

		private ConcernsCase CreateConcernsCase(string ukPrn)
		{
			var result = CreateNonConcernsCase(ukPrn);

			result.ConcernsRecords.Add(DatabaseModelBuilder.BuildConcernsRecord());

			return result;
		}

		private async Task<List<ConcernsCase>> BulkCreateActiveCases(string ukPrn)
		{
			using var context = _testFixture.GetContext();

			List<ConcernsCase> cases = new List<ConcernsCase>();

			for (var idx = 0; idx < 10; idx++)
			{
				cases.Add(CreateConcernsCase(ukPrn));
			}

			await context.SaveCases(cases);

			var orderedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			return orderedCases;
		}

		private async Task<List<ConcernsCase>> BulkCreateClosedCases(string ukPrn)
		{
			using var context = _testFixture.GetContext();

			List<ConcernsCase> cases = new List<ConcernsCase>();

			for (var idx = 0; idx < 10; idx++)
			{
				cases.Add(DatabaseModelBuilder.CloseCase(CreateConcernsCase(ukPrn)));
			}

			await context.SaveCases(cases);

			var orderedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			return orderedCases;
		}
	}
}
