using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConcernsCaseWork.Data.Tests;

public class DatabaseTestFixture
{
	private readonly string _connectionString;

	private static readonly object _lock = new();
	private static bool _databaseInitialized;
	
	protected TestDbGateway TestDbGateway => new ();

	protected DatabaseTestFixture()
	{
		var configPath = Path.Combine(
			Directory.GetCurrentDirectory(), "appsettings.tests.json");

		var config = new ConfigurationBuilder()
			.AddJsonFile(configPath)
			.Build();

		_connectionString = config.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found");
		
		lock (_lock)
		{
			if (!_databaseInitialized)
			{
				using (var context = CreateContext())
				{
					context.Database.EnsureDeleted();
					context.Database.Migrate();
				}

				_databaseInitialized = true;
			}
		}
	}

	protected ConcernsDbContext CreateContext()
		=> new ConcernsDbContext(
			new DbContextOptionsBuilder<ConcernsDbContext>()
				.UseSqlServer(_connectionString)
				.Options);
}