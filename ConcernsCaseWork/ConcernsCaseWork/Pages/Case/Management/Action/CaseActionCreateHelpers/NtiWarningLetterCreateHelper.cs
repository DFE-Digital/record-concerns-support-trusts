using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public class NtiWarningLetterCreateHelper : CaseActionCreateHelper
	{
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;

		public NtiWarningLetterCreateHelper(INtiWarningLetterModelService ntiWarningLetterModelService)
		{
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
		}

		public override bool CanHandle(CaseActionEnum caseActionEnum)
		{
			return caseActionEnum == CaseActionEnum.NtiWarningLetter;
		}

		public override async Task<bool> NewCaseActionAllowed(long caseUrn, string caseWorker)
		{
			var ntis = await _ntiWarningLetterModelService.GetNtiWLsForCase(caseUrn);
			return base.HasOpenCaseAction(ntis) ? throw new InvalidOperationException("There is already an open NTI: Warning letter action linked to this case. Please resolve that before opening another one.")
				: true;
		}
	}
}
