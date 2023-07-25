using ConcernsCaseWork.API.Features.CityTechnicalCollege;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-records")]
	public class ConcernsRecordController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ConcernsRecordController(IMediator mediator)
		{
			_mediator = mediator;
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

		[HttpGet("{Id}:int", Name = nameof(GetByID))]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromRoute] GetByID.Query query)
		{
			var model = await _mediator.Send(query);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(model);
		}
	}
}
