using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ConcernsCaseWork.API.StartupConfiguration;

public static class OpenApiConfigurationExtensions
{
	public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			foreach (var desc in provider.ApiVersionDescriptions)
			{
				c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
			}
                
			c.SupportedSubmitMethods(SubmitMethod.Get);
		});
    
		return app;
	}
	
	public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSwaggerGen();
		services.ConfigureOptions<SwaggerOptions>();
			
		return services;
	}
}