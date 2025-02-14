using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ConcernsCaseWork.Authorization
{
	public class CypressAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IHostEnvironment _environment;
		private readonly IConfiguration _configuration;

		public CypressAuthenticationHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			IHttpContextAccessor httpContextAccessor,
			IHostEnvironment environment,
			IConfiguration configuration)
			: base(options, logger, encoder)
		{
			_httpContextAccessor = httpContextAccessor;
			_environment = environment;
			_configuration = configuration;
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var httpContext = _httpContextAccessor.HttpContext;
			if (httpContext == null)
			{
				return Task.FromResult(AuthenticateResult.Fail("No HttpContext"));
			}

			if (!AutomationHandler.ClientSecretHeaderValid(_environment, _httpContextAccessor, _configuration))
			{
				return Task.FromResult(AuthenticateResult.Fail("No Cypress headers"));
			}

			var userId = httpContext.Request.Headers["x-user-context-id"].FirstOrDefault() ?? Guid.NewGuid().ToString();

			var headers = httpContext.Request.Headers
				.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.First()))
				.ToArray();

			var userInfo = UserInfo.FromHeaders(headers);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, userInfo.Name),
				new Claim(ClaimTypes.NameIdentifier, userId),
				new Claim(ClaimTypes.Authentication, "true")
			};

			foreach (var claim in userInfo.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, claim));
			}

			var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
			var principal = new ClaimsPrincipal(claimsIdentity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);

			return Task.FromResult(AuthenticateResult.Success(ticket));
		}
	}
}