using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/case-actions/nti-under-consideration")]
	public class NTIUnderConsiderationController : ControllerBase
	{
		private readonly ILogger<NTIUnderConsiderationController> _logger;
		private readonly IMediator _mediator;

		public NTIUnderConsiderationController(
			ILogger<NTIUnderConsiderationController> logger,
			IMediator mediator)
		{
			_mediator = mediator;
			_logger = logger;
		}

		[HttpDelete("{underConsiderationId}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken = default)
		{
			await _mediator.Send(command);

			return NoContent();
		}
	}
}
