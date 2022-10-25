﻿using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Service.TRAMS.Base;
using Service.TRAMS.Decision;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Tests.Decision
{
	public class DecisionServiceTests
	{
		[Test]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			Fixture fixture = CreateMockedFixture();
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(DecisionService).GetConstructors());
		}

		[Test]
		public void DecisionService_Implements_IDecisionService()
		{
		Fixture fixture = CreateMockedFixture();
			fixture.Register(() => Mock.Of<IHttpClientFactory>());
			fixture.Register(() => Mock.Of<ILogger<DecisionService>>());

			var sut = fixture.Create<DecisionService>();
			Assert.That(sut, Is.AssignableTo<IDecisionService>());
		}

		[Test]
		public void DecisionService_IsAm_AbstractService()
		{
			Fixture fixture = CreateMockedFixture();

			var sut = fixture.Create<DecisionService>();
			Assert.That(sut, Is.AssignableTo<AbstractService>());
		}

		private Fixture CreateMockedFixture()
		{
			var fixture = new Fixture();
			fixture.Register(() => Mock.Of<IHttpClientFactory>());
			fixture.Register(() => Mock.Of<ILogger<DecisionService>>());
			return fixture;
		}

		[Test]
		public async Task DecisionService_PostDecision_When_Success_Returns_Response()
		{
			var httpClientFactory = new Mock<IHttpClientFactory>();
			var mockMessageHandler = new Mock<HttpMessageHandler>();
			
			Fixture fixture = CreateMockedFixture();
			fixture.Register(() => Mock.Get(httpClientFactory));
			fixture.Register(() => Mock.Get(mockMessageHandler));

			var expectedInputDto = fixture.Create<CreateDecisionDto>();
			var expectedResponseDto = fixture.Create<CreateDecisionResponseDto>();
			var responseWrapper = new ApiWrapper<CreateDecisionResponseDto>(expectedResponseDto);

			var configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			var tramsApiEndpoint = configuration["trams:api_endpoint"];

			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(responseWrapper))
				});

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(tramsApiEndpoint);
			httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new DecisionService(httpClientFactory.Object, Mock.Of<ILogger<DecisionService>>());
			var result = await sut.PostDecision(expectedInputDto);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.ConcernsCaseUrn, Is.EqualTo(expectedResponseDto.ConcernsCaseUrn));
			Assert.That(result.DecisionId, Is.EqualTo(expectedResponseDto.DecisionId));
		}
	}
}
