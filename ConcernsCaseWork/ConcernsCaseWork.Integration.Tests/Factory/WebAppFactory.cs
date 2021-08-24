using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Integration.Tests.Factory
{
	public class WebAppFactory : WebApplicationFactory<Startup>
	{
		private IConfigurationRoot Configuration { get; }

		public WebAppFactory(IConfigurationRoot configuration)
		{
			Configuration = configuration;
		}
		
		protected override IWebHostBuilder CreateWebHostBuilder()
		{
			return WebHost.CreateDefaultBuilder()
				.UseStartup<Startup>();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseContentRoot(".");
			base.ConfigureWebHost(builder);
			builder.UseConfiguration(Configuration);

			builder.ConfigureTestServices(services =>
			{
				// Setup your mocks here
			});
		}
	}
}