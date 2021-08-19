using ConcernsCaseWork.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
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
	        services.AddRazorPages(options =>
            {
	            options.Conventions.AddPageRoute("/home", "");
	            options.Conventions.AddPageRoute("/notfound", "/error/404");
	            options.Conventions.AddPageRoute("/notfound", "/error/{code:int}");
	            
            }).AddViewOptions(options =>
            {
				options.HtmlHelperOptions.ClientValidationEnabled = false;
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
	            options.PageViewLocationFormats.Add($"/Pages/Partials/{RazorViewEngine.ViewExtension}");
            });
            
            // Azure AD
            // TODO
            
            // Redis
			services.AddRedis(Configuration);
			
            // TRAMS API.
            services.AddTramsApi(Configuration);
            
            // AutoMapper.
            services.AddAutoMapper(typeof(Startup));
            
            // Route options.
            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            
			// Internal Services
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

            // Combined with razor routing 404 display custom page NotFound
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
