using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Features.Paging;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Features.ConcernsRating
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-ratings")]
	public class ConcernsRatingController(
		ILogger<ConcernsRatingController> logger,
		IIndexConcernsRatings indexConcernsRatings) : ControllerBase
	{
		[HttpGet]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ConcernsRatingResponse>>> Index(CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to get Concerns Ratings");
			var ratings = indexConcernsRatings.Execute();

			logger.LogInformation($"Returning Concerns ratings");
			var pagingResponse = PagingResponseFactory.Create(1, 50, ratings.Count, Request);
			var response = new ApiResponseV2<ConcernsRatingResponse>(ratings, pagingResponse);
			return Ok(response);
		}
	}
}