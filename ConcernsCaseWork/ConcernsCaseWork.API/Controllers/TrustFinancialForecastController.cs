using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
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
	    private readonly IUseCaseAsync<GetTrustFinancialForecastByIdRequest, TrustFinancialForecastResponse> _getTrustFinancialForecast;
	    private readonly IUseCaseAsync<GetTrustFinancialForecastForCaseRequest, IEnumerable<TrustFinancialForecastResponse>> _getTrustFinancialForecastsForCase;
	    private readonly IUseCaseAsync<UpdateTrustFinancialForecastRequest, int> _updateTrustFinancialForecast;
	    private readonly IUseCaseAsync<CloseTrustFinancialForecastRequest, int> _closeFinancialTrustForecast;

	    public TrustFinancialForecastController(
		    ILogger<TrustFinancialForecastController> logger,
		    IUseCaseAsync<CreateTrustFinancialForecastRequest, int> createTrustFinancialForecast,
		    IUseCaseAsync<GetTrustFinancialForecastByIdRequest, TrustFinancialForecastResponse> getTrustFinancialForecast,
		    IUseCaseAsync<UpdateTrustFinancialForecastRequest, int>  updateTrustFinancialForecast, 
		    IUseCaseAsync<CloseTrustFinancialForecastRequest, int> closeFinancialTrustForecast, 
		    IUseCaseAsync<GetTrustFinancialForecastForCaseRequest, IEnumerable<TrustFinancialForecastResponse>> getTrustFinancialForecastsForCase)
	    {
		    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		    _createTrustFinancialForecast = createTrustFinancialForecast ?? throw new ArgumentNullException(nameof(createTrustFinancialForecast));
		    _getTrustFinancialForecast = getTrustFinancialForecast ?? throw new ArgumentNullException(nameof(getTrustFinancialForecast));
		    _updateTrustFinancialForecast = updateTrustFinancialForecast ?? throw new ArgumentNullException(nameof(updateTrustFinancialForecast));
		    _closeFinancialTrustForecast = closeFinancialTrustForecast ?? throw new ArgumentNullException(nameof(closeFinancialTrustForecast));
		    _getTrustFinancialForecastsForCase = getTrustFinancialForecastsForCase ?? throw new ArgumentNullException(nameof(getTrustFinancialForecastsForCase));
	    }

	    [HttpPost]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<string>>> Create(
		    [FromRoute] int caseUrn, 
		    [Required] CreateTrustFinancialForecastRequest request, 
		    CancellationToken cancellationToken)
	    {
		    LogInfo($"Executing Create Trust Financial Forecast for Case Urn {caseUrn}");
		    
		    if (!ValidateIdHasValue(caseUrn, nameof(Create)) || !request.IsValid())
		    {
			    LogInfo("Failed to create Trust Financial Forecast due to bad request");
			    return BadRequest();
		    }

		    var createdId = await _createTrustFinancialForecast.Execute(request, cancellationToken);
		    var response = new ApiSingleResponseV2<string>(createdId.ToString());

		    LogInfo($"Returning Id of created response. Case Urn {caseUrn}, Trust Financial Forecast Id {response}");
		    return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
	    }

	    [HttpGet("{id:int}")]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<TrustFinancialForecastResponse>>> GetById([FromRoute] int caseUrn, [FromRoute] int id, CancellationToken cancellationToken)
	    {
		    LogInfo($"Attempting to get Trust Financial Forecast by Urn {caseUrn}, Id {id}");
	    
		    var request = new GetTrustFinancialForecastByIdRequest{ CaseUrn = caseUrn, TrustFinancialForecastId = id};
		    if (!request.IsValid())
		    {
			    return BadRequest();
		    }
	    
		    var response = await _getTrustFinancialForecast.Execute(request, cancellationToken);
		    if (response == null)
		    {
			    return NotFound();
		    }
		    else
		    {
			    var actionResponse = new ApiSingleResponseV2<TrustFinancialForecastResponse>(response);
			    return Ok(actionResponse);
		    }
	    }
	    
	    [HttpGet]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<IEnumerable<TrustFinancialForecastResponse>>>> GetAllForCase([FromRoute] int caseUrn, CancellationToken cancellationToken)
	    {
		    LogInfo($"Attempting to get Trust Financial Forecast by Urn {caseUrn}");
		    var request = new GetTrustFinancialForecastForCaseRequest{ CaseUrn = caseUrn };
            if (!request.IsValid())
            {
                return BadRequest();
            }

		    var response = await _getTrustFinancialForecastsForCase.Execute(request, cancellationToken);
		    if (response == null)
		    {
			    return NotFound();
		    }
		    else
		    {
			    var actionResponse = new ApiSingleResponseV2<IEnumerable<TrustFinancialForecastResponse>>(response);
			    return Ok(actionResponse);
		    }
	    }
	    
	    [HttpPut("{id:int}")]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<string>>> Update(int caseUrn, int id, UpdateTrustFinancialForecastRequest request, CancellationToken cancellationToken)
	    {
		    LogInfo($"Attempting to update Decision by Urn {caseUrn}, DecisionId {id}");
	    
		    if (!ValidateIdHasValue(caseUrn, nameof(Update))
		        || !ValidateIdHasValue(id, nameof(Update))
		        || !request.IsValid())
		    {
			    return BadRequest();
		    }
	    
		    var updatedId = await _updateTrustFinancialForecast.Execute(request, cancellationToken);
		    var response = new ApiSingleResponseV2<string>(updatedId.ToString());
		    return new OkObjectResult(response);
	    }
	    
	    [HttpPatch("{id:int}")]
	    [MapToApiVersion("2.0")]
	    public async Task<ActionResult<ApiSingleResponseV2<string>>> Close(int caseUrn, int id, CloseTrustFinancialForecastRequest request, CancellationToken cancellationToken)
	    {
		    LogInfo($"Attempting to update Decision by Urn {caseUrn}, DecisionId {id}");
	    
		    if (!ValidateIdHasValue(caseUrn, nameof(Close))
		        || !ValidateIdHasValue(id, nameof(Close))
		        || !request.IsValid())
		    {
			    return BadRequest();
		    }
	    
		    var closedId = await _closeFinancialTrustForecast.Execute(request, cancellationToken);
		    var response = new ApiSingleResponseV2<string>(closedId.ToString());
		    return new OkObjectResult(response);
	    }

	    private bool ValidateIdHasValue(int id, string methodName)
	    {
		    if (id > 0) return true;
		    
		    LogInfo($"{methodName} found invalid Id value");
		    return false;
	    }

	    private void LogInfo(string msg, [CallerMemberName] string caller = "")
	    {
		    _logger.LogInformation("{Caller} {Msg}", caller, msg);
	    }
    }
}