using AutoFixture;
using ConcernsCaseWork.Authorization;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Authorization
{
	public class AutomationHandlerTests
	{
		private Fixture _fixture;

		public AutomationHandlerTests() {
			_fixture = new Fixture();
		}

		[TestCaseSource(nameof(GetEnvironmentTestCases))]
		public void ValidateHostEnviroment(string environment, bool expected)
		{
			IHostEnvironment hostEnvironment = new HostingEnvironment()
			{
				EnvironmentName = environment
			};

			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers.Add(HeaderNames.Authorization, "Bearer 123");

			Mock<IHttpContextAccessor> mockHttpAccessor = new Mock<IHttpContextAccessor>();
			mockHttpAccessor.Setup(m => m.HttpContext).Returns(httpContext);

			var configurationSettings = new Dictionary<string, string>()
			{
				{ "CypressTestSecret", "123" }
			};

            IConfiguration configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(configurationSettings)
				.Build();

			var result = AutomationHandler.ClientSecretHeaderValid(hostEnvironment, mockHttpAccessor.Object, configuration);

			result.Should().Be(expected);
		}

		[TestCaseSource(nameof(GetAuthKeyTestCases))]
		public void ValidateAuthKey(string headerAuthKey, string serverAuthKey)
		{
			IHostEnvironment hostEnvironment = new HostingEnvironment()
			{
				EnvironmentName = Environments.Development
			};

			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers.Add(HeaderNames.Authorization, $"Bearer {headerAuthKey}");

			Mock<IHttpContextAccessor> mockHttpAccessor = new Mock<IHttpContextAccessor>();
			mockHttpAccessor.Setup(m => m.HttpContext).Returns(httpContext);

			var configurationSettings = new Dictionary<string, string>()
			{
				{ "CypressTestSecret", serverAuthKey }
			};

			IConfiguration configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(configurationSettings)
				.Build();

			var result = AutomationHandler.ClientSecretHeaderValid(hostEnvironment, mockHttpAccessor.Object, configuration);

			result.Should().BeFalse();
		}

		private static IEnumerable<TestCaseData> GetEnvironmentTestCases()
		{
			yield return new TestCaseData(Environments.Development, true);
			yield return new TestCaseData(Environments.Staging, true);
			yield return new TestCaseData(Environments.Production, false);
		}

		private static IEnumerable<TestCaseData> GetAuthKeyTestCases()
		{
			yield return new TestCaseData(null, null);
			yield return new TestCaseData("123", null);
			yield return new TestCaseData("", "");
			yield return new TestCaseData("123", "456");
		}
	}
}
