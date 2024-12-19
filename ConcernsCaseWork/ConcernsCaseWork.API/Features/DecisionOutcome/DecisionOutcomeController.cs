using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.PolicyType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.Decision.Outcome
{
	[ApiVersion("2.0")]
	[ApiController]
	[Authorize(Policy = Policy.Default)]
	[Route("v{version:apiVersion}/concerns-cases/{concernsCaseUrn:int}/decisions/{decisionId:int}/outcome")]
	public class DecisionController(IMediator mediator) : ControllerBase
	{
		[HttpPost(Name = "CreateDecisionOutcome")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create(int concernsCaseUrn, int decisionId, [FromBody] CreateDecisionOutcomeRequest request)
		{
			var command = new Create.Command(concernsCaseUrn, decisionId, request);

			var commandResult = await mediator.Send(command);

			var response = new ApiSingleResponseV2<CreateDecisionOutcomeResponse>(commandResult);

			return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
		}

		[HttpPut]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update(int concernsCaseUrn, int decisionId, [FromBody] UpdateDecisionOutcomeRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Update.Command(concernsCaseUrn, decisionId, request);
			var commandResult = await mediator.Send(command);

			return Ok(new ApiSingleResponseV2<UpdateDecisionOutcomeResponse>(commandResult));
		}
	}
}
