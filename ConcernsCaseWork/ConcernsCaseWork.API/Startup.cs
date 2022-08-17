using Concerns.Data;
using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.API.Middleware;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using ExceptionHandlerMiddleware = ConcernsCaseWork.API.Middleware.ExceptionHandlerMiddleware;

namespace ConcernsCaseWork.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddConcernsApiProject(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        // {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
	        app.UseConcernsCaseworkSwagger(provider);
	        
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseMiddleware<UrlDecoderMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseConcernsCaseworkEndpoints();
        }
    }

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
    
    public class SwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
	    private readonly IApiVersionDescriptionProvider _provider;
        
	    private const string ServiceTitle = "Academies API";
	    private const string ServiceDescription = "The Academies API provides users with access to a variety of data " +
	                                              "regarding academies and trusts in England.\n\n" +
	                                              "The available data includes general data acadamy transfers and " +
	                                              "applications to become an academy and is compiled from a variety of internal and " +
	                                              "external services.";
	    private const string ContactName = "Support";
	    private const string ContactEmail = "servicedelivery.rdd@education.gov.uk";

	    private const string SecuritySchemeDescription = "A valid ApiKey in the 'ApiKey' header is required to " +
	                                                     "access the Academies API.";
	    private const string DepreciatedMessage = "- API version has been depreciated.";
        
	    public SwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;
        
	    public void Configure(string name, SwaggerGenOptions options) => Configure(options);

	    public void Configure(SwaggerGenOptions options)
	    {
		    foreach (var desc in _provider.ApiVersionDescriptions)
		    {
			    var openApiInfo = new OpenApiInfo
			    {
				    Title = ServiceTitle,
				    Description = ServiceDescription,
				    Contact = new OpenApiContact { Name = ContactName, Email = ContactEmail },
				    Version = desc.ApiVersion.ToString()
			    };
			    if (desc.IsDeprecated) openApiInfo.Description += DepreciatedMessage;

			    options.SwaggerDoc(desc.GroupName, openApiInfo);
		    }
	    }
    }
}
