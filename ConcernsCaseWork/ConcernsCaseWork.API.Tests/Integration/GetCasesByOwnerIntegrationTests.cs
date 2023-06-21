using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Extensions;
using FluentAssertions;
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
	public class GetCasesByOwnerIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly ApiTestFixture _testFixture;

		public GetCasesByOwnerIntegrationTests(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_testFixture = apiTestFixture;
		}

		[Fact]
		public async Task When_HasActiveCasesWithCaseActions_Returns_CorrectInformation_200()
		{
			var owner = _fixture.Create<string>();
			List<ConcernsCase> cases = new List<ConcernsCase>();

			var expectedCase = CreateCase(owner);

			cases.Add(expectedCase);

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.CreateOpenCaseActions(expectedCase.Id);
			await context.CreateClosedCaseActions(expectedCase.Id);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
			actualCase.CaseUrn.Should().Be(expectedCase.Id);
			actualCase.CreatedAt.Should().Be(expectedCase.CreatedAt);
			actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
			actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
			actualCase.StatusName.Should().Be(CaseStatus.Live.ToString());

			actualCase.ActiveConcerns.Should().HaveCount(1);
			var concern = actualCase.ActiveConcerns.First();
			concern.Name.Should().Be(ConcernType.FinancialDeficit.Description());
			concern.Rating.Id.Should().Be((int)ConcernRating.AmberGreen);
			concern.Rating.Name.Should().Be(ConcernRating.AmberGreen.Description());

			CaseSummaryAssert.AssertCaseActions(actualCase);
		}

		[Fact]
		public async Task When_HasActiveCases_Returns_AllCases_200()
		{
			var owner = _fixture.Create<string>();
			var differentOwner = _fixture.Create<string>();
			List<ConcernsCase> cases = new List<ConcernsCase>();
			List<ConcernsCase> casesDifferentOwner = new List<ConcernsCase>();
			List<ConcernsCase> closedCases = new List<ConcernsCase>();

			for (var idx = 0; idx < 5; idx++)
			{
				cases.Add(CreateCase(owner));
				casesDifferentOwner.Add(CreateCase(differentOwner));
				closedCases.Add(DatabaseModelBuilder.CloseCase(CreateCase(owner)));
			}

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.SaveCases(casesDifferentOwner);
			await context.SaveCases(closedCases);

			var expectedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			wrapper.Paging.Should().BeNull();

			result.Should().HaveCount(5);

			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
		}

		[Fact]
		public async Task When_HasNoActiveCases_Returns_Empty_200()
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/NotExist/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data;

			result.Should().HaveCount(0);
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationOnlyNext_Returns_200()
		{
			var owner = _fixture.Create<string>();

			var cases = await BulkCreateActiveCases(owner);

			var expectedCases = cases.Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active?page=1&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeFalse();
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationNextAndPrevious_Returns_200()
		{
			var owner = _fixture.Create<string>();

			var cases = await BulkCreateActiveCases(owner);

			var expectedCases = cases.Skip(4).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active?page=3&count=2");
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
			var owner = _fixture.Create<string>();

			var cases = await BulkCreateActiveCases(owner);

			var expectedCases = cases.Skip(8).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active?page=5&count=2");
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
		public async Task When_HasClosedCasesWithCaseActions_Returns_CorrectInformation_200()
		{
			var owner = _fixture.Create<string>();
			List<ConcernsCase> cases = new List<ConcernsCase>();

			var expectedCase = DatabaseModelBuilder.CloseCase(CreateCase(owner));

			cases.Add(expectedCase);

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.CreateClosedCaseActions(expectedCase.Id);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
			actualCase.CaseUrn.Should().Be(expectedCase.Id);
			actualCase.CreatedAt.Should().Be(expectedCase.CreatedAt);
			actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
			actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
			actualCase.StatusName.Should().Be(CaseStatus.Close.ToString());

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
			var owner = _fixture.Create<string>();
			var differentOwner = _fixture.Create<string>();
			List<ConcernsCase> cases = new List<ConcernsCase>();
			List<ConcernsCase> casesDifferentOwner = new List<ConcernsCase>();
			List<ConcernsCase> openCases = new List<ConcernsCase>();

			for (var idx = 0; idx < 5; idx++)
			{
				var @case = DatabaseModelBuilder.CloseCase(CreateCase(owner));
				var caseDifferentOwner = DatabaseModelBuilder.CloseCase(CreateCase(differentOwner));
				var openCase = CreateCase(owner);

				cases.Add(@case);
				casesDifferentOwner.Add(caseDifferentOwner);
				openCases.Add(openCase);
			}

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.SaveCases(casesDifferentOwner);
			await context.SaveCases(openCases);

			var expectedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			wrapper.Paging.Should().BeNull();

			result.Should().HaveCount(5);

			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
		}

		[Fact]
		public async Task When_HasNoClosedCases_Returns_Empty_200()
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/NotExist/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data;

			result.Should().HaveCount(0);
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationOnlyNext_Returns_200()
		{
			var owner = _fixture.Create<string>();

			var cases = await BulkCreateClosedCases(owner);

			var expectedCases = cases.Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/closed?page=1&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeFalse();
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationNextAndPrevious_Returns_200()
		{
			var owner = _fixture.Create<string>();

			var cases = await BulkCreateClosedCases(owner);

			var expectedCases = cases.Skip(4).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/closed?page=3&count=2");
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
			var owner = _fixture.Create<string>();

			var cases = await BulkCreateClosedCases(owner);

			var expectedCases = cases.Skip(8).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/closed?page=5&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ClosedCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeFalse();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		private ConcernsCase CreateCase(string owner)
		{
			var result = DatabaseModelBuilder.BuildCase();
			result.CreatedBy = owner;

			result.ConcernsRecords.Add(DatabaseModelBuilder.BuildConcernsRecord());

			return result;
		}

		private async Task<List<ConcernsCase>> BulkCreateActiveCases(string owner)
		{
			using var context = _testFixture.GetContext();

			List<ConcernsCase> cases = new List<ConcernsCase>();

			for (var idx = 0; idx < 10; idx++)
			{
				cases.Add(CreateCase(owner));
			}

			await context.SaveCases(cases);

			var orderedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			return orderedCases;
		}


		private async Task<List<ConcernsCase>> BulkCreateClosedCases(string owner)
		{
			using var context = _testFixture.GetContext();

			List<ConcernsCase> cases = new List<ConcernsCase>();

			for (var idx = 0; idx < 10; idx++)
			{
				cases.Add(DatabaseModelBuilder.CloseCase(CreateCase(owner)));
			}

			await context.SaveCases(cases);

			var orderedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			return orderedCases;
		}
	}
}
