using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/case-actions/financial-plan")]
    [ApiController]
    public class FinancialPlanController : Controller
    {
        private readonly ILogger<FinancialPlanController> _logger;
        private readonly IUseCase<CreateFinancialPlanRequest, FinancialPlanResponse> _createFinancialPlanUseCase;
        private readonly IUseCase<long, FinancialPlanResponse> _getFinancialPlanByIdUseCase;
        private readonly IUseCase<int, List<FinancialPlanResponse>> _getFinancialPlansByCaseUseCase;
        private readonly IUseCase<PatchFinancialPlanRequest, FinancialPlanResponse> _patchFinancialPlanUseCase;
        private readonly IUseCase<object, List<FinancialPlanStatus>> _getAllStatuses;

        public FinancialPlanController(ILogger<FinancialPlanController> logger,
            IUseCase<CreateFinancialPlanRequest, FinancialPlanResponse> createFinancialPlanUseCase,
            IUseCase<long, FinancialPlanResponse> getFinancialPlanByIdUseCase,
            IUseCase<int, List<FinancialPlanResponse>> getFinancialPlansByCase,
            IUseCase<PatchFinancialPlanRequest, FinancialPlanResponse> patchFinancialPlan,
            IUseCase<Object, List<FinancialPlanStatus>> getAllStatuses)
        {
            _logger = logger;
            _createFinancialPlanUseCase = createFinancialPlanUseCase;
            _getFinancialPlanByIdUseCase = getFinancialPlanByIdUseCase;
            _getFinancialPlansByCaseUseCase = getFinancialPlansByCase;
            _patchFinancialPlanUseCase = patchFinancialPlan;
            _getAllStatuses = getAllStatuses;
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<FinancialPlanResponse>>> Create(CreateFinancialPlanRequest request, CancellationToken cancellationToken = default)
        {
            var createdFP = _createFinancialPlanUseCase.Execute(request);
            var response = new ApiSingleResponseV2<FinancialPlanResponse>(createdFP);

            return CreatedAtAction(nameof(GetFinancialPlanById), new { financialPlanId = createdFP.Id}, response);
        }

        [HttpGet]
        [Route("{financialPlanId}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<FinancialPlanResponse>>> GetFinancialPlanById(long financialPlanId, CancellationToken cancellationToken = default)
        {
            var fp = _getFinancialPlanByIdUseCase.Execute(financialPlanId);
            var response = new ApiSingleResponseV2<FinancialPlanResponse>(fp);

            return Ok(response);
        }

        [HttpGet]
        [Route("case/{caseUrn}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<List<FinancialPlanResponse>>>> GetFinancialPlansByCaseId(int caseUrn, CancellationToken cancellationToken = default)
        {
            var fps = _getFinancialPlansByCaseUseCase.Execute(caseUrn);
            var response = new ApiSingleResponseV2<List<FinancialPlanResponse>>(fps);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-statuses")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<List<FinancialPlanStatus>>>> GetAllStatuses(CancellationToken cancellationToken = default)
        {
            var statuses = _getAllStatuses.Execute(null);
            var response = new ApiSingleResponseV2<List<FinancialPlanStatus>>(statuses);

            return Ok(response);
        }
        
        [HttpGet]
        [Route("closure-statuses")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<List<FinancialPlanStatus>>>> GetClosureStatuses(CancellationToken cancellationToken = default)
        {
            var statuses = _getAllStatuses.Execute(null).Where(s => s.IsClosedStatus).ToList();
            var response = new ApiSingleResponseV2<List<FinancialPlanStatus>>(statuses);

            return Ok(response);
        }
        
        [HttpGet]
        [Route("open-statuses")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<List<FinancialPlanStatus>>>> GetOpenStatuses(CancellationToken cancellationToken = default)
        {
            var statuses = _getAllStatuses.Execute(null).Where(s => !s.IsClosedStatus).ToList();
            var response = new ApiSingleResponseV2<List<FinancialPlanStatus>>(statuses);

            return Ok(response);
        }

        [HttpPatch]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<FinancialPlanResponse>>> Patch(PatchFinancialPlanRequest request, CancellationToken cancellationToken = default)
        {
            var createdFP = _patchFinancialPlanUseCase.Execute(request);
            var response = new ApiSingleResponseV2<FinancialPlanResponse>(createdFP);

            return Ok(response);
        }
    }
}
