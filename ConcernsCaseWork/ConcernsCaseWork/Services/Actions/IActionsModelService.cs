using ConcernsCaseWork.Models.CaseActions;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Actions
{
	public interface IActionsModelService
	{
		Task<ActionSummaryBreakdownModel> GetActionsSummary(long caseUrn);
	}
}