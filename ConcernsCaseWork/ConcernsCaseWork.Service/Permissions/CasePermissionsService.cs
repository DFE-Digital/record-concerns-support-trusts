using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ConcernsCaseWork.Service.Permissions
{
	public class CasePermissionsService : ConcernsAbstractService, ICasePermissionsService
	{
		private readonly ILogger<CasePermissionsService> _logger;
		private const string _url = @"/v2/case-actions/nti-warning-letter";

		public CasePermissionsService(
			IHttpClientFactory clientFactory,
			ILogger<CasePermissionsService> logger,
			ICorrelationContext correlationContext,
			IClientUserInfoService userInfoService) : base(clientFactory, logger, correlationContext, userInfoService)
		{
			_logger = logger;
		}

		public async Task<GetCasePermissionsResponse> GetCasePermissions(long caseId)
		{
			_logger.LogMethodEntered();
			_logger.LogInformation($"Client getting permissions for case {caseId}");

			Guard.Against.NegativeOrZero(caseId);

			var query = new PermissionQueryRequest()
			{
				CaseIds = new[] { caseId }
			};

			var queryResult = await Post<PermissionQueryRequest, PermissionQueryResponse>($"/{EndpointsVersion}/permissions/", query, true);

			_logger.LogInformation($"Client retrieved permissions for case {caseId}");

			// find the result for the given case and return that
			return new GetCasePermissionsResponse()
			{
				Permissions = queryResult.CasePermissionResponses.Single(x => x.CaseId == caseId).Permissions.ToList()
			};
		}
	}

	public interface ICasePermissionsService
	{
		public Task<GetCasePermissionsResponse> GetCasePermissions(long caseId);
	}
}
