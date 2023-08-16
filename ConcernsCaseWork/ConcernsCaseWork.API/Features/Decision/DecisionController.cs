using ConcernsCaseWork.API.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.Decision
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases/{concernsCaseUrn:int}/decisions/")]
	public class DecisionController : ControllerBase
	{
		private readonly IMediator _mediator;

		public DecisionController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{decisionId}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromRoute] GetByID.Query query)
		{
			var model = await _mediator.Send(query);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(new ApiSingleResponseV2<GetByID.Result>(model));
		}

		[HttpPost(Name = "CreateDecision")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create([FromBody] Create.Command command)
		{
			var commandResult = await _mediator.Send(command);

			var response = new ApiSingleResponseV2<Create.CommandResult>(commandResult);

			return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
		}

		[HttpPut("{decisionId}")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update(int concernsCaseUrn, int decisionId, [FromBody] Update.DecisionModel putModel, CancellationToken cancellationToken = default)
		{
			var command = new Update.Command(concernsCaseUrn, decisionId, putModel);
			var commandResult = await _mediator.Send(command);

			var model = await _mediator.Send(new GetByID.Query() { ConcernsCaseUrn = commandResult.ConcernsCaseUrn, DecisionId = commandResult.DecisionId });
			return Ok(new ApiSingleResponseV2<GetByID.Result>(model));

		}

		[HttpPatch("{decisionId}/close")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Close(int concernsCaseUrn, int decisionId, [FromBody] Close.CloseDecisionModel closeModel, CancellationToken cancellationToken = default)
		{
			var command = new Close.Command(concernsCaseUrn, decisionId, closeModel);
			var commandResult = await _mediator.Send(command);

			var model = await _mediator.Send(new GetByID.Query() { ConcernsCaseUrn = commandResult.CaseUrn, DecisionId = commandResult.DecisionId });

			return Ok(new ApiSingleResponseV2<Close.CommandResult>(commandResult));
		}
	}
}
