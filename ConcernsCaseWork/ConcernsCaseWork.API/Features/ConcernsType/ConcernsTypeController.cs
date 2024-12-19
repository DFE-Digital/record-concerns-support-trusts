using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.Features.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Features.ConcernsType
{
	[ApiVersion("2.0")]
	[ApiController]
	[Authorize(Policy = Policy.Default)]
	[Route("v{version:apiVersion}/concerns-types")]
	public class ConcernsTypeController(
		ILogger<ConcernsTypeController> logger,
		IIndexConcernsTypes indexConcernsTypes) : ControllerBase
	{
		[HttpGet]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ConcernsTypeResponse>>> Index(CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to get Concerns Types");
			var types = indexConcernsTypes.Execute();

			logger.LogInformation($"Returning Concerns types");
			var pagingResponse = PagingResponseFactory.Create(1, 50, types.Count, Request);
			var response = new ApiResponseV2<ConcernsTypeResponse>(types, pagingResponse);
			return Ok(response);
		}
	}
}