using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Context;
using ConcernsCaseWork.Service.Decision;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Tests.Decision
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
		public void DecisionService_IsAn_AbstractService()
		{
			Fixture fixture = CreateMockedFixture();

			var sut = fixture.Create<DecisionService>();
			Assert.That(sut, Is.AssignableTo<ConcernsAbstractService>());
		}

		private Fixture CreateMockedFixture()
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoMoqCustomization());
			return fixture;
		}

		[Test]
		public async Task DecisionService_PostDecision_When_Success_Returns_Response()
		{
			Fixture fixture = CreateMockedFixture();

			var expectedInputDto = fixture.Create<CreateDecisionRequest>();
			var expectedResponseDto = fixture.Create<CreateDecisionResponse>();
			var responseWrapper = new ApiWrapper<CreateDecisionResponse>(expectedResponseDto);

			var mockMessageHandler = SetupMessageHandler($"/concerns-cases/{expectedInputDto.ConcernsCaseUrn}/decisions", responseWrapper);
			var httpClientFactory = CreateHttpClientFactory(mockMessageHandler);

			var sut = new DecisionService(httpClientFactory.Object, Mock.Of<ILogger<DecisionService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IUserContextService>());
			var result = await sut.PostDecision(expectedInputDto);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.ConcernsCaseUrn, Is.EqualTo(expectedResponseDto.ConcernsCaseUrn));
			Assert.That(result.DecisionId, Is.EqualTo(expectedResponseDto.DecisionId));
		}

		[Test]
		public async Task DecisionService_GetDecisionsByUrn_WhenSuccess_ReturnsDecisionsByUrn()
		{
			var urn = 3;

			Fixture fixture = CreateMockedFixture();
			var expectedResponseDto = fixture.Create<List<DecisionSummaryResponse>>();
			var responseWrapper = new ApiWrapper<List<DecisionSummaryResponse>>(expectedResponseDto);

			var mockMessageHandler = SetupMessageHandler($"/concerns-cases/{urn}/decisions", responseWrapper);
			var httpClientFactory = CreateHttpClientFactory(mockMessageHandler);

			var sut = new DecisionService(httpClientFactory.Object, Mock.Of<ILogger<DecisionService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IUserContextService>());
			var result = await sut.GetDecisionsByCaseUrn(urn);

			result.Should().BeEquivalentTo(expectedResponseDto);
		}

		[Test]
		public async Task DecisionService_PutDecision_Returns_Success()
		{
			Fixture fixture = CreateMockedFixture();
			var request = fixture.Create<UpdateDecisionRequest>();
			var response = fixture.Create<UpdateDecisionResponse>();

			var responseWrapper = new ApiWrapper<UpdateDecisionResponse>(response);

			var mockMessageHandler = SetupMessageHandler($"/concerns-cases/{1}/decisions/{2}", responseWrapper);
			var httpClientFactory = CreateHttpClientFactory(mockMessageHandler);

			var sut = new DecisionService(httpClientFactory.Object, Mock.Of<ILogger<DecisionService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IUserContextService>());
			var result = await sut.PutDecision(1, 2, request);

			result.ConcernsCaseUrn.Should().Be(response.ConcernsCaseUrn);
			result.DecisionId.Should().Be(response.DecisionId);
		}

		[Test]
		public async Task DecisionService_PostDecisionOutcome_When_Success_Returns_Response()
		{
			Fixture fixture = CreateMockedFixture();

			
			var expectedInputDto = fixture.Create<CreateDecisionOutcomeRequest>();
			var expectedResponseDto = fixture.Create<CreateDecisionOutcomeResponse>();
			var responseWrapper = new ApiWrapper<CreateDecisionOutcomeResponse>(expectedResponseDto);
			var caseUrn = expectedResponseDto.ConcernsCaseUrn;
			var decisionId = expectedResponseDto.DecisionId;

			var mockMessageHandler = SetupMessageHandler($"/concerns-cases/{caseUrn}/decisions/{decisionId}", responseWrapper);
			var httpClientFactory = CreateHttpClientFactory(mockMessageHandler);

			var sut = new DecisionService(httpClientFactory.Object, Mock.Of<ILogger<DecisionService>>(), Mock.Of<ICorrelationContext>(), Mock.Of<IUserContextService>());
			var result = await sut.PostDecisionOutcome(caseUrn, decisionId, expectedInputDto);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.ConcernsCaseUrn, Is.EqualTo(expectedResponseDto.ConcernsCaseUrn));
			Assert.That(result.DecisionId, Is.EqualTo(expectedResponseDto.DecisionId));
			Assert.That(result.DecisionOutcomeId, Is.EqualTo(expectedResponseDto.DecisionOutcomeId));
		}

		private Mock<IHttpClientFactory> CreateHttpClientFactory(Mock<HttpMessageHandler> mockMessageHandler)
		{
			var concernsApiEndpoint = "https://localhost";

			var result = new Mock<IHttpClientFactory>();

			var httpClient = new HttpClient(mockMessageHandler.Object);
			httpClient.BaseAddress = new Uri(concernsApiEndpoint);
			result.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

			return result;
		}

		private Mock<HttpMessageHandler> SetupMessageHandler<T>(string url, T responseWrapper)
		{
			var result = new Mock<HttpMessageHandler>();

			result.Protected()
				.Setup<Task<HttpResponseMessage>>(
						"SendAsync",
					ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString().Contains(url)),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(responseWrapper))
				});

			return result;
		}
	}
}
