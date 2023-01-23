using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

			if (IsPageRequest(httpContext.Request.Path))
			{
				userInfoService.ReceiveRequestHeaders(httpContext.Request.Headers);

				if (userInfoService.UserInfo == null)
				{
					logger.LogError($"Call to {httpContext.Request.Path} received without user information headers. Responding with bad request");
					httpContext.Response.Clear();
					httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					await httpContext.Response.WriteAsync("Unauthorized. Requests must supply user-context information");
					return;
				}
			}

			await _next(httpContext);
		}

		private bool IsPageRequest(string path) => path.StartsWith("/v2/") && !path.Contains("swagger");

	}
}
