using ConcernsCaseWork.API.Middleware;
using ConcernsCaseWork.API.StartupConfiguration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ConcernsCaseWork.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddDependencies();
	        services.AddUseCases();
	        services.AddDatabase(Configuration);
	        services.AddApi(Configuration);
	        services.AddSwagger(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
	        app.UseSwagger(provider);
	        
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
            
            app.UseEndpoints();
        }
    }
}
