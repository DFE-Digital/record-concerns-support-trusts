﻿using ConcernsCasework.Service.Base;
using ConcernsCasework.Service.MeansOfReferral;
using ConcernsCasework.Service.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.MeansOfReferral
{
	[Parallelizable(ParallelScope.All)]
	public class MeansOfReferralServiceTests
	{
		[Test]
		public async Task WhenGetMeansOfReferral_ReturnsMeansOfReferral()
		{
			// arrange
			var expectedMeansOfReferral = TypeFactory.BuildListTypeDto();
			var apiListWrapperMeansOfReferral = new ApiListWrapper<TypeDto>(expectedMeansOfReferral, null);
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];

			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiListWrapperMeansOfReferral))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<MeansOfReferralService>>();
			var service = new MeansOfReferralService(httpClientFactory.Object, logger.Object);
			
			// act
			var meansOfReferral = await service.GetMeansOfReferrals();

			// assert
			Assert.That(meansOfReferral, Is.Not.Null);
			Assert.That(meansOfReferral.Count, Is.EqualTo(expectedMeansOfReferral.Count));

			foreach (var actualMoR in meansOfReferral)
			{
				foreach (var expectedMoR in expectedMeansOfReferral.Where(expectedMoR => actualMoR.Urn.CompareTo(expectedMoR.Urn) == 0))
				{
					Assert.That(actualMoR.Name, Is.EqualTo(expectedMoR.Name));
					Assert.That(actualMoR.Description, Is.EqualTo(expectedMoR.Description));
					Assert.That(actualMoR.Urn, Is.EqualTo(expectedMoR.Urn));
				}
			}
		}
		
		[Test]
		public void WhenGetMeansOfReferral_ThrowsException()
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
			
			var logger = new Mock<ILogger<MeansOfReferralService>>();
			var typeService = new MeansOfReferralService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<HttpRequestException>(() => typeService.GetMeansOfReferrals());
		}
		
		[Test]
		public void WhenGetMeansOfReferral_And_ResponseData_IsNull_ThrowsException()
		{
			// arrange
			var apiListWrapperMeansOfReferral = new ApiListWrapper<TypeDto>(null, null);
			
			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];
			
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(apiListWrapperMeansOfReferral))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
			
			var logger = new Mock<ILogger<MeansOfReferralService>>();
			var typeService = new MeansOfReferralService(httpClientFactory.Object, logger.Object);
			
			// act / assert
			Assert.ThrowsAsync<Exception>(() => typeService.GetMeansOfReferrals());
		}
	}
}