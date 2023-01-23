namespace ConcernsCaseWork.UserContext
{
	public abstract class Claims
	{
		public const string ClaimPrefix = "concerns-casework.";
		public const string CaseWorkerRoleClaim = $"{ClaimPrefix}concerns-casework.caseworker";
		public const string TeamLeaderRoleClaim = $"{ClaimPrefix}teamleader";
		public const string AdminRoleClaim = $"{ClaimPrefix}admin";
	}
}
