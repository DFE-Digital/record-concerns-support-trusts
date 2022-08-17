using Concerns.Data.Models;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
	[Route("v{version:apiVersion}/case-actions/nti-warning-letter")]
    [ApiController]
    public class NTIWarningLetterController : Controller
    {
        private readonly ILogger<NTIWarningLetterController> _logger;
        private readonly IUseCase<CreateNTIWarningLetterRequest, NTIWarningLetterResponse> _createNtiWarningLetterUseCase;
        private readonly IUseCase<long, NTIWarningLetterResponse> _getNtiWarningLetterByIdUseCase;
        private readonly IUseCase<int, List<NTIWarningLetterResponse>> _getNtiWarningLetterByCaseUrnUseCase;
        private readonly IUseCase<PatchNTIWarningLetterRequest, NTIWarningLetterResponse> _patchNTIWarningLetterUseCase;
        private readonly IUseCase<object, List<NTIWarningLetterStatus>> _getAllStatuses;
        private readonly IUseCase<object, List<NTIWarningLetterReason>> _getAllReasons;
        private readonly IUseCase<object, List<NTIWarningLetterCondition>> _getAllConditions;
        private readonly IUseCase<object, List<NTIWarningLetterConditionType>> _getAllConditionTypes;

        public NTIWarningLetterController(ILogger<NTIWarningLetterController> logger,
            IUseCase<CreateNTIWarningLetterRequest, NTIWarningLetterResponse> createNtiWarningLetterUseCase,
            IUseCase<long, NTIWarningLetterResponse> getNtiWarningLetterByIdUseCase,
            IUseCase<int, List<NTIWarningLetterResponse>> getNtiWarningLetterByCaseUrnUseCase,
            IUseCase<PatchNTIWarningLetterRequest, NTIWarningLetterResponse> patchNTIWarningLetterUseCase,
            IUseCase<object, List<NTIWarningLetterStatus>> getAllStatuses,
            IUseCase<object, List<NTIWarningLetterReason>> getAllReasons,
            IUseCase<object, List<NTIWarningLetterCondition>> getAllConditions,
            IUseCase<object, List<NTIWarningLetterConditionType>> getAllConditionTypes
            )
        {
            _logger = logger;
            _createNtiWarningLetterUseCase = createNtiWarningLetterUseCase;
            _getNtiWarningLetterByIdUseCase = getNtiWarningLetterByIdUseCase;
            _getNtiWarningLetterByCaseUrnUseCase = getNtiWarningLetterByCaseUrnUseCase;

            _getAllStatuses = getAllStatuses;
            _getAllReasons = getAllReasons;
            _getAllConditions = getAllConditions;
            _patchNTIWarningLetterUseCase = patchNTIWarningLetterUseCase;
            _getAllConditionTypes = getAllConditionTypes;
        }

        [HttpPost]
        public ActionResult<ApiSingleResponseV2<NTIWarningLetterResponse>> Create(CreateNTIWarningLetterRequest request)
        {
            var createdWarningLetter = _createNtiWarningLetterUseCase.Execute(request);
            var response = new ApiSingleResponseV2<NTIWarningLetterResponse>(createdWarningLetter);

            return CreatedAtAction(nameof(GetNTIWarningLetterById), new { warningLetterId = createdWarningLetter.Id}, response);
        }

        [HttpGet]
        [Route("{warningLetterId}")]
        public ActionResult<ApiSingleResponseV2<NTIWarningLetterResponse>> GetNTIWarningLetterById(long warningLetterId)
        {
            var warningLetter = _getNtiWarningLetterByIdUseCase.Execute(warningLetterId);
            var response = new ApiSingleResponseV2<NTIWarningLetterResponse>(warningLetter);

            return Ok(response);
        }

        [HttpGet]
        [Route("case/{caseUrn}")]
        public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterResponse>>> GetNtiWarningLetterByCaseUrn(int caseUrn)
        {
            var warningLetters = _getNtiWarningLetterByCaseUrnUseCase.Execute(caseUrn);
            var response = new ApiSingleResponseV2<List<NTIWarningLetterResponse>>(warningLetters);

            return Ok(response);
        }

        [HttpPatch]
        public ActionResult<ApiSingleResponseV2<NTIWarningLetterResponse>> Patch(PatchNTIWarningLetterRequest request)
        {
            var createdWarningLetter = _patchNTIWarningLetterUseCase.Execute(request);
            var response = new ApiSingleResponseV2<NTIWarningLetterResponse>(createdWarningLetter);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-statuses")]
        public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterStatus>>> GetAllStatuses()
        {
            var statuses = _getAllStatuses.Execute(null);
            var response = new ApiSingleResponseV2<List<NTIWarningLetterStatus>>(statuses);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-reasons")]
        public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterReason>>> GetAllReasons()
        {
            var reasons = _getAllReasons.Execute(null);
            var response = new ApiSingleResponseV2<List<NTIWarningLetterReason>>(reasons);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-conditions")]
        public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterCondition>>> GetAllConditions()
        {
            var conditions = _getAllConditions.Execute(null);
            var response = new ApiSingleResponseV2<List<NTIWarningLetterCondition>>(conditions);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-condition-types")]
        public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterConditionType>>> GetAllConditionTypes()
        {
            var conditionTypes = _getAllConditionTypes.Execute(null);
            var response = new ApiSingleResponseV2<List<NTIWarningLetterConditionType>>(conditionTypes);

            return Ok(response);
        }
    }
}
