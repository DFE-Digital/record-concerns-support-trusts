using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.UseCases;
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

        public ConcernsCaseDecisionController(
            ILogger<ConcernsCaseDecisionController> logger,
            IUseCaseAsync<CreateDecisionRequest, CreateDecisionResponse> createDecisionUseCase,
            IUseCaseAsync<GetDecisionRequest, GetDecisionResponse> getDecisionUserCase
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createDecisionUseCase = createDecisionUseCase ?? throw new ArgumentNullException(nameof(createDecisionUseCase));
            _getDecisionUserCase = getDecisionUserCase ?? throw new ArgumentNullException(nameof(getDecisionUserCase));
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<CreateDecisionResponse>>> Create(int urn, CreateDecisionRequest request, CancellationToken cancellationToken)
        {
            LogInfo($"Executing Create. Urn {urn}");

            if (urn <= 0)
            {
                LogInfo($"Failed to create Concerns Case Decision - invalid urn value");
                return BadRequest();
            }

            if (request != null)
            {
                request.ConcernsCaseUrn = urn;
            }

            if(request == null || !request.IsValid())
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
        public async Task<ActionResult<ApiSingleResponseV2<GetDecisionResponse>>> GetById(int urn, int decisionId, CancellationToken cancellationToken)
        {
            LogInfo($"Attempting to get Concerns Decision by Urn {urn}, DecisionId {decisionId}");

            if (urn <= 0)
            {
                LogInfo($"Failed to GET Concerns Case Decision - invalid urn value");
                return BadRequest();
            }
            if (decisionId <= 0)
            {
                LogInfo($"Failed to GET Concerns Case Decision - invalid urn value");
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

        private void LogInfo(string msg, [CallerMemberName] string caller = "")
        {
            _logger.LogInformation($"{caller} {msg}");
        }
    }
}