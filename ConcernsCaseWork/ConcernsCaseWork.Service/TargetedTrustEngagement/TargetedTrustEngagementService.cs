using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.UserContext;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.TargetedTrustEngagement
{
	public class TargetedTrustEngagementService : ConcernsAbstractService, ITargetedTrustEngagementService
	{
		private readonly ILogger<TargetedTrustEngagementService> _logger;

		public TargetedTrustEngagementService(IHttpClientFactory clientFactory, ILogger<TargetedTrustEngagementService> logger, ICorrelationContext correlationContext, IClientUserInfoService userInfoService, IUserTokenService apiTokenService) : base(clientFactory, logger, correlationContext, userInfoService, apiTokenService)
		{
			_logger = Guard.Against.Null(logger);
		}

		public async Task<CreateTargetedTrustEngagementResponse> PostTargetedTrustEngagement(CreateTargetedTrustEngagementRequest createTargetedTrustEngagementRequestDto)
		{
			_logger.LogMethodEntered();

			_ = Guard.Against.Null(createTargetedTrustEngagementRequestDto);

			// post the new tte
			var postResponse = await Post<CreateTargetedTrustEngagementRequest, CreateTargetedTrustEngagementResponse>($"/{EndpointsVersion}/concerns-cases/{createTargetedTrustEngagementRequestDto.CaseUrn}/targetedtrustengagement", createTargetedTrustEngagementRequestDto);

			_logger.LogInformation($"Targeted Trust Engagement created. caseUrn: {postResponse.ConcernsCaseUrn}, TargetedTrustEngagementId:{postResponse.TargetedTrustEngagementId}");
			return postResponse;
		}

		public async Task<List<TargetedTrustEngagementSummaryResponse>> GetTargetedTrustEngagementByCaseUrn(int urn)
		{
			_logger.LogMethodEntered();
			_logger.LogInformation($"Getting ttes for case urn {urn}");

			var result = await Get<List<TargetedTrustEngagementSummaryResponse>>($"/{EndpointsVersion}/concerns-cases/{urn}/targetedtrustengagement");

			_logger.LogInformation($"Retrieved {result.Count} ttes for case urn {urn}");

			return result;
		}

		public async Task<GetTargetedTrustEngagementResponse> GetTargetedTrustEngagement(int urn, int targetedTrustEngagementId)
		{
			_logger.LogMethodEntered();
			
			
			var result = await Get<GetTargetedTrustEngagementResponse>($"/{EndpointsVersion}/concerns-cases/{urn}/targetedtrustengagement/{targetedTrustEngagementId}");
			_logger.LogInformation($"Retrieved targeted trust engagement {targetedTrustEngagementId} for case urn {urn}");
			return result;
		}

		public async Task<UpdateTargetedTrustEngagementResponse> PutTargetedTrustEngagement(int caseUrn, int targetedTrustEngagementId, UpdateTargetedTrustEngagementRequest updateTargetedTrustEngagementRequest)
		{
			_logger.LogMethodEntered();

			var endpoint = $"/{EndpointsVersion}/concerns-cases/{caseUrn}/targetedtrustengagement/{targetedTrustEngagementId}";

			// put the tte
			var putResponse = await Put<UpdateTargetedTrustEngagementRequest, UpdateTargetedTrustEngagementResponse>(endpoint, updateTargetedTrustEngagementRequest);

			_logger.LogInformation($"Targeted trust engagement updated. caseUrn: {putResponse.ConcernsCaseUrn}, TargetedTrustEngagementId:{putResponse.TargetedTrustEngagementId}");
			return putResponse;
		}

		public async Task<CloseTargetedTrustEngagementResponse> CloseTargetedTrustEngagement(int caseUrn, int targetedTrustEngagementId, CloseTargetedTrustEngagementRequest closeTargetedTrustEngagementRequest)
		{
			_logger.LogMethodEntered();

			var endpoint = $"/{EndpointsVersion}/concerns-cases/{caseUrn}/targetedtrustengagement/{targetedTrustEngagementId}/close";

			var response = await Patch<CloseTargetedTrustEngagementRequest, CloseTargetedTrustEngagementResponse>(endpoint, closeTargetedTrustEngagementRequest);

			_logger.LogInformation("Targeted trust engagement closed. caseUrn: {ResponseConcernsCaseUrn}, TargetedTrustEngagementId:{TargetedTrustEngagementId}", response.CaseUrn, response.TargetedTrustEngagementId);

			return response;
		}

		public async Task DeleteTargetedTrustEngagement(int caseUrn, int targetedTrustEngagementId)
		{
			_logger.LogMethodEntered();

			var endpoint = $"/{EndpointsVersion}/concerns-cases/{caseUrn}/targetedtrustengagement/{targetedTrustEngagementId}";

			// delete the tte
			await Delete(endpoint);
			_logger.LogInformation($"Targeted trust engagement deleted. caseUrn: {caseUrn}, TargetedTrustEngagementId:{targetedTrustEngagementId}");
		}
	}
}
