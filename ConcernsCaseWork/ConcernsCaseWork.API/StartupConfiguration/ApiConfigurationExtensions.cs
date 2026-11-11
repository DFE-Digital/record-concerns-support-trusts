using System.Net.Mime;
using System.Net.Http;
using System.Net.Security;

namespace ConcernsCaseWork.API.StartupConfiguration;

public static class ApiConfigurationExtensions
{
	public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
	{
		var concernsApiEndpoint = configuration["ConcernsCasework:ApiEndpoint"];
		var concernsApiKey = configuration["ConcernsCasework:ApiKey"];

		if (string.IsNullOrEmpty(concernsApiEndpoint) || string.IsNullOrEmpty(concernsApiKey))
			throw new InvalidOperationException("AddConcernsApi::missing configuration");

		var concernsApiUri = new Uri(concernsApiEndpoint);
		var isDevelopment = string.Equals(configuration["ASPNETCORE_ENVIRONMENT"], "Development", StringComparison.OrdinalIgnoreCase);
		var isLocalHttps = IsLocalHttpsEndpoint(concernsApiUri);
		var allowLocalDevChainErrors = isDevelopment && isLocalHttps;

		services.AddHttpClient("ConcernsClient", config =>
		{
			config.BaseAddress = concernsApiUri;
			config.DefaultRequestHeaders.Add("ApiKey", concernsApiKey);
			config.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);
		})
		.ConfigurePrimaryHttpMessageHandler(() => CreateConcernsClientHandler(allowLocalDevChainErrors));

		services.AddControllers();

		services.AddApiVersioning(config =>
		{
			config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
			config.AssumeDefaultVersionWhenUnspecified = true;
			config.ReportApiVersions = true;
		});
		services.AddVersionedApiExplorer(setup =>
		{
			// ReSharper disable once StringLiteralTypo
			setup.GroupNameFormat = "'v'VVV";
			setup.SubstituteApiVersionInUrl = true;
		});

		services.AddSwaggerGen();
		services.ConfigureOptions<SwaggerOptions>();

		return services;
	}

	private static bool IsLocalHttpsEndpoint(Uri concernsApiUri)
	{
		return string.Equals(concernsApiUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
			&& IsLocalHost(concernsApiUri.Host);
	}

	private static bool IsLocalHost(string host)
	{
		return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
			|| string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
	}

	private static HttpMessageHandler CreateConcernsClientHandler(bool allowLocalDevChainErrors)
	{
		var handler = new HttpClientHandler();
		if (!allowLocalDevChainErrors)
		{
			return handler;
		}

		// Local docker uses a self-signed cert for localhost. In Development only,
		// allow chain trust errors for localhost requests while still validating host.
		handler.ServerCertificateCustomValidationCallback = (request, _, _, sslErrors) =>
		{
			if (sslErrors == SslPolicyErrors.None)
			{
				return true;
			}

			if (request?.RequestUri == null || !IsLocalHost(request.RequestUri.Host))
			{
				return false;
			}

			return sslErrors == SslPolicyErrors.RemoteCertificateChainErrors;
		};

		return handler;
	}
	
	public static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
	{
		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		return app;
	}
}