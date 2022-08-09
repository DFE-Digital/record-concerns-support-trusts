using ConcernsCaseWork.Constraints;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
			services.AddMvc();

			services.AddRazorPages(options =>
			{
				options.Conventions.AddPageRoute("/home", "");
				options.Conventions.AddPageRoute("/notfound", "/error/404");
				options.Conventions.AddPageRoute("/notfound", "/error/{code:int}");
	
			}).AddViewOptions(options =>
			{
				options.HtmlHelperOptions.ClientValidationEnabled = false;
			});

			// Configuration options
			services.AddConfigurationOptions(Configuration);

			// Azure AD
			// TODO

			// Redis
			services.AddRedis(Configuration);

			// TRAMS API
			services.AddTramsApi(Configuration);

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
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
			{
				options.LoginPath = "/login";
				options.Cookie.Name = ".ConcernsCasework.Login";
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			services.AddRouting(options =>
			{
				options.ConstraintMap.Add("fpEditModes", typeof(FinancialPlanEditModeConstraint));
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
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

			// Security headers
			app.UseSecurityHeaders(
				SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));

			// Combined with razor routing 404 display custom page NotFound
			app.UseStatusCodePagesWithReExecute("/error/{0}");

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			// Enable session for the application
			app.UseSession();

			app.UseRouting();

			// Enable Sentry middleware for performance monitoring
			app.UseSentryTracing();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
			});
		}
	}
}
