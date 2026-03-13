using AutoFixture;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConcernsCaseWork.Service.Tests.Cases
{
	public class ApiCaseSummaryServiceTests
	{
		public static readonly Fixture _fixture = new();

		[Test]
		public async Task GetCaseSearchCriterias_ReturnsCaseSearchParametersDto()
		{
			// Arrange
			var loggerMock = new Mock<ILogger<ApiCaseSummaryService>>();
			var correlationContextMock = new Mock<ICorrelationContext>();
			var clientFactoryMock = new Mock<IHttpClientFactory>();
			var userInfoServiceMock = new Mock<IClientUserInfoService>();

			var serviceMock = new Mock<ApiCaseSummaryService>(
				loggerMock.Object,
				correlationContextMock.Object,
				clientFactoryMock.Object,
				userInfoServiceMock.Object
			)
			{ CallBase = true };

			var expectedDto = new CaseSearchParametersDto
			{
				CaseOwners = ["owner1", "owner2"],
				TeamLeaders = ["leader1", "leader2"]
			};

			serviceMock
				.Setup(x => x.Get<CaseSearchParametersDto>(It.IsAny<string>(), false))
				.ReturnsAsync(expectedDto);

			// Act
			var result = await serviceMock.Object.GetCaseSearchCriterias();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(expectedDto.CaseOwners, Is.EqualTo(result.CaseOwners));
				Assert.That(expectedDto.TeamLeaders, Is.EqualTo(result.TeamLeaders));
			});
		}

		[Test]
		public async Task GetAllCaseSummariesByFilter_ReturnsExpectedResult()
		{
			// Arrange
			var loggerMock = new Mock<ILogger<ApiCaseSummaryService>>();
			var correlationContextMock = new Mock<ICorrelationContext>();
			var clientFactoryMock = new Mock<IHttpClientFactory>();
			var userInfoServiceMock = new Mock<IClientUserInfoService>();

			var expectedData = new List<ActiveCaseSummaryDto>
				{
					new() { ActiveConcerns = [] }
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
			var result = await serviceMock.Object.GetAllCaseSummariesByFilter([], []);

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
