using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
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
		public async Task<IActionResult> Create([FromBody] ConcernsRecordRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Create.Command(request);
			var commandResult = await _mediator.Send(command);
			var record = await _mediator.Send(new GetByID.Query() { Id = commandResult });
			return CreatedAtAction(nameof(GetByID), new { Id = record.Id }, new ApiSingleResponseV2<ConcernsRecordResponse>(record));
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
		public async Task<IActionResult> GetByCaseId([FromRoute] ListByCaseUrn.Query query)
		{
			var records = await _mediator.Send(query);

			var response = new ApiResponseV2<ConcernsRecordResponse>(records, null);
			return Ok(response);
		}

		[HttpPatch("{Id}", Name = nameof(Update))]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update(int Id, [FromBody] ConcernsRecordRequest request, CancellationToken cancellationToken = default)
		{
			var command = new Update.Command(Id, request);
			var commandResult = await _mediator.Send(command);

			var result = await _mediator.Send(new GetByID.Query() { Id = commandResult });
			return Ok(new ApiSingleResponseV2<ConcernsRecordResponse>(result));
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
