using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/case-actions/nti-under-consideration")]
    [ApiController]
    public class NTIUnderConsiderationController : Controller
    {
        private readonly ILogger<NTIUnderConsiderationController> _logger;
        private readonly IUseCase<CreateNTIUnderConsiderationRequest, NTIUnderConsiderationResponse> _createNtiUnderConsiderationUseCase;
        private readonly IUseCase<long, NTIUnderConsiderationResponse> _getNtiUnderConsiderationByIdUseCase;
        private readonly IUseCase<int, List<NTIUnderConsiderationResponse>> _getNtiUnderConsiderationByCaseUrnUseCase;
        private readonly IUseCase<PatchNTIUnderConsiderationRequest, NTIUnderConsiderationResponse> _patchNTIUnderConsiderationUseCase;
        private readonly IUseCase<object, List<NTIUnderConsiderationStatus>> _getAllStatuses;
        private readonly IUseCase<object, List<NTIUnderConsiderationReason>> _getAllReasons;

        public NTIUnderConsiderationController(ILogger<NTIUnderConsiderationController> logger,
            IUseCase<CreateNTIUnderConsiderationRequest, NTIUnderConsiderationResponse> createNtiUnderConsiderationUseCase,
            IUseCase<long, NTIUnderConsiderationResponse> getNtiUnderConsiderationByIdUseCase,
            IUseCase<int, List<NTIUnderConsiderationResponse>> getNtiUnderConsiderationByCaseUrnUseCase,
            IUseCase<PatchNTIUnderConsiderationRequest, NTIUnderConsiderationResponse> patchNTIUnderConsiderationUseCase,
            IUseCase<object, List<NTIUnderConsiderationStatus>> getAllStatuses,
            IUseCase<object, List<NTIUnderConsiderationReason>> getAllReasons
            )
        {
            _logger = logger;
            _createNtiUnderConsiderationUseCase = createNtiUnderConsiderationUseCase;
            _getNtiUnderConsiderationByIdUseCase = getNtiUnderConsiderationByIdUseCase;
            _getNtiUnderConsiderationByCaseUrnUseCase = getNtiUnderConsiderationByCaseUrnUseCase;
            _patchNTIUnderConsiderationUseCase = patchNTIUnderConsiderationUseCase;
            _getAllStatuses = getAllStatuses;
            _getAllReasons = getAllReasons;
        }

  
        [HttpPost]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<NTIUnderConsiderationResponse>> Create(CreateNTIUnderConsiderationRequest request)
        {
            var createdConsideration = _createNtiUnderConsiderationUseCase.Execute(request);
            var response = new ApiSingleResponseV2<NTIUnderConsiderationResponse>(createdConsideration);

            return CreatedAtAction(nameof(GetNTIUnderConsiderationById), new { underConsiderationId = createdConsideration.Id}, response);
        }

        [HttpGet]
        [Route("{underConsiderationId}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<NTIUnderConsiderationResponse>> GetNTIUnderConsiderationById(long underConsiderationId)
        {
            var consideration = _getNtiUnderConsiderationByIdUseCase.Execute(underConsiderationId);
            var response = new ApiSingleResponseV2<NTIUnderConsiderationResponse>(consideration);

            return Ok(response);
        }

        [HttpGet]
        [Route("case/{caseUrn}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NTIUnderConsiderationResponse>>> GetNtiUnderConsiderationByCaseUrn(int caseUrn)
        {
            var considerations = _getNtiUnderConsiderationByCaseUrnUseCase.Execute(caseUrn);
            var response = new ApiSingleResponseV2<List<NTIUnderConsiderationResponse>>(considerations);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-statuses")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NTIUnderConsiderationStatus>>> GetAllStatuses()
        {
            var statuses = _getAllStatuses.Execute(null);
            var response = new ApiSingleResponseV2<List<NTIUnderConsiderationStatus>>(statuses);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-reasons")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NTIUnderConsiderationReason>>> GetAllReasons()
        {
            var reasons = _getAllReasons.Execute(null);
            var response = new ApiSingleResponseV2<List<NTIUnderConsiderationReason>>(reasons);

            return Ok(response);
        }

        [HttpPatch]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<NTIUnderConsiderationResponse>> Patch(PatchNTIUnderConsiderationRequest request)
        {
            var createdConsideration = _patchNTIUnderConsiderationUseCase.Execute(request);
            var response = new ApiSingleResponseV2<NTIUnderConsiderationResponse>(createdConsideration);

            return Ok(response);  
        }
    }
}
