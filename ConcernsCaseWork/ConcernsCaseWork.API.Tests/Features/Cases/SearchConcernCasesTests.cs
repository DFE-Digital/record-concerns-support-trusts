using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using Moq;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Features.Cases
{
	public class SearchConcernCasesTests
	{
		[Fact]
		public async Task Execute_ReturnsMappedResponsesAndCount()
		{
			// Arrange
			var mockGateway = new Mock<ICaseSummaryGateway>();
			var parameters = new SearchCasesParameters { Page = 1, Count = 10 };
			var vmList = new List<ActiveCaseSummaryVm>
			{
				new() {
					ActiveConcerns = [],
					Rating = new ConcernsRating(),
					CaseLastUpdatedAt = null
				}
			};
			int recordCount = 1;

			mockGateway.Setup(g => g.SearchActiveCases(parameters))
				.ReturnsAsync((vmList, recordCount));

			var sut = new SearchConcernCases(mockGateway.Object);

			// Act
			var (responses, count) = await sut.Execute(parameters);

			// Assert
			Assert.NotNull(responses);
			Assert.Single(responses);
			Assert.Equal(recordCount, count);
			Assert.IsType<ActiveCaseSummaryResponse>(responses[0]);
		}
	}

}
