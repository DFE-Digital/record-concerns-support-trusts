using DfE.CoreLibs.Testing.Authorization;
using Microsoft.AspNetCore.Routing; 
using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.Extensions.DependencyInjection;
using DfE.CoreLibs.Testing.Authorization.Helpers;

namespace ConcernsCaseWork.Tests.Security
{
	public class PageSecurityTests
	{
		private readonly AuthorizationTester _validator;
		private static readonly Lazy<IEnumerable<RouteEndpoint>> _endpoints = new(InitializeEndpoints);
		private const bool _globalAuthorizationEnabled = true;

		public PageSecurityTests()
		{
			_validator = new AuthorizationTester(_globalAuthorizationEnabled);
		}

		[Theory]
		[TestCaseSource(nameof(GetPageSecurityTestData))]
		public void ValidatePageSecurity(string route, string expectedSecurity)
		{
			var result = _validator.ValidatePageSecurity(route, expectedSecurity, _endpoints.Value);
			Assert.That(result.Message, Is.Null);
		}

		public static IEnumerable<object[]> GetPageSecurityTestData()
		{
			var configFilePath = "ExpectedSecurityConfig.json";
			var pages = EndpointTestDataProvider.GetPageSecurityTestDataFromFile(configFilePath, _endpoints.Value, _globalAuthorizationEnabled);
			return pages;
		}

		private static IEnumerable<RouteEndpoint> InitializeEndpoints()
		{
			// Using a temporary factory to access the EndpointDataSource for lazy initialization
			var factory = new CustomWebApplicationFactory<Startup>();
			var endpointDataSource = factory.Services.GetRequiredService<EndpointDataSource>();

			var endpoints = endpointDataSource.Endpoints
			   .OfType<RouteEndpoint>()
			   .DistinctBy(x=> x.DisplayName)
			   .Where(x => !x.RoutePattern.RawText.Contains("MicrosoftIdentity") && 
						   !x.DisplayName.Contains("ConcernsCaseWork.API.Features") &&
						   !x.RoutePattern.RawText.Equals("/") &&
						   !x.Metadata.Any(m => m is RouteNameMetadata && ((RouteNameMetadata)m).RouteName == "default"));

			return endpoints;
		}
	}
}
