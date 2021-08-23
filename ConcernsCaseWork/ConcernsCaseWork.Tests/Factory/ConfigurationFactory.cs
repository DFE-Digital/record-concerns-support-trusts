using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Factory
{
	internal static class ConfigurationFactory
	{
		public static IConfigurationRoot ConfigurationBuilder(Dictionary<string, string> initialData)
		{
			return new ConfigurationBuilder().AddInMemoryCollection(initialData).AddEnvironmentVariables().Build();
		}
		
		public static IConfigurationRoot ConfigurationUserSecretsBuilder()
		{
			return new ConfigurationBuilder().AddUserSecrets<Startup>().Build();
		}
	}
}