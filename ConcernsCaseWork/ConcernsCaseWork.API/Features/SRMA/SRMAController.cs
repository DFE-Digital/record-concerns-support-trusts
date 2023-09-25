using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.Data.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;

namespace ConcernsCaseWork.API.Features.SRMA
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/case-actions/srma")]
	public class SRMAController : ControllerBase
	{
		private readonly ILogger<SRMAController> _logger;
		private readonly IMediator _mediator;

		public SRMAController(
			ILogger<SRMAController> logger,
			IMediator mediator)
		{
			_mediator = mediator;
			_logger = logger;
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create(CreateSRMARequest request, CancellationToken cancellationToken = default)
		{
			var command = new Create.Command(request);
			var commandResult = await _mediator.Send(command);
			var model = await _mediator.Send(new GetByID.Query() { srmaId = commandResult });
			return CreatedAtAction(nameof(GetByID), new { srmaId = model.Id }, new ApiSingleResponseV2<SRMAResponse>(model));
		}

		[HttpGet]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByID([FromQuery] GetByID.Query query)
		{
			var model = await _mediator.Send(query);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(new ApiSingleResponseV2<SRMAResponse>(model));
		}

		[HttpGet]
		[Route("case/{caseUrn}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> ListByCaseUrn([FromRoute] ListByCaseUrn.Query query)
		{
			var model = await _mediator.Send(query);
			return Ok(new ApiSingleResponseV2<ICollection<SRMAResponse>>(model));
		}

		[HttpPatch]
		[Route("{srmaId}/update-status")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> UpdateStatus(int srmaId, SRMAStatus status, CancellationToken cancellationToken = default)
		{
			var command = SRMA.UpdateStatus.Command.Create(srmaId, status);
			var commandResult = await _mediator.Send(command);
			return await GetByID(new GetByID.Query() { srmaId = commandResult });
		}

		[HttpPatch]
		[Route("{srmaId}/update-reason")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> UpdateReason(int srmaId, SRMAReasonOffered reason, CancellationToken cancellationToken = default)
		{
			var command = SRMA.UpdateReason.Command.Create(srmaId, reason);
			var commandResult = await _mediator.Send(command);
			return await GetByID(new GetByID.Query() { srmaId = commandResult });
		}

		[HttpPatch]
		[Route("{srmaId}/update-notes")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> UpdateSrmaNotes(int srmaId, [StringLength(SrmaConstants.NotesLength)] string notes, CancellationToken cancellationToken = default)
		{
			var command = UpdateNotes.Command.Create(srmaId, notes);
			var commandResult = await _mediator.Send(command);
			return await GetByID(new GetByID.Query() { srmaId = commandResult });
		}

		[HttpPatch]
		[Route("{srmaId}/update-closed-date")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> UpdateClosedDate([FromRoute] UpdateDateClosed.Command command, CancellationToken cancellationToken = default)
		{
			var commandResult = await _mediator.Send(command);
			return await GetByID(new GetByID.Query() { srmaId = commandResult });
		}

		[HttpPatch]
		[Route("{srmaId}/update-offered-date")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> UpdateOfferedDate(int srmaId, string offeredDate, CancellationToken cancellationToken = default)
		{
			try
			{
				DateTime? date = DeserialiseDateTime(offeredDate);

				if (date == null)
				{
					return BadRequest("Offered Date Cannot Be Null");
				}
				else
				{
					var command = SRMA.UpdateOfferedDate.Command.Create(srmaId, date.Value);
					var commandResult = await _mediator.Send(command);
					return await GetByID(new GetByID.Query() { srmaId = commandResult });
				}
			}
			catch (FormatException ex)
			{
				_logger.LogError(ex, "DateTime received doesn't conform to format");
				throw;
			}

		}

		[HttpPatch]
		[Route("{srmaId}/update-date-accepted")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> UpdateAcceptedDate(int srmaId, string acceptedDate, CancellationToken cancellationToken = default)
		{
			try
			{
				var command = UpdateDateAccepted.Command.Create(srmaId, DeserialiseDateTime(acceptedDate));
				var commandResult = await _mediator.Send(command);
				return await GetByID(new GetByID.Query() { srmaId = commandResult });
			}
			catch (FormatException ex)
			{
				_logger.LogError(ex, "DateTime received doesn't conform to format");
				throw;
			}

		}

		[HttpPatch]
		[Route("{srmaId}/update-date-report-sent")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> UpdateReportSentDate(int srmaId, string dateReportSent, CancellationToken cancellationToken = default)
		{
			try
			{
				var command = UpdateDateReportSent.Command.Create(srmaId, DeserialiseDateTime(dateReportSent));
				var commandResult = await _mediator.Send(command);
				return await GetByID(new GetByID.Query() { srmaId = commandResult });
			}
			catch (FormatException ex)
			{
				_logger.LogError(ex, "DateTime received doesn't conform to format");
				throw;
			}
		}
		[HttpPatch]
		[Route("{srmaId}/update-visit-dates")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> UpdateVisitDates(int srmaId, string startDate, string endDate, CancellationToken cancellationToken = default)
		{
			try
			{
				var command = SRMA.UpdateVisitDates.Command.Create(srmaId, DeserialiseDateTime(startDate), DeserialiseDateTime(endDate));
				var commandResult = await _mediator.Send(command);
				return await GetByID(new GetByID.Query() { srmaId = commandResult });
			}
			catch (FormatException ex)
			{
				_logger.LogError(ex, "DateTime received doesn't conform to format");
				throw;
			}
		}

		[HttpDelete("{srmaId}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete([FromRoute] Delete.Command command, CancellationToken cancellationToken = default)
		{
			await _mediator.Send(command);

			return NoContent();
		}

		private DateTime? DeserialiseDateTime(string value)
		{
			var dateTimeFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;
			return string.IsNullOrWhiteSpace(value) ? null : (DateTime?)DateTime.ParseExact(value, "dd-MM-yyyy", dateTimeFormatInfo, DateTimeStyles.None);
		}
	}
}
