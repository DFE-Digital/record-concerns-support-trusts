using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;

namespace ConcernsCaseWork.Services.Context
{
	public class UserContextService : IUserContextService
	{
		private IPrincipal _principal;

		public void SetPrincipal(IPrincipal principal)
		{
			_principal = principal;

			var contextHeaderString = $"name:{GetPrincipalName(_principal)}";
		}

		public void AddHeaders(HttpRequestMessage request)
		{
			if (_principal != null)
			{
				var contextHeaderString = $"name:{GetPrincipalName(_principal)}";
				request.Headers.Add("x-user-context", contextHeaderString);
			}
			else
			{
				request.Headers.Add("x-user-context", string.Empty);
			}
		}


		private string GetPrincipalName(IPrincipal principal)
		{
			Guard.Against.Null(principal);

			if (principal?.Identity?.Name is null)
			{
				throw new ArgumentNullException(nameof(principal.Identity.Name));
			}

			return principal.Identity.Name;
		}
	}

	public interface IUserContextService
	{
		void SetPrincipal(IPrincipal principal);
		void AddHeaders(HttpRequestMessage request);
	}
}
