using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.API.Middleware;
using ConcernsCaseWork.Constraints;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Middleware;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Security;
using ConcernsCaseWork.Services.PageHistory;
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
using Microsoft.Identity.Web;
using System;
using System.Security.Claims;

namespace ConcernsCaseWork
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		private IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages(options =>
			{
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

			// Configuration options
			services.AddConfigurationOptions(Configuration);

			// Azure AD
			services.AddAuthorization(options => { options.DefaultPolicy = SetupAuthorizationPolicyBuilder().Build(); });
			services.AddMicrosoftIdentityWebAppAuthentication(Configuration);
			services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
				options =>
				{
					options.Cookie.Name = ".ConcernsCasework.Login";
					options.Cookie.HttpOnly = true;
					options.Cookie.IsEssential = true;
					options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration["AuthenticationExpirationInMinutes"]));
					options.SlidingExpiration = true;
					options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // in A2B this was only if string.IsNullOrEmpty(Configuration["CI"]), but why not always?
					options.AccessDeniedPath = "/access-denied";
				});

			// Redis
			services.AddRedis(Configuration);

			// APIs
			services.AddTramsApi(Configuration);
			services.AddConcernsApi(Configuration);

			// AutoMapper
			services.AddAutoMapper(typeof(Startup));

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
			});

			// Authentication

			//services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
			//{
			//	options.LoginPath = "/login";
			//	options.Cookie.Name = ".ConcernsCasework.Login";
			//	options.Cookie.HttpOnly = true;
			//	options.Cookie.IsEssential = true;
			//});

			services.AddRouting(options =>
			{
				options.ConstraintMap.Add("fpEditModes", typeof(FinancialPlanEditModeConstraint));
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
		{
			AbstractPageModel.PageHistoryStorageHandler = app.ApplicationServices.GetService<IPageHistoryStorageHandler>();

			app.UseConcernsCaseworkSwagger(provider);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseMiddleware<ExceptionHandlerMiddleware>();
			app.UseMiddleware<ApiKeyMiddleware>();

			// Security headers
			app.UseSecurityHeaders(
				SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));

			// Combined with razor routing 404 display custom page NotFound
			app.UseStatusCodePagesWithReExecute("/error/{0}");

			app.UseHttpsRedirection();

			//For Azure AD redirect uri to remain https
			var forwardOptions = new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.All,
				RequireHeaderSymmetry = false
			};
			forwardOptions.KnownNetworks.Clear();
			forwardOptions.KnownProxies.Clear();
			app.UseForwardedHeaders(forwardOptions);

			app.UseStaticFiles();

			// Enable session for the application
			app.UseSession();

			app.UseRouting();

			// Enable Sentry middleware for performance monitoring
			app.UseSentryTracing();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseMiddleware<CorrelationIdMiddleware>();
			app.UseMiddleware<NavigationHistoryMiddleware>();
			app.UseMiddleware<UserContextMiddleware>();

			app.UseConcernsCaseworkEndpoints();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
			});
		}

		/// <summary>
		/// Builds Authorization policy
		/// Ensure authenticated user and restrict roles if they are provided in configuration
		/// </summary>
		/// <returns>AuthorizationPolicyBuilder</returns>
		private AuthorizationPolicyBuilder SetupAuthorizationPolicyBuilder()
		{
			var policyBuilder = new AuthorizationPolicyBuilder();
			var allowedRoles = Configuration.GetSection("AzureAd")["AllowedRoles"];
			policyBuilder.RequireAuthenticatedUser();
			// Allows us to add in role support later.
			if (!string.IsNullOrWhiteSpace(allowedRoles))
			{
				policyBuilder.RequireClaim(ClaimTypes.Role, allowedRoles.Split(','));
			}
			return policyBuilder;
		}
	}
}
