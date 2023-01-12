using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Context;
using System.Security.Claims;
using System.Security.Principal;

namespace ConcernsCaseWork.Service.Context
{
	public class UserContextService : IUserContextService
	{
		private ClaimsPrincipal _claimsPrincipal;

		public void SetPrincipal(ClaimsPrincipal claimsPrincipal)
		{
			_claimsPrincipal = claimsPrincipal;
		}

		public void AddHeaders(HttpRequestMessage request)
		{
			var header = new UserContextHeader()
			{
				Name = GetPrincipalName(_claimsPrincipal),
				Roles = _claimsPrincipal.FindAll(c => c.Value.StartsWith("concerns-casework.")).Select(x => x.Value).ToArray()
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
