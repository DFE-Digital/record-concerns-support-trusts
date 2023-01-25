using Ardalis.GuardClauses;
using ConcernsCaseWork.UserContext;
using System.Net;
using System.Text;

namespace ConcernsCaseWork.API.Middleware
{
	public class UserContextReceiverMiddleware
	{
		private readonly RequestDelegate _next;

		public UserContextReceiverMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext, IServerUserInfoService userInfoService, ILogger<UserContextReceiverMiddleware> logger)
		{
			Guard.Against.Null(userInfoService);
			Guard.Against.Null(logger);

			if (IsApiRequest(httpContext.Request.Path))
			{
				userInfoService.ReceiveRequestHeaders(httpContext.Request.Headers);

				if (userInfoService.UserInfo == null)
				{
					logger.LogError($"Call to {httpContext.Request.Path} received without user information headers. Responding with unauthorized request. Headers:{HeadersToStrings(httpContext.Request)}");
					httpContext.Response.Clear();
					httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					await httpContext.Response.WriteAsync($"Unauthorized. Requests must supply user-context information. Requested path: {httpContext.Request.Path}, Headers:{HeadersToStrings(httpContext.Request)}");
					return;
				}
			}

			await _next(httpContext);
		}

		private string HeadersToStrings(HttpRequest httpContextRequest)
		{
			var sb = new StringBuilder();
			var headerStrings = httpContextRequest.Headers.Select(x => $"Key:{x.Key}, Value'{x.Value.ToString()}'; ").ToArray();
			sb.AppendJoin(';', headerStrings);
			return sb.ToString();
		}

		private bool IsApiRequest(string path) => path.StartsWith("/v2/") && !path.Contains("swagger");

	}
}
