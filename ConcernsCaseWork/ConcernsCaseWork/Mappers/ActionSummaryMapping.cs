using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public class ActionSummaryMapping
	{
		public static ActionSummaryBreakdownModel ToActionSummaryBreakdown(List<ActionSummaryModel> actionSummaries)
		{
			var openCaseActions = actionSummaries.Where(a => a.RawClosedDate == null).OrderBy(a => a.RawOpenedDate).ToList();
			var closedCaseActions = actionSummaries.Except(openCaseActions).OrderBy(a => a.RawClosedDate).ToList();

			var result = new ActionSummaryBreakdownModel()
			{
				OpenActions = openCaseActions,
				ClosedActions = closedCaseActions
			};

			return result;
		}
	}
}
