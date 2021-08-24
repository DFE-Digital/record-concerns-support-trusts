using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class ConfigurationFactory
	{
		public static IConfigurationRoot ConfigurationBuilder(Dictionary<string, string> initialData)
		{
			return new ConfigurationBuilder().AddInMemoryCollection(initialData).Build();
		}
		
		public static IConfigurationRoot ConfigurationUserSecretsBuilder()
		{
			return new ConfigurationBuilder().AddUserSecrets<Startup>().Build();
		}
	}
}