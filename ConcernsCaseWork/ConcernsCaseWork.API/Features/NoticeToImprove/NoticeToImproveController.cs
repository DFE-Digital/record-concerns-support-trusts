using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	[ApiVersion("2.0")]
	[Authorize(Policy = Policy.Default)]
	[Route("v{version:apiVersion}/case-actions/notice-to-improve")]
	[ApiController]
	public class NoticeToImproveController(ILogger<NoticeToImproveController> logger,
		IUseCase<CreateNoticeToImproveRequest, NoticeToImproveResponse> createNoticeToImproveUseCase,
		IUseCase<long, NoticeToImproveResponse> getNoticeToImproveByIdUseCase,
		IUseCase<int, List<NoticeToImproveResponse>> getNoticeToImproveByCaseUrnUseCase,
		IUseCase<PatchNoticeToImproveRequest, NoticeToImproveResponse> patchNoticeToImproveUseCase,
		IUseCase<long, DeleteNoticeToImproveResponse> deleteNoticeToImproveByIdUseCase,
		IUseCase<object, List<NoticeToImproveStatus>> getAllStatuses,
		IUseCase<object, List<NoticeToImproveReason>> getAllReasons,
		IUseCase<object, List<NoticeToImproveCondition>> getAllConditions,
		IUseCase<object, List<NoticeToImproveConditionType>> getAllConditionTypes
			) : Controller
	{
		[HttpPost]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>>> Create(CreateNoticeToImproveRequest request, CancellationToken cancellationToken = default)
		{
			var createdNoticeToImprove = createNoticeToImproveUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NoticeToImproveResponse>(createdNoticeToImprove);

			return CreatedAtAction(nameof(GetNoticeToImproveById), new { noticeToImproveId = createdNoticeToImprove.Id }, response);
		}

		[HttpGet]
		[Route("{noticeToImproveId}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>>> GetNoticeToImproveById(long noticeToImproveId, CancellationToken cancellationToken = default)
		{
			var noticeToImprove = getNoticeToImproveByIdUseCase.Execute(noticeToImproveId);
			if (noticeToImprove == null)
				return NotFound();
			var response = new ApiSingleResponseV2<NoticeToImproveResponse>(noticeToImprove);
			return Ok(response);
		}

		[Authorize(Policy = Policy.CanDelete)]
		[HttpDelete("{noticeToImproveId}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete(long noticeToImproveId, CancellationToken cancellationToken = default)
		{
			LogInfo($"Attempting to delete Notice To Improve matching Id {noticeToImproveId}");
			if (!ValidateNoticeToImproveId(noticeToImproveId, nameof(Delete)))
				return BadRequest();

			var noticeToImprove = getNoticeToImproveByIdUseCase.Execute(noticeToImproveId);
			if (noticeToImprove == null)
			{
				LogInfo($"Deleting Notice To Improve matching failed: No Notice To Improve Matching Id {noticeToImproveId} was found");
				return NotFound();
			}

			deleteNoticeToImproveByIdUseCase.Execute(noticeToImproveId);
			LogInfo($"Successfully Deleted Notice To Improve matching Id {noticeToImproveId}");

			return NoContent();
		}

		[HttpGet]
		[Route("case/{caseUrn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveResponse>>>> GetNoticesToImproveByCaseUrn(int caseUrn, CancellationToken cancellationToken = default)
		{
			var noticesToImprove = getNoticeToImproveByCaseUrnUseCase.Execute(caseUrn);
			var response = new ApiSingleResponseV2<List<NoticeToImproveResponse>>(noticesToImprove);

			return Ok(response);
		}

		[HttpPatch]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>>> Patch(PatchNoticeToImproveRequest request, CancellationToken cancellationToken = default)
		{
			var createdNoticeToImprove = patchNoticeToImproveUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NoticeToImproveResponse>(createdNoticeToImprove);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveStatus>>>> GetAllStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = getAllStatuses.Execute(null);
			var response = new ApiSingleResponseV2<List<NoticeToImproveStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-reasons")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveReason>>>> GetAllReasons(CancellationToken cancellationToken = default)
		{
			var reasons = getAllReasons.Execute(null);
			var response = new ApiSingleResponseV2<List<NoticeToImproveReason>>(reasons);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-conditions")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveCondition>>>> GetAllConditions(CancellationToken cancellationToken = default)
		{
			var conditions = getAllConditions.Execute(null);
			var response = new ApiSingleResponseV2<List<NoticeToImproveCondition>>(conditions);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-condition-types")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveConditionType>>>> GetAllConditionTypes(CancellationToken cancellationToken = default)
		{
			var conditionTypes = getAllConditionTypes.Execute(null);
			var response = new ApiSingleResponseV2<List<NoticeToImproveConditionType>>(conditionTypes);

			return Ok(response);
		}


		private bool ValidateNoticeToImproveId(long noticeToImproveId, string methodName)
		{
			if (noticeToImproveId <= 0)
			{
				LogInfo($"{methodName} found invalid noticeToImproveId value");
				return false;
			}

			return true;
		}

		private void LogInfo(string msg, [CallerMemberName] string caller = "")
		{
			logger.LogInformation($"{caller} {msg}");
		}
	}
}
