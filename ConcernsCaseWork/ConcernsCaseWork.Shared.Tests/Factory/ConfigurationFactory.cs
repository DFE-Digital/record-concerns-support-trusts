using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class ConfigurationFactory
	{
		public static IConfigurationBuilder ConfigurationInMemoryBuilder(this IConfigurationBuilder configurationBuilder, Dictionary<string, string> initialData)
		{
			return configurationBuilder.AddInMemoryCollection(initialData);
		}
		
		public static IConfigurationBuilder ConfigurationUserSecretsBuilder(this IConfigurationBuilder configurationBuilder)
		{
			return configurationBuilder.AddUserSecrets<Startup>();
		}
		
		public static IConfigurationBuilder ConfigurationJsonFile(this IConfigurationBuilder configurationBuilder)
		{
			return configurationBuilder.AddJsonFile("appsettings.json");
		}
	}
}