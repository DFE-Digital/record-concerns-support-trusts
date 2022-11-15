namespace ConcernsCaseWork.API.StartupConfiguration;

public static class StartupConfigurationExtensions
{
	public static IServiceCollection AddConcernsApiProject(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDependencies();
		services.AddDatabase(configuration);
		services.AddApi(configuration);
		services.AddUseCases();

		return services;
	}
}