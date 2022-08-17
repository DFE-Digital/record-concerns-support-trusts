using Concerns.Data;
using Concerns.Data.Gateways;
using ConcernsCaseWork.API.UseCases;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace ConcernsCaseWork.API.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddConcernsApiProject(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDependencies();
			services.AddDatabase(configuration);
			services.AddApi(configuration);
			services.AddUseCases();

			return services;
		}
		
		public static IServiceCollection AddUseCases(this IServiceCollection services)
		{
			var allTypes = typeof(IUseCase<,>).Assembly.GetTypes();

			foreach (var type in allTypes)
			{
				foreach (var @interface in type.GetInterfaces())
				{
					if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IUseCase<,>))
					{
						services.AddScoped(@interface, type);
					}
				}
			}

			return services;
		}

		public static IServiceCollection AddDependencies(this IServiceCollection services)
		{
			// services.AddScoped<ISearchTrusts, SearchTrusts>();
			services.AddScoped<ICreateConcernsCase, CreateConcernsCase>();
			services.AddScoped<IConcernsCaseGateway, ConcernsCaseGateway>();
			services.AddScoped<IGetConcernsCaseByUrn, GetConcernsCaseByUrn>();
			services.AddScoped<IGetConcernsCaseByTrustUkprn, GetConcernsCaseByTrustUkprn>();
			services.AddScoped<IIndexConcernsStatuses, IndexConcernsStatuses>();
			services.AddScoped<IConcernsStatusGateway, ConcernsStatusGateway>();
			services.AddScoped<IConcernsRecordGateway, ConcernsRecordGateway>();
			services.AddScoped<ICreateConcernsRecord, CreateConcernsRecord>();
			services.AddScoped<IConcernsTypeGateway, ConcernsTypeGateway>();
			services.AddScoped<IConcernsRatingGateway, ConcernsRatingsGateway>();
			services.AddScoped<IIndexConcernsRatings, IndexConcernsRatings>();
			services.AddScoped<IUpdateConcernsCase, UpdateConcernsCase>();
			services.AddScoped<IIndexConcernsTypes, IndexConcernsTypes>();
			services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();

			services.AddScoped<IIndexConcernsMeansOfReferrals, IndexConcernsMeansOfReferrals>();
			services.AddScoped<IConcernsMeansOfReferralGateway, ConcernsMeansOfReferralGateway>();

			services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();

			services.AddScoped<IGetConcernsRecordsByCaseUrn, GetConcernsRecordsByCaseUrn>();
			services.AddScoped<IGetConcernsCasesByOwnerId, GetConcernsCasesByOwnerId>();

			services.AddScoped<ISRMAGateway, SRMAGateway>();
			services.AddScoped<IFinancialPlanGateway, FinancialPlanGateway>();
			services.AddScoped<INTIUnderConsiderationGateway, NTIUnderConsiderationGateway>();
			services.AddScoped<INTIWarningLetterGateway, NTIWarningLetterGateway>();

			return services;
		}

		public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ConcernsDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

			return services;
		}

		public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
		{
			var concernsApiEndpoint = configuration["concernsCaseWorkApi:ApiEndpoint"];

			if (string.IsNullOrEmpty(concernsApiEndpoint))
				throw new Exception("AddConcernsApi::missing configuration");

			services.AddHttpClient("ConcernsClient", client =>
			{
				client.BaseAddress = new Uri(concernsApiEndpoint);
				client.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);
			});
			
			services.AddControllers();
			services.AddApiVersioning(config => 
			{
				config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
				config.AssumeDefaultVersionWhenUnspecified = true;
				config.ReportApiVersions = true;
			});
			services.AddVersionedApiExplorer(setup =>
			{
				setup.GroupNameFormat = "'v'VVV";
				setup.SubstituteApiVersionInUrl = true;
			});
            
			services.AddSwaggerGen();
			services.ConfigureOptions<SwaggerOptions>();
			
			return services;
		}
	}
}
