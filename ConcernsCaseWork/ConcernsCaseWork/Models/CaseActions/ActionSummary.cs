namespace ConcernsCaseWork.Models.CaseActions
{
	public record ActionSummary
	{
		public string RelativeUrl { get; init; }
		public string StatusName { get; init; }
		public string Name { get; init; }
		public string OpenedDate { get; init; }
		public string ClosedDate { get; init; }
	}
}