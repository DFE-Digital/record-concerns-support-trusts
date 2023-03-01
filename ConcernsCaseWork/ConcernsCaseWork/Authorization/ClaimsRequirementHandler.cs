namespace ConcernsCaseWork.Authorization
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection.PortableExecutable;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using ConcernsCaseWork.UserContext;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Authorization.Infrastructure;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Hosting;

	public class ClaimsRequirementHandler : AuthorizationHandler<ClaimsAuthorizationRequirement>, IAuthorizationRequirement
	{
		private readonly IHostEnvironment _environment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;

		public ClaimsRequirementHandler(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration)
		{
			_environment = environment;
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
		}
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimsAuthorizationRequirement requirement)
		{
			if (AutomationHandler.ClientSecretHeaderValid(_environment, _httpContextAccessor, _configuration))
			{
				var simpleHeaders = _httpContextAccessor.HttpContext.Request.Headers
					.Select(X => new KeyValuePair<string, string>(X.Key, X.Value.First()))
					.ToArray();

				var userInfo = UserInfo.FromHeaders(simpleHeaders);

				var currentUser = context.User.Identities.FirstOrDefault();

				currentUser?.AddClaim(new Claim(ClaimTypes.Name, userInfo.Name));

				foreach (var claim in userInfo.Roles)
				{
					currentUser?.AddClaim(new Claim(ClaimTypes.Role, claim));
				}

				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
