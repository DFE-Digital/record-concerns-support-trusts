using Ardalis.GuardClauses;
using ConcernsCaseWork.Service.Context;
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

	public Task Invoke(HttpContext httpContext, IUserContextService userContextService)
	{
		userContextService.SetPrincipal(httpContext.User);
		return _next(httpContext);
	}
}