using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class ActionSummaryBreakdownModel
	{
		public ActionSummaryBreakdownModel()
		{
			OpenActions = new List<ActionSummaryModel>();
			ClosedActions = new List<ActionSummaryModel>();
		}
		public List<ActionSummaryModel> OpenActions { get; set; }
		public List<ActionSummaryModel> ClosedActions { get; set; }
	}
}
