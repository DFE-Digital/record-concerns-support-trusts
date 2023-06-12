using AutoFixture;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
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

		[Fact]

		public async Task When_HasActiveCasesWithCaseActions_Returns_CorrectInformation_200()
		{
			var ukPrn = CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();

			var expectedCase = CreateConcernsCase(ukPrn);

			cases.Add(expectedCase);

			await SaveCases(cases);
			await CreateOpenCaseActions(expectedCase.Id);
			await CreateClosedCaseActions(expectedCase.Id);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
			actualCase.CaseUrn.Should().Be(expectedCase.Id);
			actualCase.CreatedAt.Should().Be(expectedCase.CreatedAt);
			actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
			actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
			actualCase.StatusName.Should().Be("Live");
			actualCase.Rating.Id.Should().Be(1);
			actualCase.Rating.Name.Should().Be("Red-Plus");

			actualCase.Decisions.Should().HaveCount(1);
			var decision = actualCase.Decisions.First();
			decision.Name.Should().Be("Decision: No Decision Types");

			actualCase.NoticesToImprove.Should().HaveCount(1);
			var nti = actualCase.NoticesToImprove.First();
			nti.Name.Should().Be("Action: Notice To Improve");

			actualCase.NtisUnderConsideration.Should().HaveCount(1);
			var ntiUnderConsideration = actualCase.NtisUnderConsideration.First();
			ntiUnderConsideration.Name.Should().Be("Action: NTI under consideration");

			actualCase.NtiWarningLetters.Should().HaveCount(1);
			var ntiWarningLetter = actualCase.NtiWarningLetters.First();
			ntiWarningLetter.Name.Should().Be("Action: NTI warning letter");

			actualCase.FinancialPlanCases.Should().HaveCount(1);
			var financialPlan = actualCase.FinancialPlanCases.First();
			financialPlan.Name.Should().Be("Action: Financial plan");

			actualCase.SrmaCases.Should().HaveCount(1);
			var srma = actualCase.SrmaCases.First();
			srma.Name.Should().Be("Action: School Resource Management Adviser");

			actualCase.TrustFinancialForecasts.Should().HaveCount(1);
			var tff = actualCase.TrustFinancialForecasts.First();
			tff.Name.Should().Be("Action: Trust Financial Forecast (TFF)");
		}

		[Fact]
		public async Task When_HasActiveCases_Returns_AllCases_200()
		{
			var ukPrn = CreateUkPrn();
			var differentUkPrn = CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();
			List<ConcernsCase> casesDifferentUkPrn = new List<ConcernsCase>();
			List<ConcernsCase> closedCases = new List<ConcernsCase>();

			for (var idx = 0; idx < 5; idx++)
			{
				cases.Add(CreateNonConcernsCase(ukPrn));
				cases.Add(CreateConcernsCase(ukPrn));
				casesDifferentUkPrn.Add(CreateNonConcernsCase(differentUkPrn));
				closedCases.Add(CloseCase(CreateNonConcernsCase(ukPrn)));
			}

			await SaveCases(cases);
			await SaveCases(casesDifferentUkPrn);
			await SaveCases(closedCases);

			var expectedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>> ();
			var result = wrapper.Data.ToList();

			wrapper.Paging.Should().BeNull();

			result.Should().HaveCount(10);

			AssertCaseList(result, expectedCases);
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
			var ukPrn = CreateUkPrn();

			var cases = await BulkCreateActiveCases(ukPrn);

			var expectedCases = cases.Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active?page=1&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			AssertCaseList(result, expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeFalse();
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationNextAndPrevious_Returns_200()
		{
			var ukPrn = CreateUkPrn();

			var cases = await BulkCreateActiveCases(ukPrn);

			var expectedCases = cases.Skip(4).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active?page=3&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			AssertCaseList(result, expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasActiveCases_PaginationPreviousOnly_Returns_200()
		{
			var ukPrn = CreateUkPrn();

			var cases = await BulkCreateActiveCases(ukPrn);

			var expectedCases = cases.Skip(8).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/active?page=5&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			AssertCaseList(result, expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeFalse();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasClosedCasesWithCaseActions_Returns_CorrectInformation_200()
		{
			var ukPrn = CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();

			var expectedCase = CloseCase(CreateConcernsCase(ukPrn));

			cases.Add(expectedCase);

			await SaveCases(cases);
			await CreateOpenCaseActions(expectedCase.Id);
			await CreateClosedCaseActions(expectedCase.Id);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
			actualCase.CaseUrn.Should().Be(expectedCase.Id);
			actualCase.CreatedAt.Should().Be(expectedCase.CreatedAt);
			actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
			actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
			actualCase.StatusName.Should().Be("Close");
			actualCase.Rating.Id.Should().Be(1);
			actualCase.Rating.Name.Should().Be("Red-Plus");

			actualCase.Decisions.Should().HaveCount(1);
			var decision = actualCase.Decisions.First();
			decision.Name.Should().Be("Decision: No Decision Types");

			actualCase.NoticesToImprove.Should().HaveCount(1);
			var nti = actualCase.NoticesToImprove.First();
			nti.Name.Should().Be("Action: Notice To Improve");

			actualCase.NtisUnderConsideration.Should().HaveCount(1);
			var ntiUnderConsideration = actualCase.NtisUnderConsideration.First();
			ntiUnderConsideration.Name.Should().Be("Action: NTI under consideration");

			actualCase.NtiWarningLetters.Should().HaveCount(1);
			var ntiWarningLetter = actualCase.NtiWarningLetters.First();
			ntiWarningLetter.Name.Should().Be("Action: NTI warning letter");

			actualCase.FinancialPlanCases.Should().HaveCount(1);
			var financialPlan = actualCase.FinancialPlanCases.First();
			financialPlan.Name.Should().Be("Action: Financial plan");

			actualCase.SrmaCases.Should().HaveCount(1);
			var srma = actualCase.SrmaCases.First();
			srma.Name.Should().Be("Action: School Resource Management Adviser");

			actualCase.TrustFinancialForecasts.Should().HaveCount(1);
			var tff = actualCase.TrustFinancialForecasts.First();
			tff.Name.Should().Be("Action: Trust Financial Forecast (TFF)");
		}

		[Fact]
		public async Task When_HasClosedCases_Returns_AllCases_200()
		{
			var ukPrn = CreateUkPrn();
			var differentUkPrn = CreateUkPrn();
			List<ConcernsCase> cases = new List<ConcernsCase>();
			List<ConcernsCase> casesDifferentUkPrn = new List<ConcernsCase>();
			List<ConcernsCase> openCases = new List<ConcernsCase>();

			for (var idx = 0; idx < 5; idx++)
			{
				var nonConcernsCase = CreateNonConcernsCase(ukPrn);
				var concernsCase = CreateConcernsCase(ukPrn);
				var nonTrustCase = CreateNonConcernsCase(differentUkPrn);

				cases.Add(CloseCase(nonConcernsCase));
				cases.Add(CloseCase(concernsCase));
				casesDifferentUkPrn.Add(CloseCase(nonTrustCase));
				openCases.Add(CreateNonConcernsCase(ukPrn));
			}

			await SaveCases(cases);
			await SaveCases(casesDifferentUkPrn);
			await SaveCases(openCases);

			var expectedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			wrapper.Paging.Should().BeNull();

			result.Should().HaveCount(10);

			AssertCaseList(result, expectedCases);
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationOnlyNext_Returns_200()
		{
			var ukPrn = CreateUkPrn();

			var cases = await BulkCreateClosedCases(ukPrn);

			var expectedCases = cases.Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed?page=1&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			AssertCaseList(result, expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeFalse();
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationNextAndPrevious_Returns_200()
		{
			var ukPrn = CreateUkPrn();

			var cases = await BulkCreateClosedCases(ukPrn);

			var expectedCases = cases.Skip(4).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed?page=3&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			AssertCaseList(result, expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeTrue();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasClosedCases_PaginationPreviousOnly_Returns_200()
		{
			var ukPrn = CreateUkPrn();

			var cases = await BulkCreateClosedCases(ukPrn);

			var expectedCases = cases.Skip(8).Take(2).ToList();

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/{ukPrn}/closed?page=5&count=2");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			result.Should().HaveCount(2);
			AssertCaseList(result, expectedCases);
			wrapper.Paging.RecordCount.Should().Be(10);
			wrapper.Paging.HasNext.Should().BeFalse();
			wrapper.Paging.HasPrevious.Should().BeTrue();
		}

		[Fact]
		public async Task When_HasNoClosedCases_Returns_Empty_200()
		{
			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/bytrust/NoExist/closed");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data;

			result.Should().HaveCount(0);
		}

		private string CreateUkPrn()
		{
			return _fixture.Create<string>().Substring(0, 7);
		}

		public Decision CreateDecision()
		{
			var result = new Decision()
			{
			};

			return result;
		}

		private ConcernsCase CreateNonConcernsCase(string ukPrn)
		{
			var result = new ConcernsCase()
			{
				RatingId = 1,
				StatusId = 1,
				TrustUkprn = ukPrn,
				ConcernsRecords = new List<ConcernsRecord>(),
				CreatedAt = _fixture.Create<DateTime>(),
				CreatedBy = _fixture.Create<string>()
		};

			return result;
		} 

		private ConcernsCase CreateConcernsCase(string ukPrn)
		{
			var result = CreateNonConcernsCase(ukPrn);

			var concern = new ConcernsRecord()
			{
				RatingId = 1,
				StatusId = 1,
				TypeId = 3
			};

			result.ConcernsRecords.Add(concern);

			return result;
		}

		private ConcernsCase CloseCase(ConcernsCase concernsCase)
		{
			concernsCase.ClosedAt = _fixture.Create<DateTime>();
			concernsCase.StatusId = 3;

			return concernsCase;
		}

		private async Task CreateOpenCaseActions(int caseId)
		{
			using var context = _testFixture.GetContext();

			context.Decisions.Add(DatabaseModelBuilder.BuildDecision(caseId));
			context.NoticesToImprove.Add(DatabaseModelBuilder.BuildNoticeToImprove(caseId));
			context.NTIUnderConsiderations.Add(DatabaseModelBuilder.BuildNTIUnderConsideration(caseId));
			context.NTIWarningLetters.Add(DatabaseModelBuilder.BuildNTIWarningLetter(caseId));
			context.SRMACases.Add(DatabaseModelBuilder.BuildSrma(caseId));
			context.TrustFinancialForecasts.Add(DatabaseModelBuilder.BuildTrustFinancialForecast(caseId));
			context.FinancialPlanCases.Add(DatabaseModelBuilder.BuildFinancialPlan(caseId));

			await context.SaveChangesAsync();
		}

		private async Task CreateClosedCaseActions(int caseId)
		{
			using var context = _testFixture.GetContext();

			var decision = DatabaseModelBuilder.BuildDecision(caseId);
			decision.ClosedAt = _fixture.Create<DateTime>();
			context.Decisions.Add(decision);

			var noticeToImprove = DatabaseModelBuilder.BuildNoticeToImprove(caseId);
			noticeToImprove.ClosedAt = _fixture.Create<DateTime>();
			context.NoticesToImprove.Add(noticeToImprove);

			var ntiUnderConsideration = DatabaseModelBuilder.BuildNTIUnderConsideration(caseId);
			ntiUnderConsideration.ClosedAt = _fixture.Create<DateTime>();
			context.NTIUnderConsiderations.Add(ntiUnderConsideration);

			var ntiWarningLetter = DatabaseModelBuilder.BuildNTIWarningLetter(caseId);
			ntiWarningLetter.ClosedAt = _fixture.Create<DateTime>();
			context.NTIWarningLetters.Add(ntiWarningLetter);

			var srma = DatabaseModelBuilder.BuildSrma(caseId);
			srma.ClosedAt = _fixture.Create<DateTime>();
			context.SRMACases.Add(srma);

			var tff = DatabaseModelBuilder.BuildTrustFinancialForecast(caseId);
			tff.ClosedAt = _fixture.Create<DateTime>();
			context.TrustFinancialForecasts.Add(tff);

			var financialPlan = DatabaseModelBuilder.BuildFinancialPlan(caseId);
			financialPlan.ClosedAt = _fixture.Create<DateTime>();
			context.FinancialPlanCases.Add(financialPlan);

			await context.SaveChangesAsync();
		}

		private async Task<List<ConcernsCase>> BulkCreateActiveCases(string ukPrn)
		{
			List<ConcernsCase> cases = new List<ConcernsCase>();

			for (var idx = 0; idx < 10; idx++)
			{
				cases.Add(CreateConcernsCase(ukPrn));
			}

			await SaveCases(cases);

			var orderedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			return orderedCases;
		}

		private async Task<List<ConcernsCase>> BulkCreateClosedCases(string ukPrn)
		{
			List<ConcernsCase> cases = new List<ConcernsCase>();

			for (var idx = 0; idx < 10; idx++)
			{
				cases.Add(CloseCase(CreateConcernsCase(ukPrn)));
			}

			await SaveCases(cases);

			var orderedCases = cases.OrderByDescending(c => c.CreatedAt).ToList();

			return orderedCases;
		}

		private async Task<List<ConcernsCase>> SaveCases(List<ConcernsCase> cases)
		{
			await using var context = _testFixture.GetContext();

			context.ConcernsCase.AddRange(cases);
			await context.SaveChangesAsync();

			return cases;
		}

		private void AssertCaseList(List<ActiveCaseSummaryResponse> actualCases, List<ConcernsCase> expectedCases)
		{
			for (var idx = 0; idx < expectedCases.Count; idx++)
			{
				var expectedCase = expectedCases[idx];
				var actualCase = actualCases[idx];

				actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
				actualCase.CaseUrn.Should().Be(expectedCase.Id);
			}
		}
	}
}
