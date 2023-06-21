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

	public async Task<ActivePagedCasesDto> GetActiveCaseSummariesByTrust(string trustUkPrn, int page, int recordCount)
	{
		var response =await GetByPagination<ActivePagedCasesDto>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/active?page={page}&count={recordCount}");
		ActivePagedCasesDto result = new ActivePagedCasesDto();
		result.Cases = response.Data.GetEnumerator().Current.Cases;
		result.Page = response.Paging.Page;
		result.RecordCount = response.Paging.RecordCount;
		result.HasNext = response.Paging.HasNext;
		result.HasPrevious = response.Paging.HasPrevious;
		return result;
		

	}


	public async Task<ClosedPagedCasesDto> GetClosedCaseSummariesByTrust(string trustUkPrn, int page, int recordCount)
	{
		var response =await GetByPagination<ClosedPagedCasesDto>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/active?page={page}&count={recordCount}");
		var result = new ClosedPagedCasesDto();
		result.Cases = response.Data.GetEnumerator().Current.Cases;
		result.Page = response.Paging.Page;
		result.RecordCount = response.Paging.RecordCount;
		result.HasNext = response.Paging.HasNext;
		result.HasPrevious = response.Paging.HasPrevious;
		return result;
	}
		
}