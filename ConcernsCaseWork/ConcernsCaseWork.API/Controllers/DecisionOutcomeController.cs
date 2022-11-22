using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases/{urn:int}/decisions/{decisionId:int}/outcome")]
	public class DecisionOutcomeController : ControllerBase
	{
		[HttpPost]
		[ApiVersion("2.0")]
		public IActionResult Create(int urn, int decisionId, CreateDecisionOutcomeRequest request)
		{
			return new ObjectResult("") { StatusCode = StatusCodes.Status201Created };
		}

		[HttpPut]
		[ApiVersion("2.0")]
		public IActionResult Put(int urn, int decisionId, UpdateDecisionOutcomeRequest request)
		{
			return new ObjectResult("") { StatusCode = StatusCodes.Status200OK };
		}
	}
}
