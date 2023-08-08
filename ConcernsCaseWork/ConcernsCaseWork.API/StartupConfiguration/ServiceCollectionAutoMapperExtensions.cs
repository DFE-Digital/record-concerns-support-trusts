using AutoMapper;
using System.Reflection;

namespace ConcernsCaseWork.API.StartupConfiguration
{
	public static class ServiceCollectionAutoMapperExtensions
	{
		public static IServiceCollection RegisterAutoMapperProfiles(this IServiceCollection services, Assembly asm)
		{
			var profiles =
				from t in asm.GetTypes()
				where typeof(Profile).IsAssignableFrom(t)
				select (Profile)Activator.CreateInstance(t);


			var config = new MapperConfiguration(config =>
			{
				foreach (var profile in profiles)
				{
					config.AddProfile(profile);
				}
			});

			services.AddSingleton<MapperConfiguration>(config);

			return services;
		}
	}
}
