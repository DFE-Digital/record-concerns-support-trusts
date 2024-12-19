using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.Features.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Features.ConcernsStatus
{
	[ApiVersion("2.0")]
	[ApiController]
	[Authorize(Policy = Policy.Default)]
	[Route("v{version:apiVersion}/concerns-statuses")]
	public class ConcernsStatusController(
		ILogger<ConcernsStatusController> logger,
		IIndexConcernsStatuses indexConcernsStatuses) : ControllerBase
	{
		[HttpGet]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiResponseV2<ConcernsStatusResponse>> Index(CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to get Concerns Statuses");
			var statuses = indexConcernsStatuses.Execute();

			logger.LogInformation($"Returning Concerns statuses");
			var pagingResponse = PagingResponseFactory.Create(1, 50, statuses.Count, Request);
			var response = new ApiResponseV2<ConcernsStatusResponse>(statuses, pagingResponse);
			return Ok(response);
		}
	}
}