using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ConcernsCaseWork.API.Extensions;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseConcernsCaseworkSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
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
	    
	public static IApplicationBuilder UseConcernsCaseworkEndpoints(this IApplicationBuilder app)
	{
		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

		return app;
	}
}