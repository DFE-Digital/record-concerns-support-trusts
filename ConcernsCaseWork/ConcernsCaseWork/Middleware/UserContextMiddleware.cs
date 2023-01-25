using Ardalis.GuardClauses;
using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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