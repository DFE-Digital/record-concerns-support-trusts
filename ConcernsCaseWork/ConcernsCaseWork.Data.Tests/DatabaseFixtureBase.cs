using ConcernsCaseWork.UserContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Data.Tests;

public class DatabaseTestFixture
{
	private readonly string? _connectionString;

	private static readonly object _lock = new();
	private static bool _databaseInitialized;

	public DatabaseTestFixture(IEnumerable<IInterceptor>? interceptors = null)
	{
		var configPath = Path.Combine(
		Directory.GetCurrentDirectory(), "appsettings.tests.json");

		var config = new ConfigurationBuilder()
			.AddJsonFile(configPath)
			.AddEnvironmentVariables();

		_connectionString = BuildDatabaseConnectionString(config);

		lock (_lock)
		{
			if (!_databaseInitialized)
			{
				using (var context = CreateContext(interceptors))
				{
					context.Database.EnsureDeleted();
					context.Database.Migrate();
				}

				_databaseInitialized = true;
			}
		}
	}

	private static string BuildDatabaseConnectionString(IConfigurationBuilder configBuilder)
	{
		var currentConfig = configBuilder.Build();

		var connection = currentConfig.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found");

		var sqlBuilder = new SqlConnectionStringBuilder(connection);
		sqlBuilder.InitialCatalog = "DataTests";

		var result = sqlBuilder.ToString();

		return result;
	}

	public ConcernsDbContext CreateContext(IEnumerable<IInterceptor>? interceptors = null)
	{
		var userInfoService = new ServerUserInfoService()
		{
			UserInfo = new UserInfo() { Name = "Data.Tests@test.gov.uk", Roles = new[] { Claims.CaseWorkerRoleClaim } }
		};

		return new ConcernsDbContext(new DbContextOptionsBuilder<ConcernsDbContext>().UseSqlServer(_connectionString).Options, userInfoService, interceptors);
	}
}