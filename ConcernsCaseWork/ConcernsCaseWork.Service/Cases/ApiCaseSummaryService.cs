using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ConcernsCaseWork.Service.Cases;

public class ApiCaseSummaryService : ConcernsAbstractService, IApiCaseSummaryService
{
	private IFeatureManager _featureManager;

	public ApiCaseSummaryService(
		ILogger<ApiCaseSummaryService> logger, 
		ICorrelationContext correlationContext, 
		IHttpClientFactory clientFactory, 
		IClientUserInfoService userInfoService,
		IFeatureManager featureManager) : base(clientFactory, logger, correlationContext, userInfoService) 
	{
		_featureManager = featureManager;
	}

	public async Task<IEnumerable<ActiveCaseSummaryDto>> GetActiveCaseSummariesForUsersTeam(string caseworker)
		=> await Get<IEnumerable<ActiveCaseSummaryDto>>($"/{EndpointsVersion}/concerns-cases/summary/{caseworker}/active/team");

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
		var endpoint = await AppendPagination($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/active", page);

		var result = await GetByPagination<ActiveCaseSummaryDto>(endpoint);

		return result;
	}

	public async Task<ApiListWrapper<ClosedCaseSummaryDto>> GetClosedCaseSummariesByTrust(
		string trustUkPrn,
		int? page = 1)
	{
		var endpoint = await AppendPagination($"/{EndpointsVersion}/concerns-cases/summary/bytrust/{trustUkPrn}/closed", page);

		var result = await GetByPagination<ClosedCaseSummaryDto>(endpoint);
        return result;
    }

	private async Task<string> AppendPagination(string endpoint, int? page)
	{
		var isPaginationEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.IsTrustCasesTabsEnabled);

		var result = endpoint;

		if (isPaginationEnabled)
		{
			result += $"?page={page}&count=5";
		}

		return result;
	}
}