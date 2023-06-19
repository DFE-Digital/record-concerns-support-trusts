using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Tests.Fixtures;
using ConcernsCaseWork.API.Tests.Helpers;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Integration
{
	[Collection(ApiTestCollection.ApiTestCollectionName)]
	public class GetActiveCasesByOwnerIntegrationTests
	{
		private readonly Fixture _fixture;
		private readonly HttpClient _client;
		private readonly ApiTestFixture _testFixture;

		public GetActiveCasesByOwnerIntegrationTests(ApiTestFixture apiTestFixture)
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

			var expectedCase = CreateConcernsCase(owner);

			cases.Add(expectedCase);

			await SaveCases(cases);

			var getResponse = await _client.GetAsync($"/v2/concerns-cases/summary/{owner}/active");
			getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

			var wrapper = await getResponse.Content.ReadFromJsonAsync<ApiResponseV2<ActiveCaseSummaryResponse>>();
			var result = wrapper.Data.ToList();

			var actualCase = result.First();
		}

		private ConcernsCase CreateConcernsCase(string owner)
		{
			var result = CreateNonConcernsCase(owner);

			result.ConcernsRecords.Add(DatabaseModelBuilder.BuildConcernsRecord());

			return result;
		}
		private ConcernsCase CreateNonConcernsCase(string owner)
		{
			var result = new ConcernsCase()
			{
				RatingId = (int)ConcernRating.RedPlus,
				StatusId = (int)CaseStatus.Live,
				TrustUkprn = DatabaseModelBuilder.CreateUkPrn(),
				ConcernsRecords = new List<ConcernsRecord>(),
				CreatedAt = _fixture.Create<DateTime>(),
				CreatedBy = owner
			};

			return result;
		}

		private async Task<List<ConcernsCase>> SaveCases(List<ConcernsCase> cases)
		{
			await using var context = _testFixture.GetContext();

			context.ConcernsCase.AddRange(cases);
			await context.SaveChangesAsync();

			return cases;
		}
	}
}
