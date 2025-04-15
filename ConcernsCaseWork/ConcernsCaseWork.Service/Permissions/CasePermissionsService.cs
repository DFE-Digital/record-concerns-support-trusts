using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Permissions
{
	// TODO: Rename this service to 'PermissionsService'
	public class CasePermissionsService(
		IHttpClientFactory clientFactory,
		ILogger<CasePermissionsService> logger,
		ICorrelationContext correlationContext,
		IClientUserInfoService userInfoService,
		IUserTokenService userTokenService) : ConcernsAbstractService(clientFactory, logger, correlationContext, userInfoService, userTokenService), ICasePermissionsService
	{
		public async Task<GetCasePermissionsResponse> GetCasePermissions(long caseId)
		{
			logger.LogMethodEntered();
			logger.LogInformation($"Client getting permissions for case {caseId}");

			Guard.Against.NegativeOrZero(caseId);

			var query = new PermissionQueryRequest()
			{
				CaseIds = new[] { caseId }
			};

			var queryResult = await Post<PermissionQueryRequest, PermissionQueryResponse>($"/{EndpointsVersion}/permissions/", query, true);

			logger.LogInformation($"Client retrieved permissions for case {caseId}");

			// find the result for the given case and return that
			return new GetCasePermissionsResponse()
			{
				Permissions = queryResult.CasePermissionResponses.Single(x => x.CaseId == caseId).Permissions.ToList()
			};
		}

		public async Task<bool> UserHasDeletePermissions(long caseId)
		{
			var casePermissions = await GetCasePermissions(caseId);
			return casePermissions.HasDeletePermissions();
		}
	}

	public interface ICasePermissionsService
	{
		public Task<GetCasePermissionsResponse> GetCasePermissions(long caseId);
		public Task<bool> UserHasDeletePermissions(long caseId);
	}
}
