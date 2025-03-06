using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.CaseActions;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.CaseActions
{
	public class SRMAProviderTests
	{
		private Mock<IClientUserInfoService> _clientUserInfoService;
		[SetUp]
		public void Setup()
		{
			_clientUserInfoService = new Mock<IClientUserInfoService>();
			_clientUserInfoService.Setup(x => x.UserInfo).Returns(new UserInfo());
		}
		[Test]
		public void GetSRMAsByCaseId_Returns_ListOfSrmaDto()
		{
			// Arrange
			var logger = new Mock<ILogger<SRMAProvider>>();

			var srmas = new SRMADto[] {
			new() {
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test1"
			},
			new() {
				Id = 655,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test2"
			},
			new() {
				Id = 656,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.OfferLinked,
				Notes = "Test3"
			}};

			var httpClientFactory = CreateMockFactory(srmas.ToList());

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());

			// Act
			var response = sut.GetSRMAsForCase(123).Result;

			// Assert
			Assert.That(response, Is.Not.Null);
			Assert.That(srmas.Length, Is.EqualTo(response.Count));
			Assert.That(srmas.First().Id, Is.EqualTo(response.First().Id));
			Assert.That(srmas.First().Status, Is.EqualTo(response.First().Status));
			Assert.That(srmas.First().Reason, Is.EqualTo(response.First().Reason));
			Assert.That(srmas.First().Notes, Is.EqualTo(response.First().Notes));
		}

		[Test]
		public void GetSRMAById_Returns_SRMADto()
		{
			// Arrange
			var srmaDto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test"
			};

			var httpClientFactory = CreateMockFactory(srmaDto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());

			// Act
			var response = sut.GetSRMAById(654).Result;

			// Assert
			Assert.That(srmaDto.Id, Is.EqualTo(response.Id));
			Assert.That(srmaDto.Status, Is.EqualTo(response.Status));
			Assert.That(srmaDto.Reason, Is.EqualTo(response.Reason));
			Assert.That(srmaDto.Notes, Is.EqualTo(response.Notes));
		}

		[Test]
		public void CreateSRMA_Returns_New_SRMADto()
		{
			// Arrange
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test"
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SaveSRMA(expectedSRMADto).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
			Assert.That(expectedSRMADto.Notes, Is.EqualTo(response.Notes));
		}

		[Test]
		public void SetDateAccepted_Returns_UpdatedSRMA()
		{
			// Arrange
			var dateAccepted = DateTime.Now;
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test",
				DateAccepted = dateAccepted
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SetDateAccepted(654, dateAccepted).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
			Assert.That(expectedSRMADto.Notes, Is.EqualTo(response.Notes));
			Assert.That(expectedSRMADto.DateAccepted, Is.EqualTo(response.DateAccepted));
		}

		[Test]
		public void SetDateClosed_Returns_UpdatedSRMA()
		{
			// Arrange
			var dateClosed = DateTime.Now;
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test",
				ClosedAt = dateClosed
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SetDateClosed(654).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
			Assert.That(expectedSRMADto.Notes, Is.EqualTo(response.Notes));
			Assert.That(expectedSRMADto.ClosedAt, Is.EqualTo(response.ClosedAt));
		}

		[Test]
		public void SetDateReportSent_Returns_UpdatedSRMA()
		{
			// Arrange
			var dateReportSent = DateTime.Now;
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test",
				DateReportSentToTrust = dateReportSent
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SetDateReportSent(654, dateReportSent).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
			Assert.That(expectedSRMADto.Notes, Is.EqualTo(response.Notes));
			Assert.That(expectedSRMADto.DateReportSentToTrust, Is.EqualTo(response.DateReportSentToTrust));
		}

		[Test]
		public void SetNotes_Returns_UpdatedSRMA()
		{
			// Arrange
			var notes = "hello world";
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = notes
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SetNotes(654, notes).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
			Assert.That(expectedSRMADto.Notes, Is.EqualTo(response.Notes));
		}

		[Test]
		public void SetOfferedDate_Returns_UpdatedSRMA()
		{
			// Arrange
			var offeredDate = DateTime.Now;
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				DateOffered = offeredDate
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SetOfferedDate(654, offeredDate).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
			Assert.That(expectedSRMADto.DateOffered, Is.EqualTo(response.DateOffered));
		}

		[Test]
		public void SetVisitDates_Returns_UpdatedSRMA()
		{
			// Arrange
			var visitStartDate = DateTime.Now.AddDays(-3);
			var visitEndDate = DateTime.Now;
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				DateVisitStart = visitStartDate,
				DateVisitEnd = visitEndDate
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SetVisitDates(654, visitStartDate, visitEndDate).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
			Assert.That(expectedSRMADto.DateVisitStart, Is.EqualTo(response.DateVisitStart));
			Assert.That(expectedSRMADto.DateVisitEnd, Is.EqualTo(response.DateVisitEnd));
		}

		[Test]
		public void SetStatus_Returns_UpdatedSRMA()
		{
			// Arrange
			var status = SRMAStatus.Complete;
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = status,
				Reason = SRMAReasonOffered.RegionsGroupIntervention
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());
			// Act
			var response = sut.SetStatus(654, status).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
		}

		[Test]
		public void SetReason_Returns_UpdatedSRMA()
		{
			// Arrange
			var reason = SRMAReasonOffered.OfferLinked;
			var expectedSRMADto = new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.Complete,
				Reason = reason
			};

			var httpClientFactory = CreateMockFactory(expectedSRMADto);

			var logger = new Mock<ILogger<SRMAProvider>>();

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), _clientUserInfoService.Object, Mock.Of<IUserTokenService>());

			// Act
			var response = sut.SetReason(654, reason).Result;

			// Assert
			Assert.That(expectedSRMADto.Id, Is.EqualTo(response.Id));
			Assert.That(expectedSRMADto.Status, Is.EqualTo(response.Status));
			Assert.That(expectedSRMADto.Reason, Is.EqualTo(response.Reason));
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