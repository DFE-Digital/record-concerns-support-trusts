using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
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
		private readonly IMediator _mediator;

		public NTIUnderConsiderationController(ILogger<NTIUnderConsiderationController> logger,
			IUseCase<CreateNTIUnderConsiderationRequest, NTIUnderConsiderationResponse> createNtiUnderConsiderationUseCase,
			IUseCase<long, NTIUnderConsiderationResponse> getNtiUnderConsiderationByIdUseCase,
			IUseCase<int, List<NTIUnderConsiderationResponse>> getNtiUnderConsiderationByCaseUrnUseCase,
			IUseCase<PatchNTIUnderConsiderationRequest, NTIUnderConsiderationResponse> patchNTIUnderConsiderationUseCase,
			IUseCase<object, List<NTIUnderConsiderationStatus>> getAllStatuses,
			IUseCase<object, List<NTIUnderConsiderationReason>> getAllReasons,
			IMediator mediator)
		{
			_logger = logger;
			_createNtiUnderConsiderationUseCase = createNtiUnderConsiderationUseCase;
			_getNtiUnderConsiderationByIdUseCase = getNtiUnderConsiderationByIdUseCase;
			_getNtiUnderConsiderationByCaseUrnUseCase = getNtiUnderConsiderationByCaseUrnUseCase;
			_patchNTIUnderConsiderationUseCase = patchNTIUnderConsiderationUseCase;
			_getAllStatuses = getAllStatuses;
			_getAllReasons = getAllReasons;
			_mediator = mediator;
		}


		[HttpPost]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NTIUnderConsiderationResponse>>> Create(CreateNTIUnderConsiderationRequest request, CancellationToken cancellationToken = default)
		{
			var createdConsideration = _createNtiUnderConsiderationUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NTIUnderConsiderationResponse>(createdConsideration);

			return CreatedAtAction(nameof(GetNTIUnderConsiderationById), new { underConsiderationId = createdConsideration.Id }, response);
		}

		[HttpGet]
		[Route("{underConsiderationId}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NTIUnderConsiderationResponse>>> GetNTIUnderConsiderationById(long underConsiderationId, CancellationToken cancellationToken = default)
		{
			var consideration = _getNtiUnderConsiderationByIdUseCase.Execute(underConsiderationId);
			if (consideration == null)
				return NotFound();
			var response = new ApiSingleResponseV2<NTIUnderConsiderationResponse>(consideration);

			return Ok(response);
		}

		[HttpGet]
		[Route("case/{caseUrn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NTIUnderConsiderationResponse>>>> GetNtiUnderConsiderationByCaseUrn(int caseUrn, CancellationToken cancellationToken = default)
		{
			var considerations = _getNtiUnderConsiderationByCaseUrnUseCase.Execute(caseUrn);
			var response = new ApiSingleResponseV2<List<NTIUnderConsiderationResponse>>(considerations);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NTIUnderConsiderationStatus>>>> GetAllStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = _getAllStatuses.Execute(null);
			var response = new ApiSingleResponseV2<List<NTIUnderConsiderationStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-reasons")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<NTIUnderConsiderationReason>>>> GetAllReasons(CancellationToken cancellationToken = default)
		{
			var reasons = _getAllReasons.Execute(null);
			var response = new ApiSingleResponseV2<List<NTIUnderConsiderationReason>>(reasons);

			return Ok(response);
		}

		[HttpPatch]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<NTIUnderConsiderationResponse>>> Patch(PatchNTIUnderConsiderationRequest request, CancellationToken cancellationToken = default)
		{
			var createdConsideration = _patchNTIUnderConsiderationUseCase.Execute(request);
			var response = new ApiSingleResponseV2<NTIUnderConsiderationResponse>(createdConsideration);

			return Ok(response);
		}

		[Authorize(Policy = "CanDelete")]
		[HttpDelete("{underConsiderationId}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken = default)
		{
			await _mediator.Send(command);

			return NoContent();
		}
	}
}
