using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Base;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseServiceTests
	{
		[Test]
		public async Task WhenGetCasesByCaseworker_ReturnsCases()
		{
			// arrange
			var expectedCases = CaseFactory.BuildListCaseDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCases))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var cases = await caseService.GetCasesByCaseworkerAndStatus("caseworker", 1);

			// assert
			Assert.That(cases, Is.Not.Null);
			Assert.That(cases.Count, Is.EqualTo(expectedCases.Count));
			
			foreach (var caseDto in cases)
			{
				foreach (var expectedCase in expectedCases.Where(expectedCase => caseDto.Urn == expectedCase.Urn))
				{
					Assert.That(caseDto.Description, Is.EqualTo(expectedCase.Description));
					Assert.That(caseDto.Issue, Is.EqualTo(expectedCase.Issue));
					Assert.That(caseDto.Status, Is.EqualTo(expectedCase.Status));
					Assert.That(caseDto.Urn, Is.EqualTo(expectedCase.Urn));
					Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
					Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
					Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
					Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
					Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
					Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
					Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCase.NextSteps));
					Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCase.CaseAim));
					Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCase.DeEscalationPoint));
					Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCase.ReviewAt));
					Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCase.UpdatedAt));
					Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCase.DirectionOfTravel));
					Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCase.ReasonAtReview));
					Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCase.TrustUkPrn));
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_ThrowsException_ReturnsEmptyCases()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var cases = await caseService.GetCasesByCaseworkerAndStatus("caseworker", 1);

			// assert
			Assert.That(cases, Is.Not.Null);
			Assert.That(cases.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenGetCaseByUrn_ReturnsCase()
		{
			// arrange
			var expectedCase = CaseFactory.BuildCaseDto();
			var expectedCaseWrap = new ApiWrapper<CaseDto>(
				new List<CaseDto> { expectedCase }, 
				new ApiWrapper<CaseDto>.Pagination(1, 1, string.Empty));
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var logger = new Mock<ILogger<CaseService>>();
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCaseWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualCase = await caseService.GetCaseByUrn(1);

			// assert
			Assert.That(actualCase, Is.Not.Null);
			Assert.That(actualCase.Description, Is.EqualTo(expectedCase.Description));
			Assert.That(actualCase.Issue, Is.EqualTo(expectedCase.Issue));
			Assert.That(actualCase.Status, Is.EqualTo(expectedCase.Status));
			Assert.That(actualCase.Urn, Is.EqualTo(expectedCase.Urn));
			Assert.That(actualCase.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
			Assert.That(actualCase.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
			Assert.That(actualCase.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
			Assert.That(actualCase.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
			Assert.That(actualCase.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
			Assert.That(actualCase.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
			Assert.That(actualCase.NextSteps, Is.EqualTo(expectedCase.NextSteps));
			Assert.That(actualCase.CaseAim, Is.EqualTo(expectedCase.CaseAim));
			Assert.That(actualCase.DeEscalationPoint, Is.EqualTo(expectedCase.DeEscalationPoint));
			Assert.That(actualCase.ReviewAt, Is.EqualTo(expectedCase.ReviewAt));
			Assert.That(actualCase.UpdatedAt, Is.EqualTo(expectedCase.UpdatedAt));
			Assert.That(actualCase.DirectionOfTravel, Is.EqualTo(expectedCase.DirectionOfTravel));
			Assert.That(actualCase.ReasonAtReview, Is.EqualTo(expectedCase.ReasonAtReview));
			Assert.That(actualCase.TrustUkPrn, Is.EqualTo(expectedCase.TrustUkPrn));
		}
		
		[Test]
		public void WhenGetCaseByUrn_ReturnsException()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => caseService.GetCaseByUrn(1));
		}
		
		[Test]
		public void WhenGetCaseByUrn_UnwrapResponse_ReturnsException()
		{
			// arrange
			var expectedCaseWrap = new ApiWrapper<CaseDto>(null, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var logger = new Mock<ILogger<CaseService>>();
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCaseWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<Exception>(() => caseService.GetCaseByUrn(1));
		}		
		
		[Test]
		public async Task WhenGetCasesByTrustUkPrn_ReturnsCases()
		{
			// arrange
			var expectedCases = CaseFactory.BuildListCaseDto();
			var expectedCaseWrap = new ApiWrapper<CaseDto>(
				expectedCases, 
				new ApiWrapper<CaseDto>.Pagination(1, 1, string.Empty));
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCaseWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var apiWrapperCasesDto = await caseService.GetCasesByTrustUkPrn(new CaseTrustSearch("trust-ukprn"));

			// assert
			Assert.That(apiWrapperCasesDto, Is.Not.Null);
			Assert.That(apiWrapperCasesDto.Data, Is.Not.Null);
			Assert.That(apiWrapperCasesDto.Data.Count, Is.EqualTo(expectedCases.Count));
			Assert.That(apiWrapperCasesDto.Paging, Is.Not.Null);
			Assert.That(apiWrapperCasesDto.Paging.Page, Is.EqualTo(1));
			Assert.That(apiWrapperCasesDto.Paging.RecordCount, Is.EqualTo(1));
			Assert.That(apiWrapperCasesDto.Paging.NextPageUrl, Is.EqualTo(string.Empty));
			
			foreach (var caseDto in apiWrapperCasesDto.Data)
			{
				foreach (var expectedCase in expectedCases.Where(expectedCase => caseDto.Urn == expectedCase.Urn))
				{
					Assert.That(caseDto.Description, Is.EqualTo(expectedCase.Description));
					Assert.That(caseDto.Issue, Is.EqualTo(expectedCase.Issue));
					Assert.That(caseDto.Status, Is.EqualTo(expectedCase.Status));
					Assert.That(caseDto.Urn, Is.EqualTo(expectedCase.Urn));
					Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
					Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
					Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
					Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
					Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
					Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
					Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCase.NextSteps));
					Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCase.CaseAim));
					Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCase.DeEscalationPoint));
					Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCase.ReviewAt));
					Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCase.UpdatedAt));
					Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCase.DirectionOfTravel));
					Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCase.ReasonAtReview));
					Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCase.TrustUkPrn));
				}
			}
		}
		
		[Test]
		public void WhenGetCasesByTrustUkPrn_ThrowsException()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			Assert.ThrowsAsync<HttpRequestException>(() => caseService.GetCasesByTrustUkPrn(new CaseTrustSearch("trust-ukprn")));
		}
		
		[Test]
		public async Task WhenGetCasesByPagination_ReturnsCases()
		{
			// arrange
			var expectedCases = CaseFactory.BuildListCaseDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCases))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var cases = await caseService.GetCasesByPagination(new CaseSearch());

			// assert
			Assert.That(cases, Is.Not.Null);
			Assert.That(cases.Count, Is.EqualTo(expectedCases.Count));
			
			foreach (var caseDto in cases)
			{
				foreach (var expectedCase in expectedCases.Where(expectedCase => caseDto.Urn == expectedCase.Urn))
				{
					Assert.That(caseDto.Description, Is.EqualTo(expectedCase.Description));
					Assert.That(caseDto.Issue, Is.EqualTo(expectedCase.Issue));
					Assert.That(caseDto.Status, Is.EqualTo(expectedCase.Status));
					Assert.That(caseDto.Urn, Is.EqualTo(expectedCase.Urn));
					Assert.That(caseDto.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
					Assert.That(caseDto.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
					Assert.That(caseDto.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
					Assert.That(caseDto.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
					Assert.That(caseDto.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
					Assert.That(caseDto.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
					Assert.That(caseDto.NextSteps, Is.EqualTo(expectedCase.NextSteps));
					Assert.That(caseDto.CaseAim, Is.EqualTo(expectedCase.CaseAim));
					Assert.That(caseDto.DeEscalationPoint, Is.EqualTo(expectedCase.DeEscalationPoint));
					Assert.That(caseDto.ReviewAt, Is.EqualTo(expectedCase.ReviewAt));
					Assert.That(caseDto.UpdatedAt, Is.EqualTo(expectedCase.UpdatedAt));
					Assert.That(caseDto.DirectionOfTravel, Is.EqualTo(expectedCase.DirectionOfTravel));
					Assert.That(caseDto.ReasonAtReview, Is.EqualTo(expectedCase.ReasonAtReview));
					Assert.That(caseDto.TrustUkPrn, Is.EqualTo(expectedCase.TrustUkPrn));
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByPagination_ThrowsException_ReturnsEmptyCases()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var cases = await caseService.GetCasesByPagination(new CaseSearch());

			// assert
			Assert.That(cases, Is.Not.Null);
			Assert.That(cases.Count, Is.EqualTo(0));
		}
		
		[Test]
		public async Task WhenPostCase_ReturnsCase()
		{
			// arrange
			var expectedCase = CaseFactory.BuildCaseDto();
			var expectedCaseWrap = new ApiWrapper<CaseDto>(
				new List<CaseDto> { expectedCase }, 
				new ApiWrapper<CaseDto>.Pagination(1, 1, string.Empty));
			
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var logger = new Mock<ILogger<CaseService>>();
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCaseWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualCase = await caseService.PostCase(CaseFactory.BuildCreateCaseDto());

			// assert
			Assert.That(actualCase, Is.Not.Null);
			Assert.That(actualCase.Description, Is.EqualTo(expectedCase.Description));
			Assert.That(actualCase.Issue, Is.EqualTo(expectedCase.Issue));
			Assert.That(actualCase.Status, Is.EqualTo(expectedCase.Status));
			Assert.That(actualCase.Urn, Is.EqualTo(expectedCase.Urn));
			Assert.That(actualCase.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
			Assert.That(actualCase.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
			Assert.That(actualCase.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
			Assert.That(actualCase.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
			Assert.That(actualCase.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
			Assert.That(actualCase.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
			Assert.That(actualCase.NextSteps, Is.EqualTo(expectedCase.NextSteps));
			Assert.That(actualCase.CaseAim, Is.EqualTo(expectedCase.CaseAim));
			Assert.That(actualCase.DeEscalationPoint, Is.EqualTo(expectedCase.DeEscalationPoint));
			Assert.That(actualCase.ReviewAt, Is.EqualTo(expectedCase.ReviewAt));
			Assert.That(actualCase.UpdatedAt, Is.EqualTo(expectedCase.UpdatedAt));
			Assert.That(actualCase.DirectionOfTravel, Is.EqualTo(expectedCase.DirectionOfTravel));
			Assert.That(actualCase.ReasonAtReview, Is.EqualTo(expectedCase.ReasonAtReview));
			Assert.That(actualCase.TrustUkPrn, Is.EqualTo(expectedCase.TrustUkPrn));
		}
		
		[Test]
		public void WhenPostCase_ReturnsException()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			Assert.ThrowsAsync<HttpRequestException>(() => caseService.PostCase(CaseFactory.BuildCreateCaseDto()));
		}
		
		[Test]
		public void WhenPostCase_UnwrapResponse_ReturnsException()
		{
			// arrange
			var expectedCaseWrap = new ApiWrapper<CaseDto>(null, null);
			
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var logger = new Mock<ILogger<CaseService>>();
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCaseWrap))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			Assert.ThrowsAsync<Exception>(() => caseService.PostCase(CaseFactory.BuildCreateCaseDto()));
		}		
		
		[Test]
		public async Task WhenPatchCaseByUrn_ReturnsCase()
		{
			// arrange
			var expectedCase = CaseFactory.BuildCaseDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedCase))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act
			var actualCase = await caseService.PatchCaseByUrn(CaseFactory.BuildCaseDto());

			// assert
			Assert.That(actualCase, Is.Not.Null);
			Assert.That(actualCase.Description, Is.EqualTo(expectedCase.Description));
			Assert.That(actualCase.Issue, Is.EqualTo(expectedCase.Issue));
			Assert.That(actualCase.Status, Is.EqualTo(expectedCase.Status));
			Assert.That(actualCase.Urn, Is.EqualTo(expectedCase.Urn));
			Assert.That(actualCase.ClosedAt, Is.EqualTo(expectedCase.ClosedAt));
			Assert.That(actualCase.CreatedAt, Is.EqualTo(expectedCase.CreatedAt));
			Assert.That(actualCase.CreatedBy, Is.EqualTo(expectedCase.CreatedBy));
			Assert.That(actualCase.CrmEnquiry, Is.EqualTo(expectedCase.CrmEnquiry));
			Assert.That(actualCase.CurrentStatus, Is.EqualTo(expectedCase.CurrentStatus));
			Assert.That(actualCase.DeEscalation, Is.EqualTo(expectedCase.DeEscalation));
			Assert.That(actualCase.NextSteps, Is.EqualTo(expectedCase.NextSteps));
			Assert.That(actualCase.CaseAim, Is.EqualTo(expectedCase.CaseAim));
			Assert.That(actualCase.DeEscalationPoint, Is.EqualTo(expectedCase.DeEscalationPoint));
			Assert.That(actualCase.ReviewAt, Is.EqualTo(expectedCase.ReviewAt));
			Assert.That(actualCase.UpdatedAt, Is.EqualTo(expectedCase.UpdatedAt));
			Assert.That(actualCase.DirectionOfTravel, Is.EqualTo(expectedCase.DirectionOfTravel));
			Assert.That(actualCase.ReasonAtReview, Is.EqualTo(expectedCase.ReasonAtReview));
			Assert.That(actualCase.TrustUkPrn, Is.EqualTo(expectedCase.TrustUkPrn));
		}
		
		[Test]
		public void WhenPatchCaseByUrn_ReturnsException()
		{
			// arrange
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.BadRequest
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<CaseService>>();
			var caseService = new CaseService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => caseService.PatchCaseByUrn(CaseFactory.BuildCaseDto()));
		}

		[Test]
		public void WhenCaseSearchIncrements_ReturnsNextPage()
		{
			// arrange
			var caseSearch = new CaseSearch();

			// act
			var page = caseSearch.Page;
			var nextPage = caseSearch.PageIncrement();

			// assert
			Assert.That(page, Is.EqualTo(1));
			Assert.That(nextPage, Is.EqualTo(2));
		}
		
		[Test]
		public void WhenBuildRequestUri_ReturnsRequestUrl()
		{
			// arrange
			var caseTrustSearch = CaseFactory.BuildCaseTrustSearch("trust-ukprn");

			// act
			var requestUri = CaseService.BuildRequestUri(caseTrustSearch);

			// assert
			Assert.That(requestUri, Is.Not.Null);
			Assert.That(requestUri, Is.EqualTo("page%3d1"));
		}
	}
}