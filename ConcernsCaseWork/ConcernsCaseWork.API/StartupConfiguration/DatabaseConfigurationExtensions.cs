using ConcernsCaseWork.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace ConcernsCaseWork.API.StartupConfiguration;

public static class DatabaseConfigurationExtensions
{
	public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<ConcernsDbContext>(options =>
			options.UseConcernsSqlServer(connectionString)
		);
		services.AddHealthChecks()
			.AddDbContextCheck<ConcernsDbContext>("Concerns Database");

		return services;
	}
}
