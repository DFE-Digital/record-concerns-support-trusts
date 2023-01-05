using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ConcernsCaseWork.API.Tests
{
    public class ConcernsDataApiFactory : WebApplicationFactory<Startup>
    {
        private readonly DbFixture _dbFixture;
        
        public ConcernsDataApiFactory(DbFixture dbFixture) => _dbFixture = dbFixture;
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
	        var configPath = Path.Combine(
		        Directory.GetCurrentDirectory(), "appsettings.tests.json");
	        
	        builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((context, config) =>
            {
	            config.AddJsonFile(configPath);
            });
        }
    }
}