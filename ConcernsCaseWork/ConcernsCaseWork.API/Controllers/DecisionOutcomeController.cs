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
		IUseCaseAsync<UpdateDecisionOutcomeUseCaseParams, UpdateDecisionOutcomeResponse> _updateDecisionOutcomeUseCase;
		private ILogger<ConcernsStatusController> _logger;

		public DecisionOutcomeController(
			IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse> createDecisionOutcomeUseCase,
			IUseCaseAsync<UpdateDecisionOutcomeUseCaseParams, UpdateDecisionOutcomeResponse> updateDecisionOutcomeUseCase,
			ILogger<ConcernsStatusController> logger)
		{
			_createDecisionOutcomeUseCase = createDecisionOutcomeUseCase;
			_updateDecisionOutcomeUseCase = updateDecisionOutcomeUseCase;
			_logger = logger;
		}

		[HttpPost]
		[ApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<CreateDecisionOutcomeResponse>>> Create(
			int caseId, 
			int decisionId, 
			CreateDecisionOutcomeRequest request, 
			CancellationToken cancellationToken)
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
		public async Task<ActionResult<ApiSingleResponseV2<UpdateDecisionOutcomeResponse>>> Put(
			int caseId, 
			int decisionId, 
			UpdateDecisionOutcomeRequest request, 
			CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Updating decision outcome for case {caseId}, decision {decisionId}");

			var parameters = new UpdateDecisionOutcomeUseCaseParams()
			{
				ConcernsCaseId = caseId,
				DecisionId = decisionId,
				Request = request
			};

			var updated = await _updateDecisionOutcomeUseCase.Execute(parameters, cancellationToken);

			_logger.LogInformation($"Updated decision outcome for case {caseId}, decision {decisionId}, outcome {updated.DecisionOutcomeId}");

			var result = new ApiSingleResponseV2<UpdateDecisionOutcomeResponse>()
			{
				Data = new UpdateDecisionOutcomeResponse()
				{
					ConcernsCaseUrn = caseId,
					DecisionId = decisionId,
					DecisionOutcomeId = updated.DecisionOutcomeId

				}
			};

			return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
		}
	}
}
