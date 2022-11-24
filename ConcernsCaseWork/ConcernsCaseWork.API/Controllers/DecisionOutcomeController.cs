using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases/{urn:int}/decisions/{decisionId:int}/outcome")]
	public class DecisionOutcomeController : ControllerBase
	{
		IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse> _createDecisionOutcomeUseCase;

		public DecisionOutcomeController(
			IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse> createDecisionOutcomeUseCase)
		{
			_createDecisionOutcomeUseCase = createDecisionOutcomeUseCase;
		}

		[HttpPost]
		[ApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<CreateDecisionOutcomeResponse>>> Create(int urn, int decisionId, CreateDecisionOutcomeRequest request, CancellationToken cancellationToken)
		{
			var parameters = new CreateDecisionOutcomeUseCaseParams()
			{
				ConcernsCaseId = urn,
				DecisionId = decisionId,
				Request = request
			};

			var created = await _createDecisionOutcomeUseCase.Execute(parameters, cancellationToken);

			var result = new ApiSingleResponseV2<CreateDecisionOutcomeResponse>(created);

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
