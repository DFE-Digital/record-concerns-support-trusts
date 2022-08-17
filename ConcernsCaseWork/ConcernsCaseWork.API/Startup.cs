using Concerns.Data;
using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.API.Middleware;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
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
            services.AddControllers();
            //services.AddApiVersioning();
            
            // EF setup
            services.AddDbContext<ConcernsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
           // services.AddScoped<ISearchTrusts, SearchTrusts>();
            services.AddScoped<ICreateConcernsCase, CreateConcernsCase>();
            services.AddScoped<IConcernsCaseGateway, ConcernsCaseGateway>();
            services.AddScoped<IGetConcernsCaseByUrn, GetConcernsCaseByUrn>();
            services.AddScoped<IGetConcernsCaseByTrustUkprn, GetConcernsCaseByTrustUkprn>();
            services.AddScoped<IIndexConcernsStatuses, IndexConcernsStatuses>();
            services.AddScoped<IConcernsStatusGateway, ConcernsStatusGateway>();
            services.AddScoped<IConcernsRecordGateway, ConcernsRecordGateway>();
            services.AddScoped<ICreateConcernsRecord, CreateConcernsRecord>();
            services.AddScoped<IConcernsTypeGateway, ConcernsTypeGateway>();
            services.AddScoped<IConcernsRatingGateway, ConcernsRatingsGateway>();
            services.AddScoped<IIndexConcernsRatings, IndexConcernsRatings>();
            services.AddScoped<IUpdateConcernsCase, UpdateConcernsCase>();
            services.AddScoped<IIndexConcernsTypes, IndexConcernsTypes>();
            services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();
            
            services.AddScoped<IIndexConcernsMeansOfReferrals, IndexConcernsMeansOfReferrals>();
            services.AddScoped<IConcernsMeansOfReferralGateway, ConcernsMeansOfReferralGateway>();
            
            services.AddScoped<IUpdateConcernsRecord, UpdateConcernsRecord>();

            services.AddScoped<IGetConcernsRecordsByCaseUrn, GetConcernsRecordsByCaseUrn>();
            services.AddScoped<IGetConcernsCasesByOwnerId, GetConcernsCasesByOwnerId>();

            services.AddScoped<ISRMAGateway, SRMAGateway>();
            services.AddScoped<IFinancialPlanGateway, FinancialPlanGateway>();
            services.AddScoped<INTIUnderConsiderationGateway, NTIUnderConsiderationGateway>();
            services.AddScoped<INTIWarningLetterGateway, NTIWarningLetterGateway>();

            // services.AddVersionedApiExplorer(setup =>
            // {
            //     setup.GroupNameFormat = "'v'VVV";
            //     setup.SubstituteApiVersionInUrl = true;
            // });
            services.AddSwaggerGen();
            services.ConfigureOptions<SwaggerOptions>();
            services.AddUseCases();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        // {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SupportedSubmitMethods(SubmitMethod.Get);
            });

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

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
