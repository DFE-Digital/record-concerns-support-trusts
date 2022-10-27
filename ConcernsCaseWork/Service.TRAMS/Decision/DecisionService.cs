using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Helpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public class DecisionService :  AbstractService, IDecisionService
	{
		private readonly ILogger<DecisionService> _logger;

		public DecisionService(IHttpClientFactory clientFactory, ILogger<DecisionService> logger, ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = Guard.Against.Null(logger);
		}

		public async Task<CreateDecisionResponseDto> PostDecision(CreateDecisionDto createDecisionDto)
		{
			_logger.LogInformation($"{nameof(DecisionService)}::{LoggingHelpers.EchoCallerName()}");

			_ = Guard.Against.Null(createDecisionDto);

			// post the new decision
			var postResponse = await Post<CreateDecisionDto, CreateDecisionResponseDto>($"/{_endpointsVersion}/concerns-cases/{createDecisionDto.ConcernsCaseUrn}/decisions", createDecisionDto);

			_logger.LogInformation($"Decision created. caseUrn: {postResponse.ConcernsCaseUrn}, DecisionId:{postResponse.DecisionId}");
			return postResponse;
		}
	}
}

