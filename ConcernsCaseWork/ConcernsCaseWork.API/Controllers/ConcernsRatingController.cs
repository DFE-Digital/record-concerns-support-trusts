using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-ratings")]
    public class ConcernsRatingController: ControllerBase
    {
        private readonly ILogger<ConcernsRatingController> _logger;
        private readonly IIndexConcernsRatings _indexConcernsRatings;

        public ConcernsRatingController(
            ILogger<ConcernsRatingController> logger,
            IIndexConcernsRatings indexConcernsRatings)
        {
            _logger = logger;
            _indexConcernsRatings = indexConcernsRatings;
        }
        
        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ConcernsRatingResponse>>> Index(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Attempting to get Concerns Ratings");
            var ratings = _indexConcernsRatings.Execute();
            
            _logger.LogInformation($"Returning Concerns ratings");
            var pagingResponse = PagingResponseFactory.Create(1, 50, ratings.Count, Request);
            var response = new ApiResponseV2<ConcernsRatingResponse>(ratings, pagingResponse);
            return Ok(response);
        }
    }
}