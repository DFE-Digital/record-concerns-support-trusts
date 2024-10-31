using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.FinancialPlan;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.UserContext;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	[ApiVersion("2.0")]
	[Route("v{version:apiVersion}/case-actions/financial-plan")]
	[ApiController]
	public class FinancialPlanController : Controller
	{
		private readonly IMediator _mediator;
		private readonly IUseCase<int, List<FinancialPlanResponse>> _getFinancialPlansByCaseUseCase;
		private readonly IUseCase<object, List<Data.Models.FinancialPlanStatus>> _getAllStatuses;

		public FinancialPlanController(
			IMediator mediator,
			IUseCase<int, List<FinancialPlanResponse>> getFinancialPlansByCase,
			IUseCase<object, List<Data.Models.FinancialPlanStatus>> getAllStatuses)
		{
			_mediator = mediator;
			_getFinancialPlansByCaseUseCase = getFinancialPlansByCase;
			_getAllStatuses = getAllStatuses;
		}

		[HttpGet("{Id}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromRoute] GetByID.Query query)
		{
			var model = await _mediator.Send(query);

			if (model == null)
			{
				return NotFound();
			}

			return Ok(new ApiSingleResponseV2<FinancialPlanResponse>(model));
		}

		[HttpPost()]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create([FromBody] CreateFinancialPlanRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Create.Command(request);
			var commandResult = await _mediator.Send(command);
			var model = await _mediator.Send(new GetByID.Query() { Id = commandResult });
			return CreatedAtAction(nameof(GetByID), new { Id = model.Id }, new ApiSingleResponseV2<FinancialPlanResponse>(model));
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
		public async Task<ActionResult<ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>>> GetAllStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = _getAllStatuses.Execute(null);
			var response = new ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("closure-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>>> GetClosureStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = _getAllStatuses.Execute(null).Where(s => s.IsClosedStatus).ToList();
			var response = new ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("open-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>>> GetOpenStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = _getAllStatuses.Execute(null).Where(s => !s.IsClosedStatus).ToList();
			var response = new ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>(statuses);

			return Ok(response);
		}


		[HttpPatch]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update([FromBody] PatchFinancialPlanRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Update.Command(request);
			var commandResult = await _mediator.Send(command);
			var model = await _mediator.Send(new GetByID.Query() { Id = commandResult });
			return Ok(new ApiSingleResponseV2<FinancialPlanResponse>(model));
		}

		[Authorize(Policy = "CanDelete")]
		[HttpDelete("{Id}")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken = default)
		{
			await _mediator.Send(command);

			return NoContent();
		}
	}
}
