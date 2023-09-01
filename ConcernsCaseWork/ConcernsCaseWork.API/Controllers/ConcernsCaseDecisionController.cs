using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-cases/{urn:int}/decisions/")]
    public class ConcernsCaseDecisionController : ControllerBase
    {
	    private readonly ILogger<ConcernsCaseDecisionController> _logger;
	    private readonly IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse> _createDecisionUseCase;
	    private readonly IUseCaseAsync<GetDecisionRequest, GetDecisionResponse> _getDecisionUserCase;
	    private readonly IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]> _getDecisionsUserCase;
	    private readonly IUseCaseAsync<(int urn, int decisionId, UpdateDecisionRequest), UpdateDecisionResponse> _updateDecisionUseCase;
	    private readonly IUseCaseAsync<DecisionUseCaseRequestParams<CloseDecisionRequest>, CloseDecisionResponse> _closeDecisionUseCase;
		private readonly IUseCaseAsync<DeleteDecisionRequest, DeleteDecisionResponse> _deleteDecision;


		public ConcernsCaseDecisionController(
		    ILogger<ConcernsCaseDecisionController> logger,
		    IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse> createDecisionUseCase,
		    IUseCaseAsync<GetDecisionRequest, GetDecisionResponse> getDecisionUseCase,
		    IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]> getDecisionsUseCase,
		    IUseCaseAsync<(int urn, int decisionId, UpdateDecisionRequest), UpdateDecisionResponse> updateDecisionUseCase, 
		    IUseCaseAsync<DecisionUseCaseRequestParams<CloseDecisionRequest>, CloseDecisionResponse> closeDecisionUseCase,
			IUseCaseAsync<DeleteDecisionRequest, DeleteDecisionResponse> deleteDecisionUsedCase)
	    {
		    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		    _createDecisionUseCase = createDecisionUseCase ?? throw new ArgumentNullException(nameof(createDecisionUseCase));
		    _getDecisionUserCase = getDecisionUseCase ?? throw new ArgumentNullException(nameof(getDecisionUseCase));
		    _getDecisionsUserCase = getDecisionsUseCase ?? throw new ArgumentNullException(nameof(getDecisionsUseCase));
		    _updateDecisionUseCase = updateDecisionUseCase ?? throw new ArgumentNullException(nameof(updateDecisionUseCase));
		    _closeDecisionUseCase = closeDecisionUseCase ?? throw new ArgumentNullException(nameof(closeDecisionUseCase));
			_deleteDecision = deleteDecisionUsedCase ?? throw new ArgumentNullException(nameof(deleteDecisionUsedCase));
		}

		/*

		[HttpPost]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<CreateDecisionResponse>>> Create(int urn, CreateDecisionRequest request, CancellationToken cancellationToken = default)
	    {
		    LogInfo($"Executing Create. Urn {urn}");

		    if (!ValidateUrn(urn, nameof(Create)))
		    {
			    return BadRequest();
		    }

		    if (request != null)
		    {
			    request.ConcernsCaseUrn = urn;
		    }

		    if (request == null || !request.IsValid())
		    {
			    LogInfo($"Failed to create Concerns Case Decision due to bad request");
			    return BadRequest();
		    }

		    var createdDecision = await _createDecisionUseCase.Execute(request, cancellationToken);
		    var response = new ApiSingleResponseV2<CreateDecisionResponse>(createdDecision);

		    LogInfo($"Returning created response. Concerns Case Urn {response.Data.ConcernsCaseUrn}, DecisionId {response.Data.DecisionId}");
		    return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
	    }




	    [HttpGet("{decisionId:int}")]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<GetDecisionResponse>>> GetById(int urn, int decisionId, CancellationToken cancellationToken = default)
	    {
		    LogInfo($"Attempting to get Concerns Decision by Urn {urn}, DecisionId {decisionId}");

		    if (!ValidateUrn(urn, nameof(GetById)) || !ValidateDecisionId(decisionId, nameof(GetById)))
		    {
			    return BadRequest();
		    }

		    var decisionResponse = await _getDecisionUserCase.Execute(new GetDecisionRequest(urn, decisionId), cancellationToken);
		    if (decisionResponse == null)
		    {
			    LogInfo($" returning NotFound");
			    return NotFound();
		    }
		    else
		    {
			    LogInfo($" returning OK Response");
			    var actionResponse = new ApiSingleResponseV2<GetDecisionResponse>(decisionResponse);
			    return Ok(actionResponse);
		    }
	    }



		[HttpPut("{decisionId:int}")]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<UpdateDecisionResponse>>> UpdateDecision(int urn, int decisionId, UpdateDecisionRequest request, CancellationToken cancellationToken = default)
	    {
		    LogInfo($"Attempting to update Decision by Urn {urn}, DecisionId {decisionId}");

		    if (!ValidateUrn(urn, nameof(GetById))
		        || !ValidateDecisionId(decisionId, nameof(GetById))
		        || !request.IsValid())
		    {
			    return BadRequest();
		    }

		    var result = await _updateDecisionUseCase.Execute((urn, decisionId, request), cancellationToken);
		    var response = new ApiSingleResponseV2<UpdateDecisionResponse>(result);

		    LogInfo($"Returning update response. Concerns Case Urn {result.ConcernsCaseUrn}, DecisionId {result.DecisionId}");
		    return new OkObjectResult(response);
	    }


		[HttpPatch("{decisionId:int}/close")]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<CloseDecisionResponse>>> CloseDecision(int urn, int decisionId, CloseDecisionRequest request, CancellationToken cancellationToken = default)
	    {
		    LogInfo($"Attempting to update Decision by Urn {urn}, DecisionId {decisionId}");

		    if (!ValidateUrn(urn, nameof(CloseDecision))
		        || !ValidateDecisionId(decisionId, nameof(CloseDecision))
		        || !request.IsValid())
		    {
			    return BadRequest();
		    }

		    var closeDecisionRequest = new DecisionUseCaseRequestParams<CloseDecisionRequest>(urn, decisionId, request);
		    var result = await _closeDecisionUseCase.Execute(closeDecisionRequest, cancellationToken);
		    var response = new ApiSingleResponseV2<CloseDecisionResponse>(result);

		    LogInfo($"Returning close decision response. Concerns Case Urn {result.CaseUrn}, DecisionId {result.DecisionId}");
		    return new OkObjectResult(response);
	    }

		*/


		//[HttpGet()]
	 //   [MapToApiVersion("2.0")]
	 //   public async Task<ActionResult<ApiSingleResponseV2<DecisionSummaryResponse[]>>> GetDecisions(int urn, CancellationToken cancellationToken = default)
	 //   {
		//    LogInfo($"Entered {nameof(GetDecisions)}, Urn {urn}");

		//    if (!ValidateUrn(urn, nameof(GetDecisions)))
		//    {
		//	    return BadRequest();
		//    }

		//    var results = await _getDecisionsUserCase.Execute(new GetDecisionsRequest(urn), cancellationToken);
		//    if (results is null)
		//    {
		//	    return NotFound();
		//    }

		//    return Ok(new ApiSingleResponseV2<DecisionSummaryResponse[]>(results));
	 //   }

		//[HttpDelete("{decisionId:int}")]
		//[MapToApiVersion("2.0")]
		//public async Task<IActionResult> Delete(int urn, int decisionId, CancellationToken cancellationToken = default)
		//{
		//	LogInfo($"Attempting to delete Concerns Decision by Urn {urn}, DecisionId {decisionId}");

		//	if (!ValidateUrn(urn, nameof(Delete)) || !ValidateDecisionId(decisionId, nameof(Delete)))
		//	{
		//		return BadRequest();
		//	}

		//	var decisionResponse = await _getDecisionUserCase.Execute(new GetDecisionRequest(urn, decisionId), cancellationToken);
		//	if (decisionResponse == null)
		//	{
		//		LogInfo($"Deleting Concern Decision failed: No Decision matching Urn {urn}, DecisionId {decisionId} was found");

		//		return NotFound();
		//	}

		//	await _deleteDecision.Execute(new DeleteDecisionRequest(urn, decisionId), cancellationToken);
		//	LogInfo($"Successfully Deleted Concern Decision By Urn {urn}, DecisionId {decisionId}");

		//	return NoContent();
		//}




	    //private void LogInfo(string msg, [CallerMemberName] string caller = "")
	    //{
		   // _logger.LogInformation($"{caller} {msg}");
	    //}
    }
}