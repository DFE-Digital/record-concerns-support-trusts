using Ardalis.GuardClauses;
using System.Security.Claims;
using System.Security.Principal;

namespace ConcernsCaseWork.UserContext
{

	public class ClientUserInfoService : IClientUserInfoService
	{
		/// <summary>
		/// Defualt constructor. Required for using the service in middleware.
		/// If using this constructor and claims principal is available, call SetPrincipal
		/// </summary>
		public ClientUserInfoService()
		{

		}

		/// <summary>
		/// Constructs a userinfo service instance given user info that has been previously configured.
		/// For use in an environment where a claims principal is available, use the default constructor followed by SetPrincipal
		/// </summary>
		/// <param name="userInfo"></param>
		public void SetPrincipal(UserInfo userInfo)
		{
			UserInfo = userInfo;
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

		public bool AddUserInfoRequestHeaders(HttpClient client)
		{
			Guard.Against.Null(client);

			if (UserInfo == null)
			{
				return false;
			}

			foreach (KeyValuePair<string, string> keyValuePair in UserInfo.ToHeadersKVP())
			{
				client.DefaultRequestHeaders.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
			}
			return true;
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
