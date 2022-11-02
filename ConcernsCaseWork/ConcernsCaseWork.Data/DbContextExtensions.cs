using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data;

public static class DbContextExtensions
{
	    
	public static DbContextOptionsBuilder UseConcernsSqlServer(this DbContextOptionsBuilder optionsBuilder, string connectionString)
	{
		optionsBuilder.UseSqlServer(
			connectionString,
			opt => opt.MigrationsHistoryTable("__EfMigrationsHistory", "concerns"));
		return optionsBuilder;
	}
}