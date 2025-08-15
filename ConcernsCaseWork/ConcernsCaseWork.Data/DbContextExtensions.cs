using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data;

public static class DbContextExtensions
{
	public static DbContextOptionsBuilder UseConcernsSqlServer(this DbContextOptionsBuilder optionsBuilder, string connectionString, bool enableRetryOnFailure = false)
	{
		optionsBuilder.UseSqlServer(
			connectionString,
			opt =>
			{
				opt.MigrationsHistoryTable("__EfMigrationsHistory", "concerns");
				if (enableRetryOnFailure)
				{
					opt.EnableRetryOnFailure(
						maxRetryCount: 2,
						maxRetryDelay: TimeSpan.FromSeconds(5),
						errorNumbersToAdd: null);
				}
			});
		return optionsBuilder;
	}
}