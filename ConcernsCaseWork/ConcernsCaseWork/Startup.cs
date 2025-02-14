using AutoMapper;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.API.Middleware;
using ConcernsCaseWork.Constraints;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Middleware;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.PageHistory;
using DfE.CoreLibs.Security.Authorization;
using DfE.CoreLibs.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using System;
using System.Threading;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using ConcernsCaseWork.API.Contracts.PolicyType;
using Microsoft.AspNetCore.Authentication;
using ConcernsCaseWork.Authorization;

namespace ConcernsCaseWork
{
	public class Startup(IConfiguration configuration)
	{
		private string _authenticationScheme = "ApiScheme";
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages(options =>
			{
				options.Conventions.AuthorizeFolder("/");
				options.Conventions.AllowAnonymousToPage("/AccessDenied");
				options.Conventions.AllowAnonymousToPage("/Maintenance");
				options.Conventions.AllowAnonymousToPage("/Accessibility");
				options.Conventions.AllowAnonymousToPage("/Cookies");
				options.Conventions.AllowAnonymousToPage("/PrivacyPolicy");
				options.Conventions.AllowAnonymousToPage("/NotFound");
				options.Conventions.AllowAnonymousToPage("/Error");
				options.Conventions.AddPageRoute("/home", "");
				options.Conventions.AddPageRoute("/notfound", "/error/404");
				options.Conventions.AddPageRoute("/notfound", "/error/{code:int}");
				options.Conventions.AddPageRoute("/case/management/action/NtiWarningLetter/add", "/case/{urn:long}/management/action/NtiWarningLetter/add");
				options.Conventions.AddPageRoute("/case/management/action/NtiWarningLetter/addConditions", "/case/{urn:long}/management/action/NtiWarningLetter/conditions");
				options.Conventions.AddPageRoute("/case/management/action/Nti/add", "/case/{urn:long}/management/action/nti/add");
				options.Conventions.AddPageRoute("/case/management/action/Nti/addConditions", "/case/{urn:long}/management/action/nti/conditions");


				// TODO:
				// Consider adding: options.Conventions.AuthorizeFolder("/");
			}).AddViewOptions(options =>
			{
				options.HtmlHelperOptions.ClientValidationEnabled = false;
			});

			services.AddFeatureManagement();

			// Configuration options
			services.AddConfigurationOptions(configuration);

			// Azure AD   
			services.AddUserTokenService(configuration);
			services.AddApplicationAuthorization(configuration);
			var authenticationBuilder = services.AddAuthentication(options =>
			{
				options.DefaultScheme = "MultiAuth";
				options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			})
			.AddPolicyScheme("MultiAuth", "Multi Auth", options =>
			{
				options.ForwardDefaultSelector = context =>
				{
					var hasCypressHeaders =
						context.Request.Headers.ContainsKey("x-user-context-role-0") &&
						context.Request.Headers.ContainsKey("x-user-context-name");

					if (hasCypressHeaders)
					{
						return "CypressAuth";
					}

					// Otherwise, use cookies
					return CookieAuthenticationDefaults.AuthenticationScheme;
				};
			})
			.AddScheme<AuthenticationSchemeOptions, CypressAuthenticationHandler>("CypressAuth", null);

			authenticationBuilder.AddMicrosoftIdentityWebApp(configuration);
			services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
				options =>
				{
					options.Cookie.Name = ".ConcernsCasework.Login";
					options.Cookie.HttpOnly = true;
					options.Cookie.IsEssential = true;
					options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration["AuthenticationExpirationInMinutes"]));
					options.SlidingExpiration = true;
					options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // in A2B this was only if string.IsNullOrEmpty(Configuration["CI"]), but why not always?
					options.AccessDeniedPath = "/access-denied";
				});
			services.AddCustomJwtAuthentication(configuration, _authenticationScheme, authenticationBuilder);
			services.AddAntiforgery(options =>
			{
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});

			// Redis
			services.AddRedis(configuration);

			// APIs
			services.AddTramsApi(configuration);
			services.AddConcernsApi(configuration);

			// AutoMapper
			services.ConfigureAndAddAutoMapper();

			// Route options
			services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

			// Internal Service
			services.AddInternalServices();

			// Session
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromHours(24);
				options.Cookie.Name = ".ConcernsCasework.Session";
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});

			services.AddRouting(options =>
			{
				options.ConstraintMap.Add("fpEditModes", typeof(FinancialPlanEditModeConstraint));
			});
			services.AddApplicationInsightsTelemetry(options =>
			{
				options.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
			});
			// Enforce HTTPS in ASP.NET Core
			// @link https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?
			services.AddHsts(options =>
			{
				options.Preload = true;
				options.IncludeSubDomains = true;
				options.MaxAge = TimeSpan.FromDays(365);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, IMapper mapper)
		{
			// Ensure we do not lose X-Forwarded-* Headers when behind a Proxy
			var forwardOptions = new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.All,
				RequireHeaderSymmetry = false
			};
			forwardOptions.KnownNetworks.Clear();
			forwardOptions.KnownProxies.Clear();
			app.UseForwardedHeaders(forwardOptions);

			AbstractPageModel.PageHistoryStorageHandler = app.ApplicationServices.GetService<IPageHistoryStorageHandler>();

			app.UseConcernsCaseworkSwagger(provider);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseMiddleware<ExceptionHandlerMiddleware>();

			// Security headers
			app.UseSecurityHeaders(
				SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));
			app.UseHsts();

			// Combined with razor routing 404 display custom page NotFound
			app.UseStatusCodePagesWithReExecute("/error/{0}");

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			// Enable session for the application
			app.UseSession();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseMiddleware<UserContextMiddleware>();
			app.UseMiddleware<CorrelationIdMiddleware>();
			app.UseMiddleware<NavigationHistoryMiddleware>();
			app.UseMiddleware<UserContextReceiverMiddleware>();
			app.UseMiddlewareForFeature<MaintenanceModeMiddleware>(FeatureFlags.IsMaintenanceModeEnabled);

			app.UseConcernsCaseworkEndpoints();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers().RequireAuthorization(new AuthorizeAttribute
				{
					AuthenticationSchemes = _authenticationScheme,
					Policy = Policy.Default
				});
				endpoints.MapRazorPages()
					.RequireAuthorization(new AuthorizeAttribute
					{
						Policy = Policy.Default
					});
			});

			mapper.CompileAndValidate();

			// If our application gets hit really hard, then threads need to be spawned
			// By default the number of threads that exist in the threadpool is the amount of CPUs (1)
			// Each time we have to spawn a new thread it gets delayed by 500ms
			// Setting the min higher means there will not be that delay in creating threads up to the min
			// Re-evaluate this based on performance tests
			// Found because redis kept timing out because it was delayed too long waiting for a thread to execute
			ThreadPool.SetMinThreads(400, 400);

			// Add Health Checks
			app.UseHealthChecks("/health");
		}
	}
}
