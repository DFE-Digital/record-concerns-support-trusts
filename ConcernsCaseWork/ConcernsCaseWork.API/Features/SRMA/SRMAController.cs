﻿using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases.CaseActions.SRMA;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.API.Contracts.Srma;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
		public async Task<IActionResult> Create(Create.Command command, CancellationToken cancellationToken = default)
		{
			var commandResult = await _mediator.Send(command);
			var model = await _mediator.Send(new GetByID.Query() { srmaId = commandResult });
			return CreatedAtAction(nameof(GetByID), new { srmaId = model.Id }, new ApiSingleResponseV2<GetByID.Result>(model));
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

			return Ok(new ApiSingleResponseV2<GetByID.Result>(model));
		}

		[HttpPatch]
		[Route("{srmaId}/update-status")]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Update(int srmaId, SRMAStatus status, CancellationToken cancellationToken = default)
		{
			var command = UpdateStatus.Command.Create(srmaId, status);
			var commandResult = await _mediator.Send(command);
			return await GetByID(new GetByID.Query() { srmaId = commandResult });
		}

		[HttpPatch]
		[Route("{srmaId}/update-reason")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Update(int srmaId, SRMAReasonOffered reason, CancellationToken cancellationToken = default)
		{
			var command = UpdateReason.Command.Create(srmaId, reason);
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
		public async Task<IActionResult> Update([FromRoute]UpdateDateClosed.Command command, CancellationToken cancellationToken = default)
		{
			var commandResult = await _mediator.Send(command);
			return await GetByID(new GetByID.Query() { srmaId = commandResult });
		}

		[HttpPatch]
		[Route("{srmaId}/update-offered-date")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Update(int srmaId, string offeredDate, CancellationToken cancellationToken = default)
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
					var command = UpdateOfferedDate.Command.Create(srmaId, date.Value);
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
		public async Task<IActionResult> Update(int srmaId, string startDate, string endDate, CancellationToken cancellationToken = default)
		{
			try
			{
				var command = UpdateVisitDates.Command.Create(srmaId, DeserialiseDateTime(startDate), DeserialiseDateTime(endDate));
				var commandResult = await _mediator.Send(command);
				return await GetByID(new GetByID.Query() { srmaId = commandResult });
			}
			catch (FormatException ex)
			{
				_logger.LogError(ex, "DateTime received doesn't conform to format");
				throw;
			}
		}

		private DateTime? DeserialiseDateTime(string value)
		{
			var dateTimeFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;
			return string.IsNullOrWhiteSpace(value) ? null : (DateTime?)DateTime.ParseExact(value, "dd-MM-yyyy", dateTimeFormatInfo, DateTimeStyles.None);
		}
	}
}