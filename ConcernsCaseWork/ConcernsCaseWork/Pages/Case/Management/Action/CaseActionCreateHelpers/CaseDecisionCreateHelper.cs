using ConcernsCaseWork.Service.Cases;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public class CaseDecisionCreateHelper : CaseActionCreateHelper
	{
		public override bool CanHandle(CaseActionEnum caseActionEnum)
		{
			return caseActionEnum == CaseActionEnum.Decision;
		}

		public override async Task<bool> NewCaseActionAllowed(long caseUrn)
		{
			return true;
		}
	}
}
