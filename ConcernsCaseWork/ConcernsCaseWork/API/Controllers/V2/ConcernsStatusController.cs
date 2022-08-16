using Concerns.Data.ResponseModels;
using Concerns.Data.UseCases;
using Microsoft.Extensions.Logging;

namespace Concerns.Data.Controllers.V2
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-statuses")]
    public class ConcernsStatusController: ControllerBase
    {
        private ILogger<ConcernsStatusController> _logger;
        private IIndexConcernsStatuses _indexConcernsStatuses;

        public ConcernsStatusController(
            ILogger<ConcernsStatusController> logger, 
            IIndexConcernsStatuses indexConcernsStatuses)
        {
            _logger = logger;
            _indexConcernsStatuses = indexConcernsStatuses;
        }


        [HttpGet]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiResponseV2<ConcernsStatusResponse>> Index()
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