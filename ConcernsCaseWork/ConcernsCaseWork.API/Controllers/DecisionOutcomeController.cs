using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases/{caseId:int}/decisions/{decisionId:int}/outcome")]
	public class DecisionOutcomeController : ControllerBase
	{
		IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse> _createDecisionOutcomeUseCase;
		private ILogger<ConcernsStatusController> _logger;

		public DecisionOutcomeController(
			IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse> createDecisionOutcomeUseCase,
			ILogger<ConcernsStatusController> logger)
		{
			_createDecisionOutcomeUseCase = createDecisionOutcomeUseCase;
			_logger = logger;
		}

		[HttpPost]
		[ApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<CreateDecisionOutcomeResponse>>> Create(int caseId, int decisionId, CreateDecisionOutcomeRequest request, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Creating decision outcome for case {caseId}, decision {decisionId}");

			var parameters = new CreateDecisionOutcomeUseCaseParams()
			{
				ConcernsCaseId = caseId,
				DecisionId = decisionId,
				Request = request
			};

			var created = await _createDecisionOutcomeUseCase.Execute(parameters, cancellationToken);

			var result = new ApiSingleResponseV2<CreateDecisionOutcomeResponse>(created);

			_logger.LogInformation($"Created decision outcome for case {caseId}, decision {decisionId}, outcome {created.DecisionOutcomeId}");

			return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
		}

		[HttpPut]
		[ApiVersion("2.0")]
		public IActionResult Put(int urn, int decisionId, UpdateDecisionOutcomeRequest request)
		{
			return new ObjectResult("") { StatusCode = StatusCodes.Status200OK };
		}
	}
}
