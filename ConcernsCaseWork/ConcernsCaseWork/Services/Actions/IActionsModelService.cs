using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Actions
{
	public interface IActionsModelService
	{
		Task<IList<ActionSummary>> GetClosedActionsSummary(string userName, long caseUrn);
	}
}