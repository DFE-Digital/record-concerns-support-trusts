using System.Net.Mime;

namespace ConcernsCaseWork.API.StartupConfiguration;

public static class ApiConfigurationExtensions
{
	public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
	{
		var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
		var concernsApiKey = configuration["ConcernsCasework:ApiKey"];

		if (string.IsNullOrEmpty(concernsApiEndpoint) || string.IsNullOrEmpty(concernsApiKey))
			throw new Exception("AddConcernsApi::missing configuration");

		services.AddHttpClient("ConcernsClient", config =>
		{
			config.BaseAddress = new Uri(concernsApiEndpoint);
			config.DefaultRequestHeaders.Add("ApiKey", concernsApiKey);
			config.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);
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
			// ReSharper disable once StringLiteralTypo
			setup.GroupNameFormat = "'v'VVV";
			setup.SubstituteApiVersionInUrl = true;
		});

		services.AddSwaggerGen();
		services.ConfigureOptions<SwaggerOptions>();

		return services;
	}
	
	public static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
	{
		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		return app;
	}
}