﻿using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.CaseActions;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.CaseActions
{
	public class SRMAProviderTests
	{
		[Test]
		public void GetSRMAsByCaseId_Returns_ListOfSrmaDto()
		{
			// Arrange
			var logger = new Mock<ILogger<SRMAProvider>>();

			var srmas = new SRMADto[] {
			new SRMADto
			{
				Id = 654,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test1"
			},
			new SRMADto
			{
				Id = 655,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.RegionsGroupIntervention,
				Notes = "Test2"
			},
			new SRMADto
			{
				Id = 656,
				Status = SRMAStatus.PreparingForDeployment,
				Reason = SRMAReasonOffered.OfferLinked,
				Notes = "Test3"
			}};

			var httpClientFactory = CreateMockFactory(srmas.ToList());

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.GetSRMAsForCase(123).Result;

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(srmas.Length, response.Count);
			Assert.AreEqual(srmas.First().Id, response.First().Id);
			Assert.AreEqual(srmas.First().Status, response.First().Status);
			Assert.AreEqual(srmas.First().Reason, response.First().Reason);
			Assert.AreEqual(srmas.First().Notes, response.First().Notes);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.GetSRMAById(654).Result;

			// Assert
			Assert.AreEqual(srmaDto.Id, response.Id);
			Assert.AreEqual(srmaDto.Status, response.Status);
			Assert.AreEqual(srmaDto.Reason, response.Reason);
			Assert.AreEqual(srmaDto.Notes, response.Notes);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SaveSRMA(expectedSRMADto).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
			Assert.AreEqual(expectedSRMADto.Notes, response.Notes);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetDateAccepted(654, dateAccepted).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
			Assert.AreEqual(expectedSRMADto.Notes, response.Notes);
			Assert.AreEqual(expectedSRMADto.DateAccepted, response.DateAccepted);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetDateClosed(654).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
			Assert.AreEqual(expectedSRMADto.Notes, response.Notes);
			Assert.AreEqual(expectedSRMADto.ClosedAt, response.ClosedAt);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetDateReportSent(654, dateReportSent).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
			Assert.AreEqual(expectedSRMADto.Notes, response.Notes);
			Assert.AreEqual(expectedSRMADto.DateReportSentToTrust, response.DateReportSentToTrust);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetNotes(654, notes).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
			Assert.AreEqual(expectedSRMADto.Notes, response.Notes);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetOfferedDate(654, offeredDate).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
			Assert.AreEqual(expectedSRMADto.DateOffered, response.DateOffered);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetVisitDates(654, visitStartDate, visitEndDate).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
			Assert.AreEqual(expectedSRMADto.DateVisitStart, response.DateVisitStart);
			Assert.AreEqual(expectedSRMADto.DateVisitEnd, response.DateVisitEnd);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetStatus(654, status).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
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

			var sut = new SRMAProvider(httpClientFactory.Object, logger.Object, Mock.Of<ICorrelationContext>(), Mock.Of<IClientUserInfoService>());

			// Act
			var response = sut.SetReason(654, reason).Result;

			// Assert
			Assert.AreEqual(expectedSRMADto.Id, response.Id);
			Assert.AreEqual(expectedSRMADto.Status, response.Status);
			Assert.AreEqual(expectedSRMADto.Reason, response.Reason);
		}

		private Mock<IHttpClientFactory> CreateMockFactory<T>(T content)
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

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			return httpClientFactory;
		}
	}
}