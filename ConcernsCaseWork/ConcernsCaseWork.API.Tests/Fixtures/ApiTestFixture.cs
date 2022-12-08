using AutoFixture;
using ConcernsCaseWork.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;

namespace ConcernsCaseWork.API.Tests.Fixtures
{
	public class ApiTestFixture : IDisposable
	{
		private WebApplicationFactory<Startup> _application;

		public HttpClient Client { get; private set; }

		public ConcernsDbContext DbContext { get; private set; }
		
		private DbContextOptions<ConcernsDbContext> DbContextOptions { get; init; }

		public ApiTestFixture()
		{
			IConfigurationRoot testConfig = null;

			_application = new WebApplicationFactory<Startup>()
				.WithWebHostBuilder(builder =>
				{
					var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json");

					builder.ConfigureAppConfiguration((context, config) =>
					{
						config.AddJsonFile(configPath);

						testConfig = config.Build();
					});
				});

			Client = _application.CreateClient();
			Client.DefaultRequestHeaders.Add("ApiKey", "app-key");

			var connection = testConfig.GetSection("ConnectionStrings")["DefaultConnection"];

			DbContextOptions = new DbContextOptionsBuilder<ConcernsDbContext>()
				.UseSqlServer(connection)
				.Options;

			DbContext = GetContext();
			DbContext.Database.Migrate();
		}

		public ConcernsDbContext GetContext() => new ConcernsDbContext(DbContextOptions);

		public void Dispose()
		{
			_application.Dispose();
			Client.Dispose();
			DbContext.Dispose();
		}
	}
}
