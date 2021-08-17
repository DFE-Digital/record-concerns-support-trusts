using ConcernsCaseWork.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
            services.AddRazorPages().AddViewOptions(options =>
            {
				options.HtmlHelperOptions.ClientValidationEnabled = false;
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
	            options.PageViewLocationFormats.Add($"/Pages/Partials/{RazorViewEngine.ViewExtension}");
            });
            
            services.AddControllersWithViews(options => 
	            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
	        ).AddSessionStateTempDataProvider();
            
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
				options.LoginPath = "/home/login";
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/");
            });
        }
    }
}
