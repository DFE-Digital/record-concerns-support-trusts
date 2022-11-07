using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Helpers;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Decision
{
	public class DecisionService :  ConcernsAbstractService, IDecisionService
	{
		private readonly ILogger<DecisionService> _logger;

		public DecisionService(IHttpClientFactory clientFactory, ILogger<DecisionService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = Guard.Against.Null(logger);
		}

		public async Task<CreateDecisionResponseDto> PostDecision(CreateDecisionDto createDecisionDto)
		{
			_logger.LogMethodEntered();

			_ = Guard.Against.Null(createDecisionDto);

			// post the new decision
			var postResponse = await Post<CreateDecisionDto, CreateDecisionResponseDto>($"/{EndpointsVersion}/concerns-cases/{createDecisionDto.ConcernsCaseUrn}/decisions", createDecisionDto);

			_logger.LogInformation($"Decision created. caseUrn: {postResponse.ConcernsCaseUrn}, DecisionId:{postResponse.DecisionId}");
			return postResponse;
		}

		public async Task<List<GetDecisionResponseDto>> GetDecisionsByCaseUrn(long urn)
		{
			_logger.LogMethodEntered();
			_logger.LogInformation($"Getting decisions for case urn {urn}");

			var result = await Get<List<GetDecisionResponseDto>>($"/{EndpointsVersion}/concerns-cases/{urn}/decisions");

			_logger.LogInformation($"Retrieved {result.Count} decisions for case urn {urn}");

			return result;
		}

		public Task<GetDecisionResponseDto> GetDecision(long urn, int decisionId)
		{
			_logger.LogMethodEntered();
			throw new NotImplementedException();
		}
	}
}
