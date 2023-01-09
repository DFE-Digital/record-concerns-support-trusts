using System.Security.Principal;

namespace ConcernsCaseWork.Authorization
{
	public interface IClaimsPrincipalHelper
	{
		public string GetPrincipalName(IPrincipal principal);
	}
}
