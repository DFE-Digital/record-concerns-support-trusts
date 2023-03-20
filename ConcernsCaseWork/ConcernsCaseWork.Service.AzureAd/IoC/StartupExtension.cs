using ConcernsCaseWork.Service.AzureAd.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ConcernsCaseWork.Service.AzureAd.IoC;

public static class StartupExtension
{
	public static void AddAzureAdService(this IServiceCollection services)
	{
		services.AddSingleton<IAdUserService, AdUserService>();
		services.AddSingleton<IGraphClient, GraphClient>();
		services.AddSingleton<IGraphClientSettings, GraphClientSettings>();
		services.AddSingleton<IGraphGroupIdSettings, GraphGroupIdSettings>();
	}
}