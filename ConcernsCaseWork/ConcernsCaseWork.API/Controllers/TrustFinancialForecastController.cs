using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.CaseActions.Decisions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-cases/{caseUrn:int}/trustfinancialforecast")]
    public class TrustFinancialForecastController : ControllerBase
    {
	    private readonly ILogger<TrustFinancialForecastController> _logger;
	    private readonly IUseCaseAsync<CreateTrustFinancialForecastRequest, int> _createTrustFinancialForecast;
	    private readonly IUseCaseAsync<GetDecisionRequest, GetDecisionResponse> _getDecisionUserCase;
	    private readonly IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]> _getDecisionsUserCase;
	    private readonly IUseCaseAsync<(int urn, int decisionId, UpdateDecisionRequest), UpdateDecisionResponse> _updateDecisionUseCase;
	    private readonly IUseCaseAsync<DecisionUseCaseRequestParams<CloseDecisionRequest>, CloseDecisionResponse> _closeDecisionUseCase;

	    public TrustFinancialForecastController(
		    ILogger<TrustFinancialForecastController> logger,
		    IUseCaseAsync<CreateTrustFinancialForecastRequest, int> createTrustFinancialForecast,
		    IUseCaseAsync<GetDecisionRequest, GetDecisionResponse> getDecisionUseCase,
		    IUseCaseAsync<GetDecisionsRequest, DecisionSummaryResponse[]> getDecisionsUseCase,
		    IUseCaseAsync<(int urn, int decisionId, UpdateDecisionRequest), UpdateDecisionResponse> updateDecisionUseCase, 
		    IUseCaseAsync<DecisionUseCaseRequestParams<CloseDecisionRequest>, CloseDecisionResponse> closeDecisionUseCase)
	    {
		    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		    _createTrustFinancialForecast = createTrustFinancialForecast ?? throw new ArgumentNullException(nameof(createTrustFinancialForecast));
		    _getDecisionUserCase = getDecisionUseCase ?? throw new ArgumentNullException(nameof(getDecisionUseCase));
		    _getDecisionsUserCase = getDecisionsUseCase ?? throw new ArgumentNullException(nameof(getDecisionsUseCase));
		    _updateDecisionUseCase = updateDecisionUseCase ?? throw new ArgumentNullException(nameof(updateDecisionUseCase));
		    _closeDecisionUseCase = closeDecisionUseCase ?? throw new ArgumentNullException(nameof(closeDecisionUseCase));
	    }

	    [HttpPost]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<string>>> Create(
		    [FromRoute] int caseUrn, 
		    [Required] CreateTrustFinancialForecastRequest request, 
		    CancellationToken cancellationToken)
	    {
		    LogInfo($"Executing Create Trust Financial Forecast for Case Urn {caseUrn}");
		    
		    if (!ValidateUrn(caseUrn, nameof(Create)) || !request.IsValid())
		    {
			    LogInfo($"Failed to create Trust Financial Forecast due to bad request");
			    return BadRequest();
		    }

		    var createdId = await _createTrustFinancialForecast.Execute(request, cancellationToken);
		    var response = new ApiSingleResponseV2<string>(createdId.ToString());

		    LogInfo($"Returning Id of created response. Case Urn {caseUrn}, Trust Financial Forecast Id {response}");
		    return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
	    }

	    // [HttpGet("{decisionId:int}")]
	    // [MapToApiVersion("2.0")]
	    // public async Task<ActionResult<ApiSingleResponseV2<GetDecisionResponse>>> GetById(int urn, int decisionId, CancellationToken cancellationToken)
	    // {
		   //  LogInfo($"Attempting to get Concerns Decision by Urn {urn}, DecisionId {decisionId}");
	    //
		   //  if (!ValidateUrn(urn, nameof(GetById)) || !ValidateDecisionId(decisionId, nameof(GetById)))
		   //  {
			  //   return BadRequest();
		   //  }
	    //
		   //  var decisionResponse = await _getDecisionUserCase.Execute(new GetDecisionRequest(urn, decisionId), cancellationToken);
		   //  if (decisionResponse == null)
		   //  {
			  //   LogInfo($" returning NotFound");
			  //   return NotFound();
		   //  }
		   //  else
		   //  {
			  //   LogInfo($" returning OK Response");
			  //   var actionResponse = new ApiSingleResponseV2<GetDecisionResponse>(decisionResponse);
			  //   return Ok(actionResponse);
		   //  }
	    // }
	    //
	    // [HttpPut("{decisionId:int}")]
	    // [MapToApiVersion("2.0")]
	    // public async Task<ActionResult<ApiSingleResponseV2<UpdateDecisionResponse>>> UpdateDecision(int urn, int decisionId, UpdateDecisionRequest request, CancellationToken cancellationToken)
	    // {
		   //  LogInfo($"Attempting to update Decision by Urn {urn}, DecisionId {decisionId}");
	    //
		   //  if (!ValidateUrn(urn, nameof(GetById))
		   //      || !ValidateDecisionId(decisionId, nameof(GetById))
		   //      || !request.IsValid())
		   //  {
			  //   return BadRequest();
		   //  }
	    //
		   //  var result = await _updateDecisionUseCase.Execute((urn, decisionId, request), cancellationToken);
		   //  var response = new ApiSingleResponseV2<UpdateDecisionResponse>(result);
	    //
		   //  LogInfo($"Returning update response. Concerns Case Urn {result.ConcernsCaseUrn}, DecisionId {result.DecisionId}");
		   //  return new OkObjectResult(response);
	    // }
	    //
	    // [HttpPatch("{decisionId:int}/close")]
	    // [MapToApiVersion("2.0")]
	    // public async Task<ActionResult<ApiSingleResponseV2<CloseDecisionResponse>>> CloseDecision(int urn, int decisionId, CloseDecisionRequest request, CancellationToken cancellationToken)
	    // {
		   //  LogInfo($"Attempting to update Decision by Urn {urn}, DecisionId {decisionId}");
	    //
		   //  if (!ValidateUrn(urn, nameof(CloseDecision))
		   //      || !ValidateDecisionId(decisionId, nameof(CloseDecision))
		   //      || !request.IsValid())
		   //  {
			  //   return BadRequest();
		   //  }
	    //
		   //  var closeDecisionRequest = new DecisionUseCaseRequestParams<CloseDecisionRequest>(urn, decisionId, request);
		   //  var result = await _closeDecisionUseCase.Execute(closeDecisionRequest, cancellationToken);
		   //  var response = new ApiSingleResponseV2<CloseDecisionResponse>(result);
	    //
		   //  LogInfo($"Returning close decision response. Concerns Case Urn {result.CaseUrn}, DecisionId {result.DecisionId}");
		   //  return new OkObjectResult(response);
	    // }
	    //
	    // [HttpGet()]
	    // [MapToApiVersion("2.0")]
	    // public async Task<ActionResult<ApiSingleResponseV2<DecisionSummaryResponse[]>>> GetDecisions(int urn, CancellationToken cancellationToken)
	    // {
		   //  LogInfo($"Entered {nameof(GetDecisions)}, Urn {urn}");
	    //
		   //  if (!ValidateUrn(urn, nameof(GetDecisions)))
		   //  {
			  //   return BadRequest();
		   //  }
	    //
		   //  var results = await _getDecisionsUserCase.Execute(new GetDecisionsRequest(urn), cancellationToken);
		   //  if (results is null)
		   //  {
			  //   return NotFound();
		   //  }
	    //
		   //  return Ok(new ApiSingleResponseV2<DecisionSummaryResponse[]>(results));
	    // }

	    private bool ValidateUrn(int urn, string methodName)
	    {
		    if (urn <= 0)
		    {
			    LogInfo($"{methodName} found invalid urn value");
			    return false;
		    }

		    return true;
	    }

	    private bool ValidateDecisionId(int decisionId, string methodName)
	    {
		    if (decisionId <= 0)
		    {
			    LogInfo($"{methodName} found invalid decisionId value");
			    return false;
		    }

		    return true;
	    }

	    private void LogInfo(string msg, [CallerMemberName] string caller = "")
	    {
		    _logger.LogInformation($"{caller} {msg}");
	    }
    }
}