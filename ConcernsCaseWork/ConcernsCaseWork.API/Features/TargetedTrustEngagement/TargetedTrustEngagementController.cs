using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.TargetedTrustEngagement
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases/{concernsCaseUrn:int}/targetedtrustengagement/")]
	public class TargetedTrustEngagementController(IMediator mediator) : ControllerBase
	{
		[HttpGet("{targetedtrustengagementId}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromRoute] GetByID.Query query)
		{
			var model = await mediator.Send(query);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(new ApiSingleResponseV2<GetTargetedTrustEngagementResponse>(model));
		}

		[HttpPost(Name = "CreateTargetedTrustEngagement")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create([FromBody] CreateTargetedTrustEngagementRequest request)
		{
			var command = new Create.Command(request);

			var commandResult = await mediator.Send(command);

			var response = new ApiSingleResponseV2<CreateTargetedTrustEngagementResponse>(commandResult);

			return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
		}

		[HttpPut("{targetedTrustEngagementId}")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update(int concernsCaseUrn, int targetedTrustEngagementId, [FromBody] UpdateTargetedTrustEngagementRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Update.Command(concernsCaseUrn, targetedTrustEngagementId, request);
			var commandResult = await mediator.Send(command);

			var model = await mediator.Send(new GetByID.Query() { ConcernsCaseUrn = commandResult.ConcernsCaseUrn, TargetedTrustEngagementId = commandResult.TargetedTrustEngagementId });
			return Ok(new ApiSingleResponseV2<GetTargetedTrustEngagementResponse>(model));

		}

		[HttpGet()]
		public async Task<IActionResult> GetTargetedTrustEngagements([FromRoute] ListByCaseUrn.Query query)
		{
			var targetedTrustEngagement = await mediator.Send(query);

			return Ok(new ApiSingleResponseV2<TargetedTrustEngagementSummaryResponse[]>(targetedTrustEngagement));
		}

		[HttpPatch("{targetedTrustEngagementId}/close")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Close(int concernsCaseUrn, int targetedTrustEngagementId, [FromBody] CloseTargetedTrustEngagementRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Close.Command(concernsCaseUrn, targetedTrustEngagementId, request);
			var commandResult = await mediator.Send(command);

			return Ok(new ApiSingleResponseV2<CloseTargetedTrustEngagementResponse>(commandResult));
		}

		[Authorize(Policy = Policy.CanDelete)]
		[HttpDelete("{targetedTrustEngagementId:int}")]
		public async Task<IActionResult> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken = default)
		{
			await mediator.Send(command);

			return NoContent();
		}
	}
}
