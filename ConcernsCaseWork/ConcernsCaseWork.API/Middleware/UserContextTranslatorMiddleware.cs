using ConcernsCaseWork.UserContext;
using System.Security.Claims;

namespace ConcernsCaseWork.API.Middleware
{
	public class UserContextTranslatorMiddleware(RequestDelegate next)
	{
		public async Task InvokeAsync(HttpContext context, IServerUserInfoService userInfoService)
		{
			if (IsApiRequest(context.Request.Path))
			{
				userInfoService.ReceiveRequestHeaders(context.Request.Headers);

				if (userInfoService.UserInfo != null)
				{
					var identity = new ClaimsIdentity("Cookies");

					foreach (var role in userInfoService.UserInfo.Roles)
					{
						identity.AddClaim(new Claim(ClaimTypes.Role, role));
					}

					context.User = new ClaimsPrincipal(identity);
				}
			}

			await next(context);
		}
		private bool IsApiRequest(string path) => path.StartsWith("/v2/") && !path.Contains("swagger");

	}
}
