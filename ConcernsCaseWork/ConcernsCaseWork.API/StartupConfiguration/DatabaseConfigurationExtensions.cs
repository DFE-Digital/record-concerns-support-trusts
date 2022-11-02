using ConcernsCaseWork.Data;

namespace ConcernsCaseWork.API.StartupConfiguration;

public static class DatabaseConfigurationExtensions
{
	public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<ConcernsDbContext>(options =>
			options.UseConcernsSqlServer(connectionString)
		);
			
		return services;
	}
}