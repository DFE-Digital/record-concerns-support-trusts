using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Middleware
{
	public class UserContextReceiverMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IUserInfoService _userInfoService;
		private readonly ILogger<UserContextReceiverMiddleware> _logger;

		public UserContextReceiverMiddleware(RequestDelegate next, IUserInfoService userInfoService, ILogger<UserContextReceiverMiddleware> logger)
		{
			_next = next;
			_userInfoService = Guard.Against.Null(userInfoService);
			_logger = Guard.Against.Null(logger);
		}

		public async Task InvokeAsync(HttpContext httpContext, ILogger<UserContextReceiverMiddleware> logger)
		{
			_userInfoService.ReceiveRequestHeaders(httpContext.Request.Headers);

			if (_userInfoService.UserInfo == null)
			{
				_logger.LogWarningMsg($"Call to {httpContext.Request.Path}");
			}

			await _next(httpContext);
		}
	}
}
