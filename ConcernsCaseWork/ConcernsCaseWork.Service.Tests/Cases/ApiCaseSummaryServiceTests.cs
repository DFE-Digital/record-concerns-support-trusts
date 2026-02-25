using AutoFixture;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Moq;
using static ConcernsCaseWork.Service.Cases.CaseSummaryDto;

namespace ConcernsCaseWork.Service.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class ApiCaseSummaryServiceTests
    {
		public static readonly Fixture _fixture = new();

		[Test]
		public async Task SearchActiveCaseSummaries_ReturnsExpectedResult()
		{
			// Arrange
			var loggerMock = new Mock<ILogger<ApiCaseSummaryService>>();
			var correlationContextMock = new Mock<ICorrelationContext>();
			var clientFactoryMock = new Mock<IHttpClientFactory>();
			var userInfoServiceMock = new Mock<IClientUserInfoService>();

			var expectedData = new List<ActiveCaseSummaryDto>
				{
					new() { ActiveConcerns = new List<ConcernSummaryDto>() }
				};
			var expectedPaging = new Pagination();
			var expectedResult = new ApiListWrapper<ActiveCaseSummaryDto>(expectedData, expectedPaging);

			var serviceMock = new Mock<ApiCaseSummaryService>(
				loggerMock.Object,
				correlationContextMock.Object,
				clientFactoryMock.Object,
				userInfoServiceMock.Object
			)
			{ CallBase = true };

			serviceMock
				.Setup(x => x.GetByPagination<ActiveCaseSummaryDto>(It.IsAny<string>(), It.IsAny<bool>()))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await serviceMock.Object.SearchActiveCaseSummaries(1);

			// Assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(expectedResult.Data, Is.EqualTo(result.Data));
				Assert.That(expectedResult.Paging, Is.EqualTo(result.Paging));
			});
		}
	}
}
