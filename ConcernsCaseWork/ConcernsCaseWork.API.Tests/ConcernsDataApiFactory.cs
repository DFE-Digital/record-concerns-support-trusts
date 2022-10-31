using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.API.Tests
{
    public class ConcernsDataApiFactory : WebApplicationFactory<Startup>
    {
        private readonly DbFixture _dbFixture;
        
        public ConcernsDataApiFactory(DbFixture dbFixture) => _dbFixture = dbFixture;
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>(
                        "ConnectionStrings:DefaultConnection", _dbFixture.ConnString),
                    new KeyValuePair<string, string>(
                        "ApiKeys:0", "{\"userName\": \"Test User\", \"apiKey\": \"testing-api-key\"}"
                        ),
                });
            });
        }
    }
}