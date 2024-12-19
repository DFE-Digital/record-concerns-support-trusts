using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.FinancialPlan;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.UseCases;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	[ApiVersion("2.0")]
	[Route("v{version:apiVersion}/case-actions/financial-plan")]
	[ApiController]
	public class FinancialPlanController(
		IMediator mediator,
		IUseCase<int, List<FinancialPlanResponse>> getFinancialPlansByCase,
		IUseCase<object, List<Data.Models.FinancialPlanStatus>> getAllStatuses) : Controller
	{
		[HttpGet("{Id}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromRoute] GetByID.Query query)
		{
			var model = await mediator.Send(query);

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
			var commandResult = await mediator.Send(command);
			var model = await mediator.Send(new GetByID.Query() { Id = commandResult });
			return CreatedAtAction(nameof(GetByID), new { Id = model.Id }, new ApiSingleResponseV2<FinancialPlanResponse>(model));
		}

		[HttpGet]
		[Route("case/{caseUrn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<FinancialPlanResponse>>>> GetFinancialPlansByCaseId(int caseUrn, CancellationToken cancellationToken = default)
		{
			var fps = getFinancialPlansByCase.Execute(caseUrn);
			var response = new ApiSingleResponseV2<List<FinancialPlanResponse>>(fps);

			return Ok(response);
		}

		[HttpGet]
		[Route("all-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>>> GetAllStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = getAllStatuses.Execute(null);
			var response = new ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("closure-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>>> GetClosureStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = getAllStatuses.Execute(null).Where(s => s.IsClosedStatus).ToList();
			var response = new ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>(statuses);

			return Ok(response);
		}

		[HttpGet]
		[Route("open-statuses")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<List<Data.Models.FinancialPlanStatus>>>> GetOpenStatuses(CancellationToken cancellationToken = default)
		{
			var statuses = getAllStatuses.Execute(null).Where(s => !s.IsClosedStatus).ToList();
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
			var commandResult = await mediator.Send(command);
			var model = await mediator.Send(new GetByID.Query() { Id = commandResult });
			return Ok(new ApiSingleResponseV2<FinancialPlanResponse>(model));
		}

		[Authorize(Policy = Policy.CanDelete)]
		[HttpDelete("{Id}")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken = default)
		{
			await mediator.Send(command);

			return NoContent();
		}
	}
}
