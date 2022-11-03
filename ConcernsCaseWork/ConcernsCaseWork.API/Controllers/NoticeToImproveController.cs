using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/case-actions/notice-to-improve")]
    [ApiController]
    public class NoticeToImproveController : Controller
    {
        private readonly ILogger<NoticeToImproveController> _logger;
        private readonly IUseCase<CreateNoticeToImproveRequest, NoticeToImproveResponse> _createNoticeToImproveUseCase;
        private readonly IUseCase<long, NoticeToImproveResponse> _getNoticeToImproveByIdUseCase;
        private readonly IUseCase<int, List<NoticeToImproveResponse>> _getNoticeToImproveByCaseUrnUseCase;
        private readonly IUseCase<PatchNoticeToImproveRequest, NoticeToImproveResponse> _patchNoticeToImproveUseCase;
        private readonly IUseCase<object, List<NoticeToImproveStatus>> _getAllStatuses;
        private readonly IUseCase<object, List<NoticeToImproveReason>> _getAllReasons;
        private readonly IUseCase<object, List<NoticeToImproveCondition>> _getAllConditions;
        private readonly IUseCase<object, List<NoticeToImproveConditionType>> _getAllConditionTypes;

        public NoticeToImproveController(ILogger<NoticeToImproveController> logger,
            IUseCase<CreateNoticeToImproveRequest, NoticeToImproveResponse> createNoticeToImproveUseCase,
            IUseCase<long, NoticeToImproveResponse> getNoticeToImproveByIdUseCase,
            IUseCase<int, List<NoticeToImproveResponse>> getNoticeToImproveByCaseUrnUseCase,
            IUseCase<PatchNoticeToImproveRequest, NoticeToImproveResponse> patchNoticeToImproveUseCase,
            IUseCase<object, List<NoticeToImproveStatus>> getAllStatuses,
            IUseCase<object, List<NoticeToImproveReason>> getAllReasons,
            IUseCase<object, List<NoticeToImproveCondition>> getAllConditions,
            IUseCase<object, List<NoticeToImproveConditionType>> getAllConditionTypes
            )
        {
            _logger = logger;
            _createNoticeToImproveUseCase = createNoticeToImproveUseCase;
            _getNoticeToImproveByIdUseCase = getNoticeToImproveByIdUseCase;
            _getNoticeToImproveByCaseUrnUseCase = getNoticeToImproveByCaseUrnUseCase;
            _getAllStatuses = getAllStatuses;
            _getAllReasons = getAllReasons;
            _getAllConditions = getAllConditions;
            _patchNoticeToImproveUseCase = patchNoticeToImproveUseCase;
            _getAllConditionTypes = getAllConditionTypes;
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>> Create(CreateNoticeToImproveRequest request)
        {
            var createdNoticeToImprove = _createNoticeToImproveUseCase.Execute(request);
            var response = new ApiSingleResponseV2<NoticeToImproveResponse>(createdNoticeToImprove);

            return CreatedAtAction(nameof(GetNoticeToImproveById), new { noticeToImproveId = createdNoticeToImprove.Id}, response);
        }

        [HttpGet]
        [Route("{noticeToImproveId}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>> GetNoticeToImproveById(long noticeToImproveId)
        {
            var warningLetter = _getNoticeToImproveByIdUseCase.Execute(noticeToImproveId);
            var response = new ApiSingleResponseV2<NoticeToImproveResponse>(warningLetter);

            return Ok(response);
        }

        [HttpGet]
        [Route("case/{caseUrn}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NoticeToImproveResponse>>> GetNoticesToImproveByCaseUrn(int caseUrn)
        {
            var noticesToImprove = _getNoticeToImproveByCaseUrnUseCase.Execute(caseUrn);
            var response = new ApiSingleResponseV2<List<NoticeToImproveResponse>>(noticesToImprove);

            return Ok(response);
        }

        [HttpPatch]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>> Patch(PatchNoticeToImproveRequest request)
        {
            var createdNoticeToImprove = _patchNoticeToImproveUseCase.Execute(request);
            var response = new ApiSingleResponseV2<NoticeToImproveResponse>(createdNoticeToImprove);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-statuses")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NoticeToImproveStatus>>> GetAllStatuses()
        {
            var statuses = _getAllStatuses.Execute(null);
            var response = new ApiSingleResponseV2<List<NoticeToImproveStatus>>(statuses);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-reasons")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NoticeToImproveReason>>> GetAllReasons()
        {
            var reasons = _getAllReasons.Execute(null);
            var response = new ApiSingleResponseV2<List<NoticeToImproveReason>>(reasons);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-conditions")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NoticeToImproveCondition>>> GetAllConditions()
        {
            var conditions = _getAllConditions.Execute(null);
            var response = new ApiSingleResponseV2<List<NoticeToImproveCondition>>(conditions);

            return Ok(response);
        }

        [HttpGet]
        [Route("all-condition-types")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<List<NoticeToImproveConditionType>>> GetAllConditionTypes()
        {
            var conditionTypes = _getAllConditionTypes.Execute(null);
            var response = new ApiSingleResponseV2<List<NoticeToImproveConditionType>>(conditionTypes);

            return Ok(response);
        }
    }
}
