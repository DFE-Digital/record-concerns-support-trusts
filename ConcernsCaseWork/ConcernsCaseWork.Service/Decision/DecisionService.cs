﻿using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Decision
{
	public class DecisionService : ConcernsAbstractService, IDecisionService
	{
		private readonly ILogger<DecisionService> _logger;

		public DecisionService(IHttpClientFactory clientFactory, ILogger<DecisionService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = Guard.Against.Null(logger);
		}

		public async Task<CreateDecisionResponse> PostDecision(CreateDecisionRequest createDecisionDto)
		{
			_logger.LogMethodEntered();

			_ = Guard.Against.Null(createDecisionDto);

			// post the new decision
			var postResponse = await Post<CreateDecisionRequest, CreateDecisionResponse>($"/{EndpointsVersion}/concerns-cases/{createDecisionDto.ConcernsCaseUrn}/decisions", createDecisionDto);

			_logger.LogInformation($"Decision created. caseUrn: {postResponse.ConcernsCaseUrn}, DecisionId:{postResponse.DecisionId}");
			return postResponse;
		}

		public async Task<List<DecisionSummaryResponse>> GetDecisionsByCaseUrn(long urn)
		{
			_logger.LogMethodEntered();
			_logger.LogInformation($"Getting decisions for case urn {urn}");

			var result = await Get<List<DecisionSummaryResponse>>($"/{EndpointsVersion}/concerns-cases/{urn}/decisions");

			_logger.LogInformation($"Retrieved {result.Count} decisions for case urn {urn}");

			return result;
		}

		public async Task<GetDecisionResponse> GetDecision(long urn, int decisionId)
		{
			_logger.LogMethodEntered();
			var result = await Get<GetDecisionResponse>($"/{EndpointsVersion}/concerns-cases/{urn}/decisions/{decisionId}");
			_logger.LogInformation($"Retrieved decision {decisionId} for case urn {urn}");
			return result;
		}

		public async Task<UpdateDecisionResponse> PutDecision(long caseUrn, long decisionId, UpdateDecisionRequest updateDecisionRequest)
		{
			_logger.LogMethodEntered();

			var endpoint = $"/{EndpointsVersion}/concerns-cases/{caseUrn}/decisions/{decisionId}";

			// put the decision
			var putResponse = await Put<UpdateDecisionRequest, UpdateDecisionResponse>(endpoint, updateDecisionRequest);

			_logger.LogInformation($"Decision updated. caseUrn: {putResponse.ConcernsCaseUrn}, DecisionId:{putResponse.DecisionId}");
			return putResponse;
		}
	}
}
