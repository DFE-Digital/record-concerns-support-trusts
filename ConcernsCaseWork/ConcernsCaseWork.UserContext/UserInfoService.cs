using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace ConcernsCaseWork.UserContext
{
	public class UserInfoService : IUserInfoService
	{
		public UserInfo? UserInfo { get; private set; }

		public void SetPrincipal(ClaimsPrincipal claimsPrincipal)
		{
			UserInfo = CreateUserInfo(claimsPrincipal);
		}

		public void AddHeaders(HttpRequestMessage request)
		{
			Guard.Against.Null(request);

			foreach (KeyValuePair<string, string> keyValuePair in UserInfo.ToHeadersKVP())
			{
				request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
			}
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

		public void ReceiveRequestHeaders(IHeaderDictionary headers)
		{
			var simpleHeaders = headers.Where(x => x.Key.StartsWith(UserInfo.RoleHeaderKeyPrefix))
				.Select(X => new KeyValuePair<string, string>(X.Key, X.Value.First()))
				.ToArray();
			UserInfo = UserInfo.FromHeaders(simpleHeaders);
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
