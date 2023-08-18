using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using MediatR;
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
		public FinancialPlanController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{Id}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromRoute]GetByID.Query query)
		{
			var model = await _mediator.Send(query);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(new ApiSingleResponseV2<GetByID.Result>(model));
		}

		[HttpPost()]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create([FromBody] Create.Command command, CancellationToken cancellationToken = default)
		{
			var commandResult = await _mediator.Send(command);
			var model = await _mediator.Send(new GetByID.Query() { Id = commandResult });
			return CreatedAtAction(nameof(GetByID), new { Id = model.Id }, new ApiSingleResponseV2<GetByID.Result>(model));
		}


		[HttpPatch]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update([FromBody] Update.Command command, CancellationToken cancellationToken = default)
		{
			var commandResult = await _mediator.Send(command);
			var model = await _mediator.Send(new GetByID.Query() { Id = commandResult });
			return Ok(new ApiSingleResponseV2<GetByID.Result>(model));
		}

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
