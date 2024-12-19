using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.API.Middleware;
using ConcernsCaseWork.API.StartupConfiguration;
using ConcernsCaseWork.Middleware;
using DfE.CoreLibs.Security;
using DfE.CoreLibs.Security.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ConcernsCaseWork.API
{
    /// <summary>
    /// THIS STARTUP ISN'T USED WHEN API IS HOSTED THROUGH WEBSITE. It is used when running API tests
    /// </summary>
    public class Startup(IConfiguration configuration)
	{
		private string _authenticationScheme = "ApiScheme";
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddConcernsApiProject(configuration);
			var authenticationBuilder = services.AddAuthentication(options =>
			{
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			});
			services.AddApplicationAuthorization(configuration);
			services.AddCustomJwtAuthentication(configuration, _authenticationScheme, authenticationBuilder);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseConcernsCaseworkSwagger(provider);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>(); 
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMiddleware<UrlDecoderMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<UserContextReceiverMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers().RequireAuthorization(new AuthorizeAttribute
				{
					AuthenticationSchemes = _authenticationScheme,
					Policy = Policy.Default
				});
			});
			app.UseConcernsCaseworkEndpoints();

            // Add Health Checks
            app.UseHealthChecks("/health");
        }
    }
}
