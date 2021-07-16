using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.TRAMS.Cases;
using System;
using System.Net.Mime;

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
            services.AddRazorPages();
            services.Configure<RazorViewEngineOptions>(options =>
            {
	            options.PageViewLocationFormats.Add("/Pages/Partials/{0}" + RazorViewEngine.ViewExtension);
            });
            
            // HttpFactory for TRAMS API.
            services.AddHttpClient("TramsClient", client =>
            {
	            var apiKey = Configuration["TramsApi:ApiKey"];
	            var endpoint = Configuration["TramsApi:Endpoint"];
	            
	            client.BaseAddress = new Uri(endpoint);
	            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
	            client.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);
            });
            
            // AutoMapper.
            services.AddAutoMapper(typeof(Startup));
            
            // Cases service model and external TRAMS.
            services.AddSingleton<ICaseModelService, CaseModelService>();
            services.AddSingleton<ICaseService, CaseService>();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
