using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Features.Case
{
    public class GetConcernsCaseSummariesByFilterTests
	{
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
