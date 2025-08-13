using ConcernsCaseWork.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace ConcernsCaseWork.API.StartupConfiguration;

public static class DatabaseConfigurationExtensions
{
	public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		var isEnabled = configuration.GetValue<bool>("ConcernsCasework:EnableSQLRetryOnFailure", false);
		services.AddDbContext<ConcernsDbContext>(options =>
			options.UseConcernsSqlServer(connectionString, isEnabled)
		);
		services.AddHealthChecks()
			.AddDbContextCheck<ConcernsDbContext>("Concerns Database");

		return services;
	}
}
