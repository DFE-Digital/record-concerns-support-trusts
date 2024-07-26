﻿namespace ConcernsCaseWork.UserContext
{
	public abstract class Claims
	{
		public const string ClaimPrefix = "concerns-casework.";
		public const string CaseWorkerRoleClaim = $"{ClaimPrefix}caseworker";
		public const string TeamLeaderRoleClaim = $"{ClaimPrefix}teamleader";
		public const string AdminRoleClaim = $"{ClaimPrefix}admin";
		//public const string CaseDeleteRoleClaim = $"{ClaimPrefix}case-delete-group";
		public const string CaseDeleteRoleClaim = $"{ClaimPrefix}admin";
	}
}
