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
		
		private DbContextOptions<ConcernsDbContext> _dbContextOptions { get; init; }

		private readonly object _lock = new();
		private bool _isInitialised = false;

		public ApiTestFixture()
		{
			lock (_lock)
			{
				if (!_isInitialised)
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

					Console.WriteLine($"Logging the connection {connection}");

					_dbContextOptions = new DbContextOptionsBuilder<ConcernsDbContext>()
						.UseSqlServer(connection)
						.Options;

					using var context = GetContext();
					context.Database.Migrate();
					_isInitialised = true;
				}
			}
		}

		public ConcernsDbContext GetContext() => new ConcernsDbContext(_dbContextOptions);

		public void Dispose()
		{
			_application.Dispose();
			Client.Dispose();
		}
	}
}
