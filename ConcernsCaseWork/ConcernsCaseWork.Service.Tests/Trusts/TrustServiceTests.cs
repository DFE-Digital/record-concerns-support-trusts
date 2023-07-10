using AutoFixture;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.UserContext;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using Moq.Protected;
using NUnit.Framework.Internal;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class TrustServiceTests
	{
		private static readonly Fixture _fixture = new();

		[TestCaseSource(nameof(TrustSearchTestCases))]
		public async Task WhenGetTrustsByPagination_ReturnsTrusts(string expectedUrl, IFeatureManager featureManager)
		{
			// arrange
			var expectedTrusts = TrustFactory.BuildListTrustSummaryDto();
			var pagination = _fixture.Create<Pagination>();
			pagination.Page = 1;
			pagination.RecordCount = expectedTrusts.Count;

			var expectedApiWrapperTrust = new ApiListWrapper<TrustSearchDto>(expectedTrusts, pagination);
			var tramsApiEndpoint = "https://localhost";
			HttpRequestMessage sentRequest = null;

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
				{
					sentRequest = request;

					var response = new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.OK,
						Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperTrust))
					};

					return response;
				} );

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(
				httpClientFactory.Object,
				logger.Object,
				Mock.Of<ICorrelationContext>(),
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				CreateCityTechnologyCollegeService(),
				featureManager);

			// act
			var apiWrapperTrusts = await trustService.GetTrustsByPagination(TrustFactory.BuildTrustSearch(), expectedTrusts.Count);

			// assert
			Assert.That(apiWrapperTrusts, Is.Not.Null);
			Assert.That(apiWrapperTrusts.Trusts, Is.Not.Null);
			Assert.That(apiWrapperTrusts.Trusts.Count, Is.EqualTo(expectedTrusts.Count));

			sentRequest.RequestUri.AbsoluteUri.Should().Contain(expectedUrl);

			foreach (var trust in apiWrapperTrusts.Trusts)
			{
				foreach (var expectedTrust in expectedTrusts)
				{
					Assert.That(trust.Urn, Is.EqualTo(expectedTrust.Urn));
					Assert.That(trust.GroupName, Is.EqualTo(expectedTrust.GroupName));
					Assert.That(trust.UkPrn, Is.EqualTo(expectedTrust.UkPrn));
					Assert.That(trust.CompaniesHouseNumber, Is.EqualTo(expectedTrust.CompaniesHouseNumber));
				}
			}
		}

		[Test]
		public async Task WhenGetTrustsByPagination_MatchesFakeTrust_ReturnsFakeTrust()
		{
			var trustInfo = new TrustSearchResponseDto()
			{
				Trusts = new List<TrustSearchDto>()
				{
					new TrustSearchDto()
					{
						UkPrn = "123"
					}
				}
			};

			var fakeTrustService = new Mock<IFakeTrustService>();
			fakeTrustService.Setup(m => m.GetTrustsByPagination(It.IsAny<string>())).Returns(trustInfo);

			var trustService = new TrustService(
				Mock.Of<IHttpClientFactory>(),
				Mock.Of<ILogger<TrustService>>(),
				Mock.Of<ICorrelationContext>(),
				Mock.Of<IClientUserInfoService>(),
				fakeTrustService.Object,
				CreateCityTechnologyCollegeService(),
				CreateFeatureManagerV2());

			var trustSearchParams = new TrustSearch() { GroupName = "Test" };

			var result = await trustService.GetTrustsByPagination(trustSearchParams, 1);

			result.Trusts.Should().HaveCount(1);
			result.Trusts.First().UkPrn.Should().Be("123");
		}

		[Test]
		public async Task WhenGetTrustsByPagination_MatchesCTC_ReturnsCTC()
		{
			//Arrange
			var expectedApiWrapperTrust = new ApiListWrapper<TrustSearchDto>(null, null);
			var tramsApiEndpoint = "https://localhost";
			HttpRequestMessage sentRequest = null;

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
				{
					sentRequest = request;

					var response = new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.OK,
						Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperTrust))
					};

					return response;
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var trustInfo = new TrustSearchResponseDto()
			{
				Trusts = new List<TrustSearchDto>()
				{
					new TrustSearchDto()
					{
						UkPrn = "987"
					}
				}
			};

			var r = Task.FromResult(trustInfo);

			var ctcService = new Mock<ICityTechnologyCollegeService>();
			ctcService.Setup(m => m.GetCollegeByPagination(It.IsAny<string>())).Returns(r);
			
			var trustService = new TrustService(
				httpClientFactory.Object,
				Mock.Of<ILogger<TrustService>>(),
				Mock.Of<ICorrelationContext>(),
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				ctcService.Object,
				CreateFeatureManagerCTCEnabled());

			var trustSearchParams = new TrustSearch() { GroupName = "Test" };

			//Act
			var result = await trustService.GetTrustsByPagination(trustSearchParams, 1);

			//Assert
			result.Trusts.Should().HaveCount(1);
			result.Trusts.First().UkPrn.Should().Be("987");
		}

		[Test]
		public void WhenGetTrustsByPagination_ThrowsException()
		{
			// arrange
			var tramsApiEndpoint = "https://localhost";

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

			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(
				httpClientFactory.Object, 
				logger.Object, 
				Mock.Of<ICorrelationContext>(), 
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				CreateCityTechnologyCollegeService(),
				CreateFeatureManagerV2());

			// act | assert
			Assert.ThrowsAsync<HttpRequestException>(() => trustService.GetTrustsByPagination(TrustFactory.BuildTrustSearch(), 100));
		}

		[TestCase("", "", "", "page%3d1%26count%3d45%26includeEstablishments%3dFalse")]
		[TestCase("group-name", "", "", "groupName%3dgroup-name%26page%3d1%26count%3d45%26includeEstablishments%3dFalse")]
		[TestCase("", "ukprn", "", "page%3d1%26count%3d45%26includeEstablishments%3dFalse")]
		[TestCase("", "1234", "", "ukprn%3d1234%26page%3d1%26count%3d45%26includeEstablishments%3dFalse")]
		[TestCase("", "", "companies-house-number", "companiesHouseNumber%3dcompanies-house-number%26page%3d1%26count%3d45%26includeEstablishments%3dFalse")]
		[TestCase("group-name", "ukprn", "", "groupName%3dgroup-name%26page%3d1%26count%3d45%26includeEstablishments%3dFalse")]
		[TestCase("group-name", "1234", "", "groupName%3dgroup-name%26ukprn%3d1234%26page%3d1%26count%3d45%26includeEstablishments%3dFalse")]
		public void WhenBuildRequestUri_ReturnsRequestUrl(string groupName, string ukprn, string companiesHouseNumber, string expectedRequestUri)
		{
			// arrange
			var trustService = new TrustService(
				Mock.Of<IHttpClientFactory>(), 
				Mock.Of<ILogger<TrustService>>(), 
				Mock.Of<ICorrelationContext>(), 
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				CreateCityTechnologyCollegeService(),
				CreateFeatureManagerV2());

			var trustSearch = TrustFactory.BuildTrustSearch(groupName, ukprn, companiesHouseNumber);

			// act
			var requestUri = trustService.BuildRequestUri(trustSearch, 45);

			// assert
			Assert.That(requestUri, Is.Not.Null);
			Assert.That(requestUri, Is.EqualTo(expectedRequestUri));
		}

		[Test]
		public async Task WhenGetTrustByUkPrn_ReturnsTrust()
		{
			// arrange
			var expectedTrust = TrustFactory.BuildTrustDetailsDto();
			var expectedApiWrapperTrust = new ApiWrapper<TrustDetailsDto>(expectedTrust);
			var tramsApiEndpoint = "https://localhost";
			HttpRequestMessage sentRequest = null;

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
				{
					sentRequest = request;

					var response = new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.OK,
						Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperTrust))
					};

					return response;
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(
				httpClientFactory.Object, 
				logger.Object, 
				Mock.Of<ICorrelationContext>(), 
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				CreateCityTechnologyCollegeService(),
				CreateFeatureManagerV2());

			// act
			var trustDetailDto = await trustService.GetTrustByUkPrn("999999");

			sentRequest.RequestUri.AbsoluteUri.Should().Contain("https://localhost/v2/trust/999999");

			// assert
			Assert.That(trustDetailDto, Is.Not.Null);
			Assert.That(trustDetailDto.GiasData, Is.Not.Null);
			Assert.That(trustDetailDto.GiasData.GroupId, Is.EqualTo(expectedTrust.GiasData.GroupId));
			Assert.That(trustDetailDto.GiasData.GroupName, Is.EqualTo(expectedTrust.GiasData.GroupName));
			Assert.That(trustDetailDto.GiasData.GroupTypeCode, Is.EqualTo(expectedTrust.GiasData.GroupTypeCode));
			Assert.That(trustDetailDto.GiasData.UkPrn, Is.EqualTo(expectedTrust.GiasData.UkPrn));
			Assert.That(trustDetailDto.GiasData.CompaniesHouseNumber, Is.EqualTo(expectedTrust.GiasData.CompaniesHouseNumber));
			Assert.That(trustDetailDto.GiasData.GroupContactAddress, Is.Not.Null);
			Assert.That(trustDetailDto.GiasData.GroupContactAddress.County, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress.County));
			Assert.That(trustDetailDto.GiasData.GroupContactAddress.Locality, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress.Locality));
			Assert.That(trustDetailDto.GiasData.GroupContactAddress.Postcode, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress.Postcode));
			Assert.That(trustDetailDto.GiasData.GroupContactAddress.Street, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress.Street));
			Assert.That(trustDetailDto.GiasData.GroupContactAddress.Town, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress.Town));
			Assert.That(trustDetailDto.GiasData.GroupContactAddress.AdditionalLine, Is.EqualTo(expectedTrust.GiasData.GroupContactAddress.AdditionalLine));
		}


		[Test]
		public async Task WhenGetTrustByUkPrn_MatchesFakeTrust_ReturnsFakeTrust()
		{
			var trustInfo = new TrustDetailsDto()
			{
				GiasData = new GiasDataDto()
				{
					UkPrn = "123",
				}
			};

			var fakeTrustService = new Mock<IFakeTrustService>();
			fakeTrustService.Setup(m => m.GetTrustByUkPrn(It.IsAny<string>())).Returns(trustInfo);

			var trustService = new TrustService(
				Mock.Of<IHttpClientFactory>(),
				Mock.Of<ILogger<TrustService>>(),
				Mock.Of<ICorrelationContext>(),
				Mock.Of<IClientUserInfoService>(),
				fakeTrustService.Object,
				CreateCityTechnologyCollegeService(),
				CreateFeatureManagerV2());

			var result = await trustService.GetTrustByUkPrn("123");

			result.GiasData.UkPrn.Should().Be("123");
		}

		[Test]
		public async Task WhenGetTrustByUkPrn_V3SearchEnabled_ReturnsTrust()
		{
			// arrange
			var trustDetails = TrustFactory.BuildTrustDetailsDto();
			var expectedTrust = new TrustDetailsV3Dto()
			{
				Establishments = trustDetails.Establishments,
				GiasData = trustDetails.GiasData,
				TrustData = trustDetails.IfdData
			};

			var expectedApiWrapperTrust = new ApiWrapper<TrustDetailsV3Dto>(expectedTrust);
			var tramsApiEndpoint = "https://localhost";
			HttpRequestMessage sentRequest = null;

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
				{
					sentRequest = request;

					var response = new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.OK,
						Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperTrust))
					};

					return response;
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(
				httpClientFactory.Object,
				logger.Object,
				Mock.Of<ICorrelationContext>(),
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				CreateCityTechnologyCollegeService(),
				CreateFeatureManagerV3());

			// act
			var trustDetailDto = await trustService.GetTrustByUkPrn("999999");

			sentRequest.RequestUri.AbsoluteUri.Should().Contain("https://localhost/v3/trust/999999");

			// assert
			Assert.That(trustDetailDto, Is.Not.Null);
			Assert.That(trustDetailDto.GiasData, Is.Not.Null);

			trustDetailDto.GiasData.Should().BeEquivalentTo(expectedTrust.GiasData);
			trustDetailDto.Establishments.Should().BeEquivalentTo(expectedTrust.Establishments);
			trustDetailDto.IfdData.Should().BeEquivalentTo(expectedTrust.TrustData);
		}

		[Test]
		public void WhenGetTrustByUkPrn_ThrowsException()
		{
			// arrange
			var tramsApiEndpoint = "https://localhost";

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

			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(
				httpClientFactory.Object, 
				logger.Object, 
				Mock.Of<ICorrelationContext>(), 
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				CreateCityTechnologyCollegeService(),
				CreateFeatureManagerV2());

			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => trustService.GetTrustByUkPrn("9999999"));
		}

		[Test]
		public async Task WhenGetTrustByUKPRN_MatchesCTCANDShouldReturnCTCEnabled_ReturnsCTC()
		{
			//Arrange
			var trustInfo = new TrustDetailsDto()
			{
				GiasData = new GiasDataDto()
				{
					UkPrn = "987",
				}
			};

			Task<TrustDetailsDto> returnedObject = Task.FromResult(trustInfo); 

			var fakeTrustService = new Mock<ICityTechnologyCollegeService>();
			fakeTrustService.Setup(m => m.GetCollegeByUkPrn(It.IsAny<string>())).Returns(returnedObject);

			//Act
			var trustService = new TrustService(
				Mock.Of<IHttpClientFactory>(),
				Mock.Of<ILogger<TrustService>>(),
				Mock.Of<ICorrelationContext>(),
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				fakeTrustService.Object,
				CreateFeatureManagerCTCEnabled());

			var result = await trustService.GetTrustByUkPrn("987");
			
			//Assert
			result.GiasData.UkPrn.Should().Be("987");
		}

		[Test]
		public async Task WhenGetTrustByUKPRN_MatchesCTCANDShouldReturnCTCDisabled_CheckforTrust()
		{
			string trustUKPRN = "987654321";
			string ctcUKPRN = "987651111";

			//Arrange
			var expectedTrust = TrustFactory.BuildTrustDetailsDto(trustUKPRN);
		 	var expectedApiWrapperTrust = new ApiWrapper<TrustDetailsDto>(null);
			var tramsApiEndpoint = "https://localhost";
			HttpRequestMessage sentRequest = null;

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
				{
					sentRequest = request;

					var response = new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.OK,
						Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperTrust))
					};

					return response;
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);


			var trustInfo = new TrustDetailsDto()
			{
				GiasData = new GiasDataDto()
				{
					UkPrn = ctcUKPRN
				}
			};

			Task<TrustDetailsDto> returnedObject = Task.FromResult(trustInfo);

			var cityTechnicalCollegueService = new Mock<ICityTechnologyCollegeService>();
			cityTechnicalCollegueService.Setup(m => m.GetCollegeByUkPrn(It.IsAny<string>())).Returns(returnedObject);


			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(
				httpClientFactory.Object,
				logger.Object,
				Mock.Of<ICorrelationContext>(),
				Mock.Of<IClientUserInfoService>(),
				CreateFakeTrustService(),
				cityTechnicalCollegueService.Object,
				CreateFeatureManagerCTCDisabled());

			//Act
			var trustDetailDto = await trustService.GetTrustByUkPrn(trustUKPRN);


			//Assert
			sentRequest.RequestUri.AbsoluteUri.Should().Contain($"https://localhost/v2/trust/{trustUKPRN}");
			Assert.That(trustDetailDto, Is.Null);
		}

		private static IFakeTrustService CreateFakeTrustService()
		{
			var fakeTrustService = new Mock<IFakeTrustService>();
			fakeTrustService.Setup(m => m.GetTrustByUkPrn(It.IsAny<string>())).Returns(() => null);
			fakeTrustService.Setup(m => m.GetTrustsByPagination(It.IsAny<string>())).Returns(() => null);
			return fakeTrustService.Object;
		}

		private static ICityTechnologyCollegeService CreateCityTechnologyCollegeService()
		{
			var fakeTrustService = new Mock<ICityTechnologyCollegeService>();
			fakeTrustService.Setup(m => m.GetCollegeByUkPrn(It.IsAny<string>())).Returns(() => null);
			fakeTrustService.Setup(m => m.GetCollegeByPagination(It.IsAny<string>())).Returns(() => null);
			return fakeTrustService.Object;
		}

		private static IFeatureManager CreateFeatureManagerV2()
		{
			return CreateFeatureManager(FeatureFlags.IsV3TrustSearchEnabled, false);
		}

		private static IFeatureManager CreateFeatureManagerCTCEnabled()
		{
			return CreateFeatureManager(FeatureFlags.ShouldCTCBeIncludedInTrustSearch, true);
		}

		private static IFeatureManager CreateFeatureManagerCTCDisabled()
		{
			return CreateFeatureManager(FeatureFlags.ShouldCTCBeIncludedInTrustSearch, false);
		}

		private static IFeatureManager CreateFeatureManagerV3()
		{
			return CreateFeatureManager(FeatureFlags.IsV3TrustSearchEnabled,true);
		}

		private static IFeatureManager CreateFeatureManager(string featureFlagName, bool returnValue)
		{
			var result = new Mock<IFeatureManager>();

			result.Setup(m => m.IsEnabledAsync(featureFlagName)).ReturnsAsync(returnValue);

			return result.Object;
		}

		private static IEnumerable<TestCaseData> TrustSearchTestCases()
		{
			yield return new TestCaseData("https://localhost/v2/trusts", CreateFeatureManagerV2());
			yield return new TestCaseData("https://localhost/v3/trusts", CreateFeatureManagerV3());
		}

		private static IEnumerable<TestCaseData> SearchByUkPrnTestCases()
		{
			yield return new TestCaseData("https://localhost/v2/trust/999999", CreateFeatureManagerV2());
			yield return new TestCaseData("https://localhost/v3/trust/999999", CreateFeatureManagerV3());
		}
	}
}