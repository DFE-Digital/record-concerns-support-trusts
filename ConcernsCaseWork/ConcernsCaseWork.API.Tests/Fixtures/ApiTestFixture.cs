using AutoFixture;
using ConcernsCaseWork.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

		public ApiTestFixture()
		{
			_application = new WebApplicationFactory<Startup>()
				.WithWebHostBuilder(builder =>
				{
					var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json");

					builder.ConfigureAppConfiguration((context, config) =>
					{
						config.AddJsonFile(configPath);
					});
				});

			Client = _application.CreateClient();
			Client.DefaultRequestHeaders.Add("ConcernsApiKey", "app-key");

			var contextOptions = new DbContextOptionsBuilder<ConcernsDbContext>()
				.UseSqlServer("Server=localhost;Database=integrationtests;Integrated Security=true")
				.Options;

			DbContext = new ConcernsDbContext(contextOptions);
			DbContext.Database.Migrate();


		}
		public void Dispose()
		{
			_application.Dispose();
			Client.Dispose();
			DbContext.Dispose();
		}
	}
}
