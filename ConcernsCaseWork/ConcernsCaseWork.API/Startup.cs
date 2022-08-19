using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.API.Middleware;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
}
