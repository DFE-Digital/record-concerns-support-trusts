using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.PolicyType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.Decision
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases/{concernsCaseUrn:int}/decisions/")]
	public class DecisionController(IMediator mediator) : ControllerBase
	{
		[HttpGet("{decisionId}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromRoute] GetByID.Query query)
		{
			var model = await mediator.Send(query);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(new ApiSingleResponseV2<GetDecisionResponse>(model));
		}

		[HttpPost(Name = "CreateDecision")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create([FromBody] CreateDecisionRequest request)
		{
			var command = new Create.Command(request);

			var commandResult = await mediator.Send(command);

			var response = new ApiSingleResponseV2<CreateDecisionResponse>(commandResult);

			return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
		}

		[HttpPut("{decisionId}")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update(int concernsCaseUrn, int decisionId, [FromBody] UpdateDecisionRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Update.Command(concernsCaseUrn, decisionId, request);
			var commandResult = await mediator.Send(command);

			var model = await mediator.Send(new GetByID.Query() { ConcernsCaseUrn = commandResult.ConcernsCaseUrn, DecisionId = commandResult.DecisionId });
			return Ok(new ApiSingleResponseV2<GetDecisionResponse>(model));

		}

		[HttpGet()]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> GetDecisions([FromRoute] ListByCaseUrn.Query query)
		{
			var decisions = await mediator.Send(query);

			return Ok(new ApiSingleResponseV2<DecisionSummaryResponse[]>(decisions));
		}

		[HttpPatch("{decisionId}/close")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Close(int concernsCaseUrn, int decisionId, [FromBody] CloseDecisionRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Close.Command(concernsCaseUrn, decisionId, request);
			var commandResult = await mediator.Send(command);

			return Ok(new ApiSingleResponseV2<CloseDecisionResponse>(commandResult));
		}

		[Authorize(Policy = Policy.CanDelete)]
		[HttpDelete("{decisionId:int}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken = default)
		{
			await mediator.Send(command);

			return NoContent();
		}
	}
}
