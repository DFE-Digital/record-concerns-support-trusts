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
		public UserContextReceiverMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext, ILogger<UserContextReceiverMiddleware> logger)
		{
			//var userContext = UserInfo.FromHeaders(httpContext.Request.Headers);

			await _next(httpContext);
		}
	}
}
