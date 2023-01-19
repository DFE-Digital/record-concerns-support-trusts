using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;

public class TrustFinancialForecastCreateHelper : CaseActionCreateHelper
{
	private readonly ITrustFinancialForecastService _trustFinancialForecastService;

	public TrustFinancialForecastCreateHelper(ITrustFinancialForecastService trustFinancialForecastService)
	{
		_trustFinancialForecastService = trustFinancialForecastService;
	}

	public override bool CanHandle(CaseActionEnum caseActionEnum) => caseActionEnum == CaseActionEnum.TrustFinancialForecast;

	public override async Task<bool> NewCaseActionAllowed(long caseUrn)
	{
		var trustFinancialForecasts = await _trustFinancialForecastService.GetAllForCase((int)caseUrn);

		var hasOpenCaseAction = trustFinancialForecasts.Any(f => !f.ClosedAt.HasValue);

		return hasOpenCaseAction 
			? throw new InvalidOperationException("There is already an open trust financial forecast action linked to this case. Please resolve that before opening another one.")
			: true;
	}
}