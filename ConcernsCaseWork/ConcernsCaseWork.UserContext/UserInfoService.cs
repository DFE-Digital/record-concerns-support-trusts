using Ardalis.GuardClauses;
using System.Security.Claims;
using System.Security.Principal;

namespace ConcernsCaseWork.UserContext
{
	public class UserInfoService : IUserInfoService
	{
		private ClaimsPrincipal _claimsPrincipal;

		public void SetPrincipal(ClaimsPrincipal claimsPrincipal)
		{
			_claimsPrincipal = claimsPrincipal;
		}

		public void AddHeaders(HttpRequestMessage request)
		{
			var header = new UserInfo()
			{
				Name = GetPrincipalName(_claimsPrincipal),
				Roles = UserInfo.ParseRoleClaims(_claimsPrincipal.Claims.Select(x => x.Value).ToArray())
			};

			foreach (KeyValuePair<string, string> keyValuePair in header.ToHeadersKVP())
			{
				request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private string GetPrincipalName(IPrincipal principal)
		{
			Guard.Against.Null(principal);

			if (principal?.Identity?.Name is null)
			{
				return string.Empty;
			}

			return principal.Identity.Name;
		}
	}
}
