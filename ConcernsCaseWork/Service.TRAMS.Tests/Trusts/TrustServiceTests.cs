using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Trusts;
using System;
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
			var expectedTrusts = TrustFactory.CreateListTrustSummaryDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedTrusts))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<TrustService>>();
			var trustService = new TrustService(httpClientFactory.Object, logger.Object);
			
			// act
			var trusts = await trustService.GetTrustsByPagination(TrustFactory.CreateTrustSearch());

			// assert
			Assert.That(trusts, Is.Not.Null);
			Assert.That(trusts.Count, Is.EqualTo(expectedTrusts.Count));

			foreach (var trust in trusts)
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
		public async Task WhenGetTrustsByPagination_ThrowsException_ReturnsEmptyTrusts()
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
			
			// act
			var trusts = await trustService.GetTrustsByPagination(TrustFactory.CreateTrustSearch());

			// assert
			Assert.That(trusts, Is.Not.Null);
			Assert.That(trusts.Count, Is.EqualTo(0));
		}

		[TestCase("", "", "", "/trusts?page=1")]
		[TestCase("groupname", "", "", "/trusts?groupName=groupname&page=1")]
		[TestCase("", "ukprn", "", "/trusts?ukprn=ukprn&page=1")]
		[TestCase("", "", "companieshousenumber", "/trusts?companiesHouseNumber=companieshousenumber&page=1")]
		[TestCase("groupname", "ukprn", "", "/trusts?groupName=groupname&ukprn=ukprn&page=1")]
		public void WhenBuildRequestUri_ReturnsRequestUrl(string groupName, string ukprn, string companiesHouseNumber, string expectedRequestUri)
		{
			// arrange
			var trustService = new TrustService(null, null);
			var trustSearch = TrustFactory.CreateTrustSearch(groupName, ukprn, companiesHouseNumber);

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
			var expectedTrust = TrustFactory.CreateTrustDetailsDto();
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(expectedTrust))
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
			Assert.That(trustDetailDto.Gias, Is.Not.Null);
			Assert.That(trustDetailDto.Gias.GroupId, Is.EqualTo(expectedTrust.Gias.GroupId));
			Assert.That(trustDetailDto.Gias.GroupName, Is.EqualTo(expectedTrust.Gias.GroupName));
			Assert.That(trustDetailDto.Gias.UkPrn, Is.EqualTo(expectedTrust.Gias.UkPrn));
			Assert.That(trustDetailDto.Gias.CompaniesHouseNumber, Is.EqualTo(expectedTrust.Gias.CompaniesHouseNumber));
			Assert.That(trustDetailDto.Gias.GroupContactAddress, Is.Not.Null);
			Assert.That(trustDetailDto.Gias.GroupContactAddress.County, Is.EqualTo(expectedTrust.Gias.GroupContactAddress.County));
			Assert.That(trustDetailDto.Gias.GroupContactAddress.Locality, Is.EqualTo(expectedTrust.Gias.GroupContactAddress.Locality));
			Assert.That(trustDetailDto.Gias.GroupContactAddress.Postcode, Is.EqualTo(expectedTrust.Gias.GroupContactAddress.Postcode));
			Assert.That(trustDetailDto.Gias.GroupContactAddress.Street, Is.EqualTo(expectedTrust.Gias.GroupContactAddress.Street));
			Assert.That(trustDetailDto.Gias.GroupContactAddress.Town, Is.EqualTo(expectedTrust.Gias.GroupContactAddress.Town));
			Assert.That(trustDetailDto.Gias.GroupContactAddress.AdditionalLine, Is.EqualTo(expectedTrust.Gias.GroupContactAddress.AdditionalLine));
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