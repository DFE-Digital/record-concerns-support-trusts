using Ardalis.GuardClauses;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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
			Guard.Against.Null(request);

			UserInfo headers = CreateUserInfoHeaders();

			foreach (KeyValuePair<string, string> keyValuePair in headers.ToHeadersKVP())
			{
				request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private UserInfo CreateUserInfoHeaders()
		{
			return new UserInfo()
			{
				Name = GetPrincipalName(_claimsPrincipal),
				Roles = UserInfo.ParseRoleClaims(_claimsPrincipal.Claims.Select(x => x.Value).ToArray())
			};
		}

		public void AddHeaders(HttpClient client)
		{
			Guard.Against.Null(client);

			UserInfo headers = CreateUserInfoHeaders();

			foreach (KeyValuePair<string, string> keyValuePair in headers.ToHeadersKVP())
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

		public UserInfo FromRequestHeaders(HttpRequestMessage request)
		{
			Guard.Against.Null(request);
			return UserInfo.FromHeaders(request.Headers.Where(x => x.Key.StartsWith(UserInfo.RoleHeaderKeyPrefix))
				.Select(X => new KeyValuePair<string,string>(X.Key, X.Value.First()))
				.ToArray());
		}
	}
}
