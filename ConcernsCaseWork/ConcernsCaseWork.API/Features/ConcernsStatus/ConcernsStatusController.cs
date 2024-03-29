using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Features.Paging;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Features.ConcernsStatus
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-statuses")]
	public class ConcernsStatusController : ControllerBase
	{
		private readonly ILogger<ConcernsStatusController> _logger;
		private readonly IIndexConcernsStatuses _indexConcernsStatuses;

		public ConcernsStatusController(
			ILogger<ConcernsStatusController> logger,
			IIndexConcernsStatuses indexConcernsStatuses)
		{
			_logger = logger;
			_indexConcernsStatuses = indexConcernsStatuses;
		}

		[HttpGet]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ConcernsStatusResponse>>> Index(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Attempting to get Concerns Statuses");
			var statuses = _indexConcernsStatuses.Execute();

			_logger.LogInformation($"Returning Concerns statuses");
			var pagingResponse = PagingResponseFactory.Create(1, 50, statuses.Count, Request);
			var response = new ApiResponseV2<ConcernsStatusResponse>(statuses, pagingResponse);
			return Ok(response);
		}
	}
}