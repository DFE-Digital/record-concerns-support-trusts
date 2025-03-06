using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConcernsCaseWork.API.StartupConfiguration
{
    public class SwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        private const string _authorisationName = "Authorization";
		private const string _serviceTitle = "Record concerns and support for trusts API";
        private const string _serviceDescription = "Record concerns and support for trusts API";
        private const string _contactName = "Support";
        private const string _contactEmail = "servicedelivery.rdd@education.gov.uk";
		private const string _securityScheme = "bearer";
		private const string _securityFormat = "JWT";
		private const string _securitySchemeDescription = "A valid Token in the 'Authorization' header is required to " +
														 "access the Record concerns and support for trusts API.";
        private const string _deprecatedMessage = "- API version has been deprecated.";
        
        public SwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;
        
        public void Configure(string name, SwaggerGenOptions options) => Configure(options);
        
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var desc in _provider.ApiVersionDescriptions)
            {
                var openApiInfo = new OpenApiInfo
                {
                    Title = _serviceTitle,
                    Description = _serviceDescription,
                    Contact = new OpenApiContact
                    {
                        Name = _contactName,
                        Email = _contactEmail 
                    },
                    Version = desc.ApiVersion.ToString()
                };
                if (desc.IsDeprecated) openApiInfo.Description += _deprecatedMessage;
                
                options.SwaggerDoc(desc.GroupName, openApiInfo);
            }
            
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = _authorisationName,
                Description = _securitySchemeDescription,
                Type = SecuritySchemeType.Http,
				Scheme = _securityScheme,
				BearerFormat = _securityFormat,
				In = ParameterLocation.Header
            };
            options.AddSecurityDefinition(_authorisationName, securityScheme);
            options.OperationFilter<AuthenticationHeaderOperationFilter>();
            options.OperationFilter<UserContextOperationFilter>();
        }
    }
}