using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class ViewTargetedTrustEngagementModel
	{
		public string Id { get; set; }
		public string Notes { get; set; }
		public string Activity { get; set; }
		public List<string> ActivityTypes { get; set; }
		public string Outcome { get; set; }
		public string DateOpened { get; set; }
		public string DateClosed { get; set; }
		public string DateBegan { get; set; }
		public string DateEnded { get; set; }
		public bool IsClosed { get; set; }
		public bool IsEditable { get; set; }
		public string EditLink { get; set; }
	}
}
