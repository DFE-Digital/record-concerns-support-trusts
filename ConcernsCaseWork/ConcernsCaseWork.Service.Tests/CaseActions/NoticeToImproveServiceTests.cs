using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.CaseActions
{
	public class NoticeToImproveServiceTests
	{
		private Mock<IClientUserInfoService> _clientUserInfoService;
		[SetUp]
		public void Setup()
		{
			_clientUserInfoService = new Mock<IClientUserInfoService>();
			_clientUserInfoService.Setup(x => x.UserInfo).Returns(new UserInfo());
		}
		[Test]
		public void GetNtisByCaseId_Returns_ListOfNtiDto()
		{
			// Arrange
			var logger = new Mock<ILogger<NtiUnderConsiderationService>>();

			var ntis = new List<NtiUnderConsiderationDto> {
			new() {
				Id = 654,
				Notes = "Test1"
			},
			new() {
				Id = 667,
				Notes = "Test2"
			},
			new() {
				Id = 948,
				Notes = "Test3"
			}};

			var httpClientFactory = CreateMockFactory(ntis);

			var sut = new NtiUnderConsiderationService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());

			// Act
			var response = sut.GetNtisForCase(123).Result;
			Assert.Multiple(() =>
			{

				// Assert
				Assert.That(response, Is.Not.Null);
				Assert.That(ntis.Count, Is.EqualTo(response.Count));
				Assert.That(ntis.First().Id, Is.EqualTo(response.First().Id));
				Assert.That(ntis.First().Notes, Is.EqualTo(response.First().Notes));
			});
		}

		[Test]
		public void CreateNti_Returns_New_NtiDto()
		{
			// Arrange
			var expectedNtiDto = new NtiUnderConsiderationDto
			{
				Id = 654,

				Notes = "Test"
			};

			var httpClientFactory = CreateMockFactory(expectedNtiDto);

			var logger = new Mock<ILogger<NtiUnderConsiderationService>>();

			var sut = new NtiUnderConsiderationService(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());

			// Act
			var response = sut.CreateNti(expectedNtiDto).Result;

			// Assert
			Assert.That(expectedNtiDto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedNtiDto.Notes, Is.EqualTo(response.Notes));
		}

		private static Mock<IHttpClientFactory> CreateMockFactory<T>(T content)
		{
			var concernsApiEndpoint = "https://localhost";

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(new ApiWrapper<T>(content)))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object)
			{
				BaseAddress = new Uri(concernsApiEndpoint)
			};
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			return httpClientFactory;
		}
	}
}