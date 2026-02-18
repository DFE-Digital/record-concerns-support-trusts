using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Cases;

public class ApiCaseSummaryService : ConcernsAbstractService, IApiCaseSummaryService
{
	public ApiCaseSummaryService(
		ILogger<ApiCaseSummaryService> logger, 
		ICorrelationContext correlationContext, 
		IHttpClientFactory clientFactory, 
		IClientUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService) 
	{

	}

	public async Task<ApiListWrapper<ActiveCaseSummaryDto>> GetAllCaseSummariesByFilter(
		Region[] regions = null,
		int? page = 1)
	{
		var queryString = BuildQueryString(regions);
		var result = await GetByPagination<ActiveCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/all?page={page}&count=5{queryString}");

		return result;
	}

	public async Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(
		string caseworker,
		int? page = 1)
	{
		var result = await GetByPagination<ActiveCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/active/team?page={page}&count=5");

		return result;
	}


	public async Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesByCaseworker(
		string caseworker,
		int? page = 1)
	{
		var result = await GetByPagination<ActiveCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/active?page={page}&count=5");

		return result;
	}

	public async Task<ApiListWrapper<ClosedCaseSummaryDto>> GetClosedCaseSummariesByCaseworker(
		string caseworker,
		int? page = 1)
	{
		var result = await GetByPagination<ClosedCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/closed?page={page}&count=5");

		return result;
	}

	public async Task<ApiListWrapper<ActiveCaseSummaryDto>> GetActiveCaseSummariesByTrust(
		string trustUkPrn,
		int? page = 1)
	{
		var result = await GetByPagination<ActiveCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/active?page={page}&count=5");

		return result;
	}

	public async Task<ApiListWrapper<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(
		string trustUkPrn,
		int? page = 1)
	{
		var result = await GetByPagination<ClosedCaseSummaryDto>($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/closed?page={page}&count=5");

        return result;
    }

	private static string BuildQueryParametersString<T>(string key, T[] values)
	{
		if (values == null || values.Length == 0) return string.Empty;
		return "&" + string.Join("&", values.Select(value => $"{key}={value}"));
	}

	private static string BuildQueryString(Region[] regions)
	{
		string queryString = string.Empty;
		queryString += BuildQueryParametersString("regions", regions);
		return queryString;
	}
}