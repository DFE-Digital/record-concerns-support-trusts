using System;
using System.Security.Claims;

namespace ConcernsCaseWork.Helpers;

public class ClaimsPrincipalHelper : IClaimsPrincipalHelper
{
	public string GetPrincipalName(ClaimsPrincipal principal)
	{
		if (principal?.Identity?.Name is null)
		{
			throw new NullReferenceException("User.Identity returned null");
		}

		return principal.Identity.Name;
	}
}