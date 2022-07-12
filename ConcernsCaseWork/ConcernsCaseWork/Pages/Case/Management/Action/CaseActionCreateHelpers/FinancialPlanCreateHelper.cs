using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.FinancialPlan;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public class FinancialPlanCreateHelper : CaseActionCreateHelper
	{
		private readonly IFinancialPlanModelService _financialPlanModelService;

		public FinancialPlanCreateHelper(IFinancialPlanModelService financialPlanModelService)
		{
			_financialPlanModelService = financialPlanModelService;
		}

		public override bool CanHandle(CaseActionEnum caseActionEnum)
		{
			return caseActionEnum == CaseActionEnum.FinancialPlan;
		}

		public override async Task<bool> NewCaseActionAllowed(long caseUrn, string caseWorker)
		{
			var fps = await _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn, caseWorker);
			return base.HasOpenCaseAction(fps) ? throw new InvalidOperationException("There is already an open Financial Plan action linked to this case. Please resolve that before opening another one.")
				: true;
		}
	}
}
