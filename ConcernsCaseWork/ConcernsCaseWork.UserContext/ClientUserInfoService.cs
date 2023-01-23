using Ardalis.GuardClauses;
using System.Security.Claims;
using System.Security.Principal;

namespace ConcernsCaseWork.UserContext
{

	public class ClientUserInfoService : IClientUserInfoService
	{
		public ClientUserInfoService()
		{

		}
		public UserInfo UserInfo { get; private set; }

		public void SetPrincipal(ClaimsPrincipal claimsPrincipal)
		{
			UserInfo = CreateUserInfo(claimsPrincipal);
		}

		private UserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
		{
			return new UserInfo()
			{
				Name = GetPrincipalName(claimsPrincipal),
				Roles = UserInfo.ParseRoleClaims(claimsPrincipal.Claims.Select(x => x.Value).ToArray())
			};
		}

		public void AddRequestHeaders(HttpClient client)
		{
			Guard.Against.Null(client);

			if (UserInfo == null)
			{
				return;
			}

			foreach (KeyValuePair<string, string> keyValuePair in UserInfo.ToHeadersKVP())
			{
				client.DefaultRequestHeaders.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
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
