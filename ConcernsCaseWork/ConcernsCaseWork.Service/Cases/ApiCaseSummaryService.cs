using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Cases;

public class ApiCaseSummaryService(
	ILogger<ApiCaseSummaryService> logger,
	ICorrelationContext correlationContext,
	IHttpClientFactory clientFactory,
	IClientUserInfoService userInfoService,
	IUserTokenService userTokenService) : ConcernsAbstractService(clientFactory, logger, correlationContext, userInfoService, userTokenService), IApiCaseSummaryService
{

	//public async Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker)
	//	=> await Get<IEnumerable<ActiveCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/active/team");

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
}