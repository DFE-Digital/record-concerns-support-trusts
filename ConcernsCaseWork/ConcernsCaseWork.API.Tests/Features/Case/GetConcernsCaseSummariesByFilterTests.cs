using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Utils.Extensions;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Features.Case
{
    public class GetConcernsCaseSummariesByFilterTests
	{
		[Fact]
		public async Task Execute_WhenGatewayReturnsSfsoAndRegionsGroupForSameRegion_ReturnsBoth()
		{
			var mockGateway = new Mock<ICaseSummaryGateway>();
			var parameters = new GetCaseSummariesByFilterParameters { Regions = [Region.London] };

			var rating = new ConcernsRating
			{
				Name = "Red",
				CreatedAt = DateTime.UtcNow.AddDays(-1),
				UpdatedAt = DateTime.UtcNow,
				Id = 1
			};

			var regionsGroupVm = new ActiveCaseSummaryVm
			{
				CaseUrn = 101,
				Rating = rating,
				TeamLedBy = "RgLeader",
				Division = Division.RegionsGroup,
				Region = Region.London,
				Territory = null,
				StatusName = "Live",
				TrustUkPrn = "111111111111",
				CreatedBy = "rg@test.gov.uk",
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				ActiveConcerns = Array.Empty<CaseSummaryVm.Concern>()
			};

			var sfsoVm = new ActiveCaseSummaryVm
			{
				CaseUrn = 102,
				Rating = rating,
				TeamLedBy = "SfsoLeader",
				Division = Division.SFSO,
				Region = null,
				Territory = Territory.South_And_South_East__London,
				StatusName = "Live",
				TrustUkPrn = "222222222222",
				CreatedBy = "sfso@test.gov.uk",
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				ActiveConcerns = Array.Empty<CaseSummaryVm.Concern>()
			};

			var caseSummaries = new List<ActiveCaseSummaryVm> { regionsGroupVm, sfsoVm };

			mockGateway
				.Setup(g => g.GetCaseSummariesByFilter(It.Is<GetCaseSummariesByFilterParameters>(p =>
					p.Regions != null && p.Regions.Length == 1 && p.Regions[0] == Region.London)))
				.ReturnsAsync((caseSummaries, 2));

			var sut = new GetConcernsCaseSummariesByFilter(mockGateway.Object);

			var (result, count) = await sut.Execute(parameters);

			Assert.Equal(2, count);
			Assert.Equal(2, result.Count);

			var rg = result.Single(r => r.Division == Division.RegionsGroup);
			Assert.Equal(101L, rg.CaseUrn);
			Assert.Equal(Region.London.Description(), rg.Area);

			var sfso = result.Single(r => r.Division == Division.SFSO);
			Assert.Equal(102L, sfso.CaseUrn);
			Assert.Equal(Territory.South_And_South_East__London.Description(), sfso.Area);
		}

		[Fact]
		public async Task Execute_ReturnsMappedResponsesAndRecordCount()
		{
			// Arrange
			var mockGateway = new Mock<ICaseSummaryGateway>();
			var parameters = new GetCaseSummariesByFilterParameters();
			var caseSummaryVm = new ActiveCaseSummaryVm {
				Rating = new ConcernsRating { 
					Name = "High" ,
					CreatedAt = DateTime.UtcNow.AddDays(-1),
					UpdatedAt = DateTime.UtcNow,
					Id = 1
				},
				TeamLedBy = "Leader" 
			};
			var caseSummaries = new List<ActiveCaseSummaryVm> { caseSummaryVm };
			var recordCount = 1;

			mockGateway
				.Setup(g => g.GetCaseSummariesByFilter(It.IsAny<GetCaseSummariesByFilterParameters>()))
				.ReturnsAsync((caseSummaries, recordCount));

			var sut = new GetConcernsCaseSummariesByFilter(mockGateway.Object);

			// Act
			var (result, count) = await sut.Execute(parameters);

			// Assert
			Assert.Single(result);
			Assert.Equal(recordCount, count);
			Assert.Equal("Leader", result[0].TeamLedBy);
		}
	}
}
