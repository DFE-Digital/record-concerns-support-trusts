﻿using ConcernsCaseWork.API.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.Decision
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases/{urn:int}/decisions/")]
	public class DecisionController : ControllerBase
	{
		private readonly IMediator _mediator;

		public DecisionController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Get a list of city technology colleges
		/// </summary>
		/// <returns>A list of city technology colleges containing the ID, Name, UKPRN, Companies House Number and Address</returns>

		/*
		[HttpGet(Name = nameof(List))]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		public async Task<IActionResult> List([FromQuery]List.Query query)
		{
			var model = await _mediator.Send(query);
			return Ok(model.Items);
		}


		/// <summary>
		/// Get an individual city technology college based on the UKPRN
		/// </summary>
		/// <param name="query">Property Reference parameter</param>
		/// <returns>A city technology college containing the ID, Name, UKPRN, Companies House Number and Address</returns>


		[HttpGet("ukprn/{ukprn}", Name = nameof(GetByUKPRN))]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> GetByUKPRN([FromRoute]GetByUKPRN.Query query)
		{
			var model = await _mediator.Send(query);
			if (model == null)
			{
				return NotFound();
			}

			return Ok(model);
		}

		*/

		[HttpPost(Name = "CreateDecision")]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create([FromBody] Create.Command command)
		{
			var commandResult = await _mediator.Send(command);

			var response = new ApiSingleResponseV2<Create.CommandResult>(commandResult);

			return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
		}
	}
}
