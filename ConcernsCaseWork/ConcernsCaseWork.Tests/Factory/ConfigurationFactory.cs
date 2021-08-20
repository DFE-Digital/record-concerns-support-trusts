using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Factory
{
	internal static class ConfigurationFactory
	{
		public static IConfigurationRoot LoginConfigurationBuilder()
		{
			var configuration = new Dictionary<string, string> { { "app:username", "username" }, { "app:password", "password" } };
			return new ConfigurationBuilder().AddInMemoryCollection(configuration).Build();
		}
		
		public static IConfigurationRoot LoginIntegrationConfigurationBuilder()
		{
			var configuration = new Dictionary<string, string>
			{
				{ "app:username", "username" }, { "app:password", "password" },
				{ "redis:local", "true" }, { "redis:host", "127.0.0.1" }, { "redis:password", "password" }, { "redis:port", "6379" },
				{ "trams:api_endpoint", "localhost" }, { "trams:api_key", "123" }
			};
			return new ConfigurationBuilder().AddInMemoryCollection(configuration).Build();
		}
	}
}