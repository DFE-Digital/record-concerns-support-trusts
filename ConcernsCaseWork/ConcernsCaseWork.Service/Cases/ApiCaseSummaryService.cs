using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Cases;

public class ApiCaseSummaryService : ConcernsAbstractService, IApiCaseSummaryService
{
	public ApiCaseSummaryService(ILogger<ApiCaseSummaryService> logger, ICorrelationContext correlationContext, IHttpClientFactory clientFactory, IClientUserInfoService userInfoService) :
		base(clientFactory, logger, correlationContext, userInfoService) { }
	public async Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker)
		=> await Get<IEnumerable<ActiveCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/active/team");
	
	public async Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker)
		=> await Get<IEnumerable<ActiveCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/active");

	public async Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByCaseworker(string caseworker)
		=> await Get<IEnumerable<ClosedCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/closed");

	public async Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByTrust(string trustUkPrn)
		=> await Get<IEnumerable<ActiveCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/active");

	public async Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(string trustUkPrn)
		=> await Get<IEnumerable<ClosedCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/closed");
}