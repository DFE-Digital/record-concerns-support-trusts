using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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


	public async Task<PagedCasesDto> GetActiveCaseSummariesByTrust(string trustUkPrn, int page, int recordCount)
	{
		var  response=await GetByPagination<ActiveCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/active?page={page}&count={recordCount}");
		PagedCasesDto result = new PagedCasesDto();
		result.ActiveCases = response.Data.ToList();
		result.PageData = new Paging()
		{
			Page = response.Paging.Page,
			HasPrevious = response.Paging.HasPrevious,
			RecordCount = response.Paging.RecordCount,
			NextPageUrl = response.Paging.NextPageUrl,
			HasNext = response.Paging.HasNext,
			Sort = "",
			SearchPhrase = ""
		};
		
		return result;
	}

	public async Task<IEnumerable<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(string trustUkPrn)
		=> await Get<IEnumerable<ClosedCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/closed");

	public async Task<PagedCasesDto> GetClosedCaseSummariesByTrust(string trustUkPrn, int page, int recordCount)
	{
		var response =await GetByPagination<ClosedCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/closed?page={page}&count={recordCount}");
		var result = new PagedCasesDto();
		result.ClosedCases = response.Data.ToList();
		result.PageData = new Paging()
		{
			Page = response.Paging.Page,
			HasPrevious = response.Paging.HasPrevious,
			RecordCount = response.Paging.RecordCount,
			NextPageUrl = response.Paging.NextPageUrl,
			HasNext = response.Paging.HasNext,
			Sort = "",
			SearchPhrase = ""
		};
		return result;
	}
		
}