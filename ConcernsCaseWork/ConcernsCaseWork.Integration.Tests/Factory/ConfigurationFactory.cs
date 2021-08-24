using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Integration.Tests.Factory
{
	internal static class ConfigurationFactory
	{
		public static IConfigurationRoot ConfigurationUserSecretsBuilder()
		{
			return new ConfigurationBuilder().AddUserSecrets<Startup>().Build();
		}
	}
}