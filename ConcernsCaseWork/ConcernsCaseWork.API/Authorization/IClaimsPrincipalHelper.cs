using System.Security.Principal;

namespace ConcernsCaseWork.API.Authorization
{
	public interface IClaimsPrincipalHelper
	{
		public string GetPrincipalName(IPrincipal principal);

		public bool IsAdmin(IPrincipal principal);
	}
}
