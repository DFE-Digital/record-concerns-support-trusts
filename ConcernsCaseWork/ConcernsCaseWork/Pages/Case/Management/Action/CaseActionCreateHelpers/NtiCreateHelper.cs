using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Service.TRAMS.Cases;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public class NtiCreateHelper : CaseActionCreateHelper
	{
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;

		public NtiCreateHelper(INtiUnderConsiderationModelService ntiUnderConsiderationModelService, INtiWarningLetterModelService ntiWarningLetterModelService)
		{
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
		}

		public override bool CanHandle(CaseActionEnum caseActionEnum)
		{
			var canHandleNTI = caseActionEnum == CaseActionEnum.NtiWarningLetter || caseActionEnum == CaseActionEnum.NtiUnderConsideration;

			return canHandleNTI;
		}

		public override async Task<bool> NewCaseActionAllowed(long caseUrn, string caseWorker)
		{
			var ntiUnderConsiderations = await _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn);
			var ntiWarningLetters = await _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn);

			var hasOpenNTIAction = base.HasOpenCaseAction(ntiUnderConsiderations) || base.HasOpenCaseAction(ntiWarningLetters);

			return hasOpenNTIAction ? throw new InvalidOperationException("There is already an open NTI action linked to this case. Please resolve that before opening another one.")
				: true;
		}
	}
}
