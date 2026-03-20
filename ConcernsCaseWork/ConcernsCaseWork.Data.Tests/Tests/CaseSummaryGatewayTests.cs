using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Tests.DbGateways;
using ConcernsCaseWork.Data.Tests.TestData;
using FluentAssertions;

namespace ConcernsCaseWork.Data.Tests.Tests;

[TestFixture]
public class CaseSummaryGatewayTests : DatabaseTestFixture
{
	private readonly TestCaseDbGateway _caseGateway = new();
	private readonly TestDataFactory _factory = new();

	[Test]
	public async Task GetCaseSummariesByFilter_WhenLondonSelected_ReturnsSfsoAndRegionsGroupCases()
	{
		var statusId = _caseGateway.GetDefaultCaseStatus().Id;
		var ratingId = _caseGateway.GetDefaultCaseRating().Id;

		var regionsGroupCase = _factory.BuildOpenCase(statusId, ratingId);
		regionsGroupCase.TrustUkprn = "111111111111";
		regionsGroupCase.CreatedBy = "rg-owner-gateway-test";
		regionsGroupCase.DivisionId = Division.RegionsGroup;
		regionsGroupCase.RegionId = Region.London;
		regionsGroupCase.Territory = null;

		var sfsoCase = _factory.BuildOpenCase(statusId, ratingId);
		sfsoCase.TrustUkprn = "222222222222";
		sfsoCase.CreatedBy = "sfso-owner-gateway-test";
		sfsoCase.DivisionId = Division.SFSO;
		sfsoCase.Territory = Territory.South_And_South_East__London;
		sfsoCase.RegionId = null;

		regionsGroupCase = _caseGateway.AddCase(regionsGroupCase);
		sfsoCase = _caseGateway.AddCase(sfsoCase);

		using var context = CreateContext();
		var sut = new CaseSummaryGateway(context);

		var (cases, _) = await sut.GetCaseSummariesByFilter(new GetCaseSummariesByFilterParameters
		{
			Regions = [Region.London]
		});

		var ours = cases.Where(c => c.CaseUrn == regionsGroupCase.Urn || c.CaseUrn == sfsoCase.Urn).ToList();
		ours.Should().HaveCount(2);
		ours.Select(c => c.Division).Should().BeEquivalentTo(new[] { Division.RegionsGroup, Division.SFSO });
	}

	[Test]
	public async Task GetCaseSummariesByFilter_WhenLondonSelected_ExcludesSfsoCaseInDifferentTerritory()
	{
		var statusId = _caseGateway.GetDefaultCaseStatus().Id;
		var ratingId = _caseGateway.GetDefaultCaseRating().Id;

		var londonRg = _factory.BuildOpenCase(statusId, ratingId);
		londonRg.TrustUkprn = "333333333333";
		londonRg.CreatedBy = "rg-london-test";
		londonRg.DivisionId = Division.RegionsGroup;
		londonRg.RegionId = Region.London;
		londonRg.Territory = null;

		var northEastSfso = _factory.BuildOpenCase(statusId, ratingId);
		northEastSfso.TrustUkprn = "444444444444";
		northEastSfso.CreatedBy = "sfso-ne-test";
		northEastSfso.DivisionId = Division.SFSO;
		northEastSfso.Territory = Territory.North_And_Utc__North_East;
		northEastSfso.RegionId = null;

		londonRg = _caseGateway.AddCase(londonRg);
		northEastSfso = _caseGateway.AddCase(northEastSfso);

		using var context = CreateContext();
		var sut = new CaseSummaryGateway(context);

		var (cases, _) = await sut.GetCaseSummariesByFilter(new GetCaseSummariesByFilterParameters
		{
			Regions = [Region.London]
		});

		cases.Should().Contain(c => c.CaseUrn == londonRg.Urn);
		cases.Should().NotContain(c => c.CaseUrn == northEastSfso.Urn);
	}
}
