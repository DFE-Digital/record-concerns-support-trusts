using ConcernsCaseWork.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Internal.Commands;

namespace ConcernsCaseWork.Data.Tests;

[TestFixture]
public class DatabaseTestFixture
{
	private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDb;Database=integrationtests;Trusted_Connection=True";

	private static readonly object _lock = new();
	private static bool _databaseInitialized;

	public DatabaseTestFixture()
	{
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

	public ConcernsDbContext CreateContext()
		=> new ConcernsDbContext(
			new DbContextOptionsBuilder<ConcernsDbContext>()
				.UseSqlServer(ConnectionString)
				.Options);
}