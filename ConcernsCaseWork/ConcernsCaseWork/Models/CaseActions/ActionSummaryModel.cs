namespace ConcernsCaseWork.Models.CaseActions
{
	public record ActionSummaryModel
	{
		public string RelativeUrl { get; set; }
		public string StatusName { get; set; }
		public string Name { get; set; }
		public string OpenedDate { get; set; }
		public string ClosedDate { get; set; }
	}
}