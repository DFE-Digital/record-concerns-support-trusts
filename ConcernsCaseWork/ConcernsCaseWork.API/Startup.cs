using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.API.Middleware;
using ConcernsCaseWork.API.StartupConfiguration;
using ConcernsCaseWork.Middleware;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ConcernsCaseWork.API
{
    /// <summary>
    /// THIS STARTUP ISN'T USED WHEN API IS HOSTED THROUGH WEBSITE. It is used when running API tests
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConcernsApiProject(Configuration);
        }

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
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<UserContextReceiverMiddleware>();

            // issue with docker communicating with https endpoints, disable redirection in development
            if (!env.IsDevelopment())
            {
	            app.UseHttpsRedirection();
            }

			app.UseRouting();

            app.UseAuthorization();

            app.UseConcernsCaseworkEndpoints();

            // Add Health Checks
            app.UseHealthChecks("/health");
        }
    }
}
