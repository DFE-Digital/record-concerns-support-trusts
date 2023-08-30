using ConcernsCaseWork.API.Features.CityTechnicalCollege;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

		[HttpGet("case/urn/{urn}")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		public async Task<IActionResult> GetByID([FromRoute] ListByCaseUrn.Query query)
		{
			var model = await _mediator.Send(query);
			return Ok(model);
		}

		[HttpPatch("{Id}", Name = nameof(Update))]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update(int Id, [FromBody] Update.ConcernModel patchModel, CancellationToken cancellationToken = default)
		{
			var command = new Update.Command(Id, patchModel);
			var commandResult = await _mediator.Send(command);
			var model = await _mediator.Send(new GetByID.Query() { Id = commandResult });
			return Ok(new ApiSingleResponseV2<GetByID.Result>(model));
		}


		[HttpDelete("{id}")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> Delete([FromRoute] Delete.Query query)
		{
			var model = await _mediator.Send(new GetByID.Query() { Id = query.Id });
			if (model == null)
			{
				return NotFound();
			}
			await _mediator.Send(query);
			return NoContent();
		}
	}
}
