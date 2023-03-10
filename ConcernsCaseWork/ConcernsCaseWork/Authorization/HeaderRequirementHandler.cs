namespace ConcernsCaseWork.Authorization
{
	using System.Linq;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Authorization.Infrastructure;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Net.Http.Headers;

	//Handler is registered from the method RequireAuthenticatedUser()
	public class HeaderRequirementHandler : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>,
		IAuthorizationRequirement
	{
		private readonly IHostEnvironment _environment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;

		public HeaderRequirementHandler(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor,
			IConfiguration configuration)
		{
			_environment = environment;
			_httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
			DenyAnonymousAuthorizationRequirement requirement)
		{
			if (AutomationHandler.ClientSecretHeaderValid(_environment, _httpContextAccessor, _configuration))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
