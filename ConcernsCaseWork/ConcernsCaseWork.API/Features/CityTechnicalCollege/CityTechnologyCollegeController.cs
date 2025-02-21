using ConcernsCaseWork.API.Contracts.Trusts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ConcernsCaseWork.API.Features.CityTechnicalCollege
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/citytechnologycolleges")]
	public class CityTechnologyCollegeController(IMediator mediator) : ControllerBase
	{

		/// <summary>
		/// Get a list of city technology colleges
		/// </summary>
		/// <returns>A list of city technology colleges containing the ID, Name, UKPRN, Companies House Number and Address</returns>
		[HttpGet(Name = nameof(List))]
		[MapToApiVersion("2.0")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		public async Task<IActionResult> List([FromQuery]List.Query query)
		{
			var model = await mediator.Send(query);
			return Ok(model.Items);
		}

		/// <summary>
		/// Get an individual city technology college based on the UKPRN
		/// </summary>
		/// <param name="query">Property Reference parameter</param>
		/// <returns>A city technology college containing the ID, Name, UKPRN, Companies House Number and Address</returns>
		[HttpGet("ukprn/{ukprn}", Name = nameof(GetByUKPRN))]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> GetByUKPRN([FromRoute]GetByUKPRN.Query query)
		{
			var model = await mediator.Send(query);

			// Our caller does not know if the UKPRN exists
			// We need to return a 204 instead of 404 in the case of null and let the client decide
			// Otherwise our logs will get a large amount of errors raised
			return Ok(model);
		}

		[HttpPost(Name = nameof(Create))]
		[ProducesResponseType((int)HttpStatusCode.Created)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Create([FromBody] CityTechnologyCollege request)
		{
			var command = new Create.Command(request);
			var response = await mediator.Send(command);
			return CreatedAtAction(nameof(GetByUKPRN), new { request.UKPRN }, null);
		}
	}
}
