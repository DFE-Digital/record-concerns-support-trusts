using Ardalis.GuardClauses;
using System;
using System.Security.Principal;

namespace ConcernsCaseWork.Authorization;

public class ClaimsPrincipalHelper : IClaimsPrincipalHelper
{
	public const string CaseWorkerRole = "concerns-casework.caseworker";
	public const string TeamLeaderRole = "concerns-casework.teamleader";
	public const string AdminRole = "concerns-casework.admin";

	public string GetPrincipalName(IPrincipal principal)
	{
		Guard.Against.Null(principal);
		if (principal?.Identity?.Name is null)
			throw new ArgumentNullException("User.Identity returned null");
		
		return principal.Identity.Name;
	}

	public bool IsCaseworker(IPrincipal principal)
	{		
		Guard.Against.Null(principal);
		return principal.IsInRole(CaseWorkerRole);
	}

	public bool IsTeamLeader(IPrincipal principal)
	{
		Guard.Against.Null(principal);
		return principal.IsInRole(TeamLeaderRole);
	}

	public bool IsAdmin(IPrincipal principal)
	{
		Guard.Against.Null(principal);
		return principal.IsInRole(AdminRole);
	}
}