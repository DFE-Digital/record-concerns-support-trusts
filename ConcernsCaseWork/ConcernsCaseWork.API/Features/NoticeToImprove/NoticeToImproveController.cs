﻿using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
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
		private readonly IUseCase<long, DeleteNoticeToImproveResponse> _deleteNoticeToImproveByIdUseCase;

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
			IUseCase<long, DeleteNoticeToImproveResponse> deleteNoticeToImproveByIdUseCase,

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
			_deleteNoticeToImproveByIdUseCase = deleteNoticeToImproveByIdUseCase;
			_getAllStatuses = getAllStatuses;
			_getAllReasons = getAllReasons;
			_getAllConditions = getAllConditions;
			_patchNoticeToImproveUseCase = patchNoticeToImproveUseCase;
			_getAllConditionTypes = getAllConditionTypes;
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>>> Create(CreateNoticeToImproveRequest request, CancellationToken cancellationToken = default)
		{
			var createdNoticeToImprove = _createNoticeToImproveUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NoticeToImproveResponse>(createdNoticeToImprove);

			return CreatedAtAction(nameof(GetNoticeToImproveById), new { noticeToImproveId = createdNoticeToImprove.Id }, response);
		}

		[HttpGet]
		[Route("{noticeToImproveId}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>>> GetNoticeToImproveById(long noticeToImproveId, CancellationToken cancellationToken = default)
		{
			var noticeToImprove = _getNoticeToImproveByIdUseCase.Execute(noticeToImproveId);
			if (noticeToImprove == null)
				return NotFound();
			var response = new ApiSingleResponseV2<NoticeToImproveResponse>(noticeToImprove);
			return Ok(response);
		}

		[HttpDelete("{noticeToImproveId}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete(long noticeToImproveId, CancellationToken cancellationToken = default)
		{
			LogInfo($"Attempting to delete Notice To Improve matching Id {noticeToImproveId}");
			if (!ValidateNoticeToImproveId(noticeToImproveId, nameof(Delete)))
				return BadRequest();

			var noticeToImprove = _getNoticeToImproveByIdUseCase.Execute(noticeToImproveId);
			if (noticeToImprove == null)
			{
				LogInfo($"Deleting Notice To Improve matching failed: No Notice To Improve Matching Id {noticeToImproveId} was found");
				return NotFound();
			}

			_deleteNoticeToImproveByIdUseCase.Execute(noticeToImproveId);
			LogInfo($"Successfully Deleted Notice To Improve matching Id {noticeToImproveId}");

			return NoContent();
		}

		[HttpGet]
		[Route("case/{caseUrn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveResponse>>>> GetNoticesToImproveByCaseUrn(int caseUrn, CancellationToken cancellationToken = default)
		{
			var noticesToImprove = _getNoticeToImproveByCaseUrnUseCase.Execute(caseUrn);
			var response = new ApiSingleResponseV2<List<NoticeToImproveResponse>>(noticesToImprove);

			return Ok(response);
		}

		[HttpPatch]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NoticeToImproveResponse>>> Patch(PatchNoticeToImproveRequest request, CancellationToken cancellationToken = default)
		{
			var createdNoticeToImprove = _patchNoticeToImproveUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NoticeToImproveResponse>(createdNoticeToImprove);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveStatus>>>> GetAllStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = _getAllStatuses.Execute(null);
			var response = new ApiSingleResponseV2<List<NoticeToImproveStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-reasons")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveReason>>>> GetAllReasons(CancellationToken cancellationToken = default)
		{
			var reasons = _getAllReasons.Execute(null);
			var response = new ApiSingleResponseV2<List<NoticeToImproveReason>>(reasons);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-conditions")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveCondition>>>> GetAllConditions(CancellationToken cancellationToken = default)
		{
			var conditions = _getAllConditions.Execute(null);
			var response = new ApiSingleResponseV2<List<NoticeToImproveCondition>>(conditions);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-condition-types")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NoticeToImproveConditionType>>>> GetAllConditionTypes(CancellationToken cancellationToken = default)
		{
			var conditionTypes = _getAllConditionTypes.Execute(null);
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
			_logger.LogInformation($"{caller} {msg}");
		}
	}
}
