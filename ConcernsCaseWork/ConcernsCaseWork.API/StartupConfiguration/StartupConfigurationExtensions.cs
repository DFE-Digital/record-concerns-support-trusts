using AutoMapper;
using ConcernsCaseWork.API.UseCases;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ConcernsCaseWork.API.StartupConfiguration;

public static class StartupConfigurationExtensions
{
	public static IServiceCollection AddConcernsApiProject(this IServiceCollection services, IConfiguration configuration)
	{
		//services.AddAutoMapper(typeof(Startup));
		services.RegisterAutoMapperProfiles(typeof(Startup).GetTypeInfo().Assembly);

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());




		services.AddApiDependencies();
		services.AddDatabase(configuration);
		services.AddApi(configuration);
		services.AddUseCases();


		//services.AddUseCaseAsyncs(); //TODO: need to update decisions iuseasync to use the new DecisionUseCaseRequestWrapper

		return services;
	}
}