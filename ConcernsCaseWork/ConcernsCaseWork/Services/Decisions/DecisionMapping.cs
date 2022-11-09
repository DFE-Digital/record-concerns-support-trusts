using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Decision;

namespace ConcernsCaseWork.Services.Decisions
{
	public class DecisionMapping
	{
		public static ActionSummaryModel ToActionSummary(DecisionSummaryResponseDto decisionSummary)
		{
			var result = new ActionSummaryModel()
			{
				OpenedDate = decisionSummary.CreatedAt.ToDayMonthYear(),
				ClosedDate = decisionSummary.ClosedAt?.ToDayMonthYear(),
				Name = $"Decision: {decisionSummary.Title}",
				StatusName = EnumHelper.GetEnumDescription(decisionSummary.Status),
				RelativeUrl = $"/case/{decisionSummary.ConcernsCaseUrn}/management/action/decision/{decisionSummary.DecisionId}"
			};

			return result;
		}
	}
}
