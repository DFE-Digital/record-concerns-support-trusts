using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Service.Cases;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers
{
	public class SrmaCreateHelper : CaseActionCreateHelper
	{
		private readonly ISRMAService _srmaService;

		public SrmaCreateHelper(ISRMAService srmaService)
		{
			_srmaService = srmaService;
		}

		public override bool CanHandle(CaseActionEnum caseActionEnum)
		{
			return caseActionEnum == CaseActionEnum.Srma;
		}

		public override async Task<bool> NewCaseActionAllowed(long caseUrn, string caseWorker)
		{
			var srmas = await _srmaService.GetSRMAsForCase(caseUrn);

			return base.HasOpenCaseAction(srmas) ? throw new InvalidOperationException("There is already an open SRMA action linked to this case. Please resolve that before opening another one.")
				: true;
		}
	}
}
