using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.Cases;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public class NtiUnderConsiderationCreateHelper : CaseActionCreateHelper
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;

		public NtiUnderConsiderationCreateHelper(INtiUnderConsiderationModelService ntiModelService)
		{
			_ntiModelService = ntiModelService;
		}

		public override bool CanHandle(CaseActionEnum caseActionEnum)
		{
			return caseActionEnum == CaseActionEnum.NtiUnderConsideration;
		}

		public override async Task<bool> NewCaseActionAllowed(long caseUrn, string caseWorker)
		{
			var ntis = await _ntiModelService.GetNtiUnderConsiderationsForCase(caseUrn);
			return base.HasOpenCaseAction(ntis) ? throw new InvalidOperationException("There is already an open NTI: Under consideration action linked to this case. Please resolve that before opening another one.")
				: true;
		}
	}
}
