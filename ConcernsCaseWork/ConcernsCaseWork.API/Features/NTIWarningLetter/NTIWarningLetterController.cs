using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
{
	[ApiVersion("2.0")]
	[Authorize(Policy = Policy.Default)]
	[Route("v{version:apiVersion}/case-actions/nti-warning-letter")]
	[ApiController]
	public class NTIWarningLetterController(ILogger<NTIWarningLetterController> logger,
		IUseCase<CreateNTIWarningLetterRequest, NTIWarningLetterResponse> createNtiWarningLetterUseCase,
		IUseCase<long, NTIWarningLetterResponse> getNtiWarningLetterByIdUseCase,
		IUseCase<int, List<NTIWarningLetterResponse>> getNtiWarningLetterByCaseUrnUseCase,
		IUseCase<PatchNTIWarningLetterRequest, NTIWarningLetterResponse> patchNTIWarningLetterUseCase,
		IUseCase<long, DeleteNTIWarningLetterResponse> deleteNTIWarningLetterUseCase,
		IUseCase<object, List<NTIWarningLetterStatus>> getAllStatuses,
		IUseCase<object, List<NTIWarningLetterReason>> getAllReasons,
		IUseCase<object, List<NTIWarningLetterCondition>> getAllConditions,
		IUseCase<object, List<NTIWarningLetterConditionType>> getAllConditionTypes
			) : Controller
	{
		[HttpPost]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<NTIWarningLetterResponse>> Create(CreateNTIWarningLetterRequest request, CancellationToken cancellationToken = default)
		{
			var createdWarningLetter = createNtiWarningLetterUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NTIWarningLetterResponse>(createdWarningLetter);

			return CreatedAtAction(nameof(GetNTIWarningLetterById), new { warningLetterId = createdWarningLetter.Id }, response);
		}

		[HttpGet]
		[Route("{warningLetterId}")]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<NTIWarningLetterResponse>> GetNTIWarningLetterById(long warningLetterId, CancellationToken cancellationToken = default)
		{
			var warningLetter = getNtiWarningLetterByIdUseCase.Execute(warningLetterId);
			if (warningLetter == null)
				return NotFound();
			var response = new ApiSingleResponseV2<NTIWarningLetterResponse>(warningLetter);
			return Ok(response);
		}

		[HttpGet]
		[Route("case/{caseUrn}")]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterResponse>>> GetNtiWarningLetterByCaseUrn(int caseUrn, CancellationToken cancellationToken = default)
		{
			var warningLetters = getNtiWarningLetterByCaseUrnUseCase.Execute(caseUrn);
			var response = new ApiSingleResponseV2<List<NTIWarningLetterResponse>>(warningLetters);

			return Ok(response);
		}

		[HttpPatch]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<NTIWarningLetterResponse>> Patch(PatchNTIWarningLetterRequest request, CancellationToken cancellationToken = default)
		{
			var createdWarningLetter = patchNTIWarningLetterUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NTIWarningLetterResponse>(createdWarningLetter);

			return Ok(response);
		}

		[Authorize(Policy = Policy.CanDelete)]
		[HttpDelete("{warningLetterId}")]
		[MapToApiVersion("2.0")]
		public IActionResult Delete(long warningLetterId, CancellationToken cancellationToken = default)
		{
			LogInfo($"Attempting to delete NTI Warning Letter matching Id {warningLetterId}");

			if (!ValidateWarningLetterId(warningLetterId, nameof(Delete)))
				return BadRequest();

			var warningLetter = getNtiWarningLetterByIdUseCase.Execute(warningLetterId);
			if (warningLetter == null)
			{
				LogInfo($"Deleting NTI Warning Letter matching failed: No Warning Letter Matching Id {warningLetterId} was found");

				return NotFound();
			}

			deleteNTIWarningLetterUseCase.Execute(warningLetterId);
			LogInfo($"Successfully Deleted NTI Warning Letter matching Id {warningLetterId}");

			return NoContent();
		}



		[HttpGet]
		[Route("all-statuses")]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterStatus>>> GetAllStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = getAllStatuses.Execute(null);
			var response = new ApiSingleResponseV2<List<NTIWarningLetterStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-reasons")]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterReason>>> GetAllReasons(CancellationToken cancellationToken = default)
		{
			var reasons = getAllReasons.Execute(null);
			var response = new ApiSingleResponseV2<List<NTIWarningLetterReason>>(reasons);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-conditions")]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<List<NTIWarningLetterCondition>>> GetAllConditions(CancellationToken cancellationToken = default)
		{
			var conditions = getAllConditions.Execute(null);
			var response = new ApiSingleResponseV2<List<NTIWarningLetterCondition>>(conditions);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-condition-types")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NTIWarningLetterConditionType>>>> GetAllConditionTypes(CancellationToken cancellationToken = default)
		{
			var conditionTypes = getAllConditionTypes.Execute(null);
			var response = new ApiSingleResponseV2<List<NTIWarningLetterConditionType>>(conditionTypes);

			return Ok(response);
		}

		private bool ValidateWarningLetterId(long warningletterId, string methodName)
		{
			if (warningletterId <= 0)
			{
				LogInfo($"{methodName} found invalid warningletterId value");
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
