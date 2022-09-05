using System.Security.Claims;

namespace ConcernsCaseWork.Helpers
{
	public interface IClaimsPrincipalHelper
	{
		public string GetPrincipalName(ClaimsPrincipal principal);
	}
}
