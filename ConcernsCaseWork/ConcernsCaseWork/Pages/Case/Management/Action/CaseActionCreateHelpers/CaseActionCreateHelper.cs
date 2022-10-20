using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Cases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public abstract class CaseActionCreateHelper
	{
		public abstract bool CanHandle(CaseActionEnum caseActionEnum);
		public abstract Task<bool> NewCaseActionAllowed(long caseUrn, string caseWorker);
		protected virtual bool HasOpenCaseAction(IEnumerable<CaseActionModel> caseActions)
		{
			return caseActions?.Any(ca => ca.ClosedAt == null) ?? false;
		}
	}
}
