using AutoMapper;
using ConcernsCaseWork.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConcernsCaseWork.Extensions
{
	/// <summary>
	/// Abstracts the steps necessarity to configure automapper in the way we want, which is that the mappings have been validated as being correct early rather than lazily fail. Should be caught by unit tests anyway.
	/// </summary>
	public static class AddAutomapperStartupExtension
	{
		public static void ConfigureAndAddAutoMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(Startup));
		}

		public static void CompileAndValidate(this IMapper mapper)
		{
			mapper.ConfigurationProvider.CompileMappings();
			mapper.ConfigurationProvider.AssertConfigurationIsValid();
		}
	}
}
