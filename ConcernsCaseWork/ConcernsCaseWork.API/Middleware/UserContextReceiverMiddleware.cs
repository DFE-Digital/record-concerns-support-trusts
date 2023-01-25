using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;
using System;
using System.Collections.Generic;
using System.Linq;
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

			if (IsApiRequest(httpContext.Request.Path))
			{
				userInfoService.ReceiveRequestHeaders(httpContext.Request.Headers);

				if (userInfoService.UserInfo == null)
				{
					logger.LogWarningMsg($"Call to {httpContext.Request.Path}");
				}
			}

			await _next(httpContext);
		}

		private bool IsApiRequest(string path) => path.StartsWith("/v2/");

	}
}
