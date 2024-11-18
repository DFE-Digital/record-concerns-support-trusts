using Ardalis.GuardClauses;
using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using System.Linq;

public class UserContextMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<UserContextMiddleware> _logger;

    public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
    {
        _next = next;
        _logger = Guard.Against.Null(logger);
    }

	public Task Invoke(HttpContext httpContext, IClientUserInfoService userInfoService)
	{


		if (httpContext.User.Identity != null && IsPageRequest(httpContext.Request.Path))
		{

			userInfoService.SetPrincipal(httpContext.User);
			return _next(httpContext);
		}
		else
		{
			return _next(httpContext);
		}
	}

	private bool IsPageRequest(string path) => !path.StartsWith("/v2/");
}

//public static class ServiceCollectionExtensions
//{
//	/// <summary>
//	/// Registers the Token Service and its dependencies in the specified <see cref="IServiceCollection"/>.
//	/// </summary>
//	/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
//	/// <param name="configuration">The application configuration containing the role-to-scope mappings.</param>
//	/// <returns>The updated <see cref="IServiceCollection"/>.</returns>
//	public static IServiceCollection AddTokenService(this IServiceCollection services)
//	{
//		services.AddScoped<IUserTokenService, ApiTokenService>();
//		services.AddHttpContextAccessor();
//		return services;
//	}
//}

//public class ApiTokenService : IUserTokenService
//{
//	private readonly ITokenAcquisition _tokenAcquisition;
//	private readonly IHttpContextAccessor _httpContextAccessor;
//	private readonly IConfiguration _configuration;

//	/// <summary>
//	/// Initializes a new instance of the <see cref="ApiTokenService"/> class.
//	/// </summary>
//	/// <param name="tokenAcquisition">The token acquisition service for acquiring tokens.</param>
//	/// <param name="httpContextAccessor">Accessor for the current HTTP context, used to retrieve the user's claims.</param>
//	/// <param name="configuration">Configuration used to retrieve role-to-scope mappings.</param>
//	public ApiTokenService(ITokenAcquisition tokenAcquisition, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
//	{
//		_tokenAcquisition = tokenAcquisition;
//		_httpContextAccessor = httpContextAccessor;
//		_configuration = configuration;
//	}

//	/// <inheritdoc />
//	public async Task<string> GetApiOboTokenAsync()
//	{
//		var userRoles = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
//		if (userRoles == null || !userRoles.Any())
//		{
//			throw new UnauthorizedAccessException("User does not have any roles assigned.");
//		}

//		var apiClientId = _configuration["Authorization:ApiSettings:ApiClientId"];
//		if (string.IsNullOrWhiteSpace(apiClientId))
//		{
//			throw new InvalidOperationException("API client ID is missing from configuration.");
//		}

//		var scopeMappings = _configuration.GetSection("Authorization:ScopeMappings").Get<Dictionary<string, List<string>>>();
//		if (scopeMappings == null)
//		{
//			throw new InvalidOperationException("ScopeMappings section is missing from configuration.");
//		}

//		// Map roles to scopes based on configuration, or use default scope if no roles match
//		var apiScopes = userRoles.SelectMany(role => scopeMappings.ContainsKey(role) ? scopeMappings[role] : new List<string>())
//								 .Distinct()
//								 .Select(scope => $"api://{apiClientId}/{scope}") // Prepend the API client ID
//								 .ToArray();

//		if (!apiScopes.Any())
//		{
//			// Use the default API scope if no specific scopes were found
//			var defaultScope = _configuration["Authorization:ApiSettings:DefaultScope"];
//			apiScopes = new[] { $"api://{apiClientId}/{defaultScope}" };
//		}

//		// Acquire the access token with the determined API scopes
//		var apiToken = await _tokenAcquisition.GetAccessTokenForUserAsync(apiScopes);
//		return apiToken;
//	}
//}

///// <summary>
///// Interface for acquiring API access tokens.
///// </summary>
//public interface IUserTokenService
//{
//	/// <summary>
//	/// Acquires an API access token by mapping the user's roles to API scopes/permissions using the On-Behalf-Of (OBO) flow.
//	/// </summary>
//	/// <returns>A JWT access token string for accessing the API.</returns>
//	/// <exception cref="UnauthorizedAccessException">Thrown if the user does not have any roles or if no valid scopes are found for the user's roles.</exception>
//	Task<string> GetApiOboTokenAsync();
//}