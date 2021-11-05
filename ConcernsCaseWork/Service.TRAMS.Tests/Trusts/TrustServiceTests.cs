using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Base;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Trusts
{
	[Parallelizable(ParallelScope.All)]
	public class TrustServiceTests
	{
		[Test]
		public async Task WhenGetTrustsByPagination_ReturnsTrusts()
		{
			// arrange
			var expectedTrusts = TrustFactory.BuildListTrustSummaryDto();
			var expectedApiWrapperTrust = new ApiWrapper<TrustSummaryDto>(expectedTrusts, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperTrust))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(httpClientFactory.Object, logger.Object);
			
			// act
			var apiWrapperTrusts = await trustService.GetTrustsByPagination(TrustFactory.BuildTrustSearch());

			// assert
			Assert.That(apiWrapperTrusts, Is.Not.Null);
			Assert.That(apiWrapperTrusts.Data, Is.Not.Null);
			Assert.That(apiWrapperTrusts.Data.Count, Is.EqualTo(expectedTrusts.Count));

			foreach (var trust in apiWrapperTrusts.Data)
			{
				foreach (var expectedTrust in expectedTrusts)
				{
					Assert.That(trust.Establishments.Count, Is.EqualTo(expectedTrust.Establishments.Count));
					Assert.That(trust.Urn, Is.EqualTo(expectedTrust.Urn));
					Assert.That(trust.GroupName, Is.EqualTo(expectedTrust.GroupName));
					Assert.That(trust.UkPrn, Is.EqualTo(expectedTrust.UkPrn));
					Assert.That(trust.CompaniesHouseNumber, Is.EqualTo(expectedTrust.CompaniesHouseNumber));

					foreach (var establishment in trust.Establishments)
					{
						foreach (var expectedEstablishment in expectedTrust.Establishments)
						{
							Assert.That(establishment.Name, Is.EqualTo(expectedEstablishment.Name));
							Assert.That(establishment.Urn, Is.EqualTo(expectedEstablishment.Urn));
							Assert.That(establishment.UkPrn, Is.EqualTo(expectedEstablishment.UkPrn));
						}
					}
				}
			}
		}
		
		[Test]
		public void WhenGetTrustsByPagination_ThrowsException_ReturnsEmptyTrusts()
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
			
			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(httpClientFactory.Object, logger.Object);
			
			// act | assert
			Assert.ThrowsAsync<HttpRequestException>(() => trustService.GetTrustsByPagination(TrustFactory.BuildTrustSearch()));
		}

		[TestCase("", "", "", "page%3d1")]
		[TestCase("group-name", "", "", "groupName%3dgroup-name%26page%3d1")]
		[TestCase("", "ukprn", "", "ukprn%3dukprn%26page%3d1")]
		[TestCase("", "", "companies-house-number", "companiesHouseNumber%3dcompanies-house-number%26page%3d1")]
		[TestCase("group-name", "ukprn", "", "groupName%3dgroup-name%26ukprn%3dukprn%26page%3d1")]
		public void WhenBuildRequestUri_ReturnsRequestUrl(string groupName, string ukprn, string companiesHouseNumber, string expectedRequestUri)
		{
			// arrange
			var trustService = new TrustService(null, null);
			var trustSearch = TrustFactory.BuildTrustSearch(groupName, ukprn, companiesHouseNumber);

			// act
			var requestUri = trustService.BuildRequestUri(trustSearch);

			// assert
			Assert.That(requestUri, Is.Not.Null);
			Assert.That(requestUri, Is.EqualTo(expectedRequestUri));
		}

		[Test]
		public async Task WhenGetTrustByUkPrn_ReturnsTrust()
		{
			// arrange
			var expectedTrust = TrustFactory.BuildTrustDetailsDto();
			var expectedApiWrapperTrust = new ApiWrapper<TrustDetailsDto>(new List<TrustDetailsDto> { expectedTrust }, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedApiWrapperTrust))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(httpClientFactory.Object, logger.Object);
			
			// act
			var trustDetailDto = await trustService.GetTrustByUkPrn("999999");

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
		public void WhenGetTrustByUkPrn_ThrowsException()
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
			
			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => trustService.GetTrustByUkPrn("9999999"));
		}
	}
}