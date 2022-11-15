using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Cases;

public class ApiCaseSummaryService : ConcernsAbstractService, IApiCaseSummaryService
{	
	public ApiCaseSummaryService(ILogger<ApiCaseSummaryService> logger, ICorrelationContext correlationContext, IHttpClientFactory clientFactory) : 
		base(clientFactory, logger, correlationContext) { }

	public async Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(string caseworker)
		=> await Get<IEnumerable<ActiveCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}");
}