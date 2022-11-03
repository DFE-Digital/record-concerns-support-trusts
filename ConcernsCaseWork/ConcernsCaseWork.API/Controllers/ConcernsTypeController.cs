using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-types")]
    public class ConcernsTypeController: ControllerBase
    {
        
        private ILogger<ConcernsTypeController> _logger;
        private IIndexConcernsTypes _indexConcernsTypes;

        public ConcernsTypeController(
            ILogger<ConcernsTypeController> logger, 
            IIndexConcernsTypes indexConcernsTypes)
        {
            _logger = logger;
            _indexConcernsTypes = indexConcernsTypes;
        }
        
        [HttpGet]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiResponseV2<ConcernsTypeResponse>> Index()
        {
            _logger.LogInformation($"Attempting to get Concerns Types");
            var types = _indexConcernsTypes.Execute();
            
            _logger.LogInformation($"Returning Concerns types");
            var pagingResponse = PagingResponseFactory.Create(1, 50, types.Count, Request);
            var response = new ApiResponseV2<ConcernsTypeResponse>(types, pagingResponse);
            return Ok(response);
        }
    }
}