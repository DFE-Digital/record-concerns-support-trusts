using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.FinancialPlan;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public class NtiWarningLetterCreateHelper : CaseActionCreateHelper
	{
		public NtiWarningLetterCreateHelper()
		{
			
		}

		public override bool CanHandle(CaseActionEnum caseActionEnum)
		{
			return caseActionEnum == CaseActionEnum.NtiWarningLetter;
		}

		public override Task<bool> NewCaseActionAllowed(long caseUrn, string caseWorker)
		{
			return Task.FromResult(true);
		}
	}
}
