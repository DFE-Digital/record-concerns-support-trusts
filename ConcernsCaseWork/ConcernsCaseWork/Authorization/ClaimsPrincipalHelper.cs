using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Context;
using System;
using System.Security.Principal;

namespace ConcernsCaseWork.Authorization;

public class ClaimsPrincipalHelper : IClaimsPrincipalHelper
{
	public string GetPrincipalName(IPrincipal principal)
	{
		Guard.Against.Null(principal);

		if (principal?.Identity?.Name is null)
		{
			throw new ArgumentNullException(nameof(principal.Identity.Name));
		}

		return principal.Identity.Name;
	}

	public bool IsCaseworker(IPrincipal principal)
	{
		Guard.Against.Null(principal);
		return principal.IsInRole(UserContext.CaseWorkerRoleClaim);
	}

	public bool IsTeamLeader(IPrincipal principal)
	{
		Guard.Against.Null(principal);
		return principal.IsInRole(UserContext.TeamLeaderRoleClaim);
	}

	public bool IsAdmin(IPrincipal principal)
	{
		Guard.Against.Null(principal);
		return principal.IsInRole(UserContext.AdminRoleClaim);
	}
}