using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-meansofreferral")]
    public class ConcernsMeansOfReferralController: ControllerBase
    {
        private readonly ILogger<ConcernsMeansOfReferralController> _logger;
        private readonly IIndexConcernsMeansOfReferrals _indexConcernsMeansOfReferrals;

        public ConcernsMeansOfReferralController(
            ILogger<ConcernsMeansOfReferralController> logger, 
            IIndexConcernsMeansOfReferrals indexConcernsMeansOfReferrals)
        {
            _logger = logger;
            _indexConcernsMeansOfReferrals = indexConcernsMeansOfReferrals;
        }
        
        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ConcernsMeansOfReferralResponse>>> Index(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Attempting to get Concerns Means of Referrals");
            var meansOfReferrals = _indexConcernsMeansOfReferrals.Execute();
            
            _logger.LogInformation("Returning Concerns Means of Referrals");
            var pagingResponse = PagingResponseFactory.Create(1, 50, meansOfReferrals.Count, Request);
            var response = new ApiResponseV2<ConcernsMeansOfReferralResponse>(meansOfReferrals, pagingResponse);
            return Ok(response);
        }
    }
}