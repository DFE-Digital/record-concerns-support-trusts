using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Tests;

[TestFixture]
public class DatabaseFixtureBase
{
	private readonly DbContextOptions<ConcernsDbContext> _optionsBuilder;
	public DatabaseFixtureBase()
	{
		var connection = new SqliteConnection("DataSource=:memory:");
		connection.Open();
 
		_optionsBuilder = new DbContextOptionsBuilder<ConcernsDbContext>().UseSqlite(connection).Options;

		using var context = new ConcernsDbContext(_optionsBuilder);
		context.Database.EnsureCreated();
	}

	protected ConcernsDbContext GetNewConcernsDbContext() => new (_optionsBuilder);
}