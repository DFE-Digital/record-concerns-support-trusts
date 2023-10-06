using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
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

namespace ConcernsCaseWork.API.Tests.Integration
{
	public class CaseWorkIntegrationTestsBase
	{
		protected readonly Fixture _fixture;
		protected readonly HttpClient _client;
		protected readonly ApiTestFixture _testFixture;
		protected const int _casesToCreate = 10;

		public CaseWorkIntegrationTestsBase(ApiTestFixture apiTestFixture)
		{
			_client = apiTestFixture.Client;
			_fixture = new();
			_testFixture = apiTestFixture;
		}

		protected async Task HasActiveCasesWithCaseActions_Returns_CorrectInformation_200(string createdBy, string url)
		{
			List<ConcernsCase> cases = new List<ConcernsCase>();

			var expectedCase = CreateCase(createdBy);

			cases.Add(expectedCase);

			using var context = _testFixture.GetContext();

			await context.SaveCases(cases);
			await context.CreateOpenCaseActions(expectedCase.Id);
			await context.CreateClosedCaseActions(expectedCase.Id);

			var getResponse = await _client.GetAsync(url);
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
			actualCase.CaseUrn.Should().Be(expectedCase.Id);
			actualCase.CreatedAt.Should().Be(expectedCase.CreatedAt);
			actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
			actualCase.CaseLastUpdatedAt.Should().Be(expectedCase.CaseLastUpdatedAt);

			actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
			actualCase.StatusName.Should().Be(CaseStatus.Live.ToString());

			actualCase.ActiveConcerns.Should().HaveCount(1);
			var concern = actualCase.ActiveConcerns.First();
			concern.Name.Should().Be(ConcernType.FinancialDeficit.Description());
			concern.Rating.Id.Should().Be((int)ConcernRating.AmberGreen);
			concern.Rating.Name.Should().Be(ConcernRating.AmberGreen.Description());

			CaseSummaryAssert.AssertCaseActions(actualCase);
		}

		protected async Task<List<ConcernsCase>> CreateCasesWithDifferentOwners(string owner, string differentOwner)
		{

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

			return cases;
		}

		public async Task HasActiveCases_PaginationOnlyNext_Returns_200(string owner,string createdBy, string url)
		{

			var cases = await BulkCreateActiveCases(createdBy);

			var expectedCases = cases.Take(2).ToList();

			var getResponse = await _client.GetAsync(url);
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			CaseSummaryAssert.AssertCaseList(result.Cast<CaseSummaryResponse>().ToList(), expectedCases);
			wrapper.Paging.RecordCount.Should().Be(_casesToCreate);
			wrapper.Paging.TotalPages.Should().Be(5);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeFalse();
		}

		protected async Task<List<ConcernsCase>> BulkCreateActiveCases(string owner)
		{
			using var context = _testFixture.GetContext();

			List<ConcernsCase> cases = new List<ConcernsCase>();

			for (var idx = 0; idx < _casesToCreate; idx++)
			{
				cases.Add(CreateCase(owner));
			}

			await context.SaveCases(cases);

			var orderedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			return orderedCases;
		}

		protected ConcernsCase CreateCase(string owner)
		{
			var result = DatabaseModelBuilder.BuildCase();
			result.CreatedBy = owner;

			result.ConcernsRecords.Add(DatabaseModelBuilder.BuildConcernsRecord());

			return result;
		}
	}
}
