using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/concerns-records")]
    
    public class ConcernsRecordController : ControllerBase
    {
        private readonly ILogger<ConcernsRecordController> _logger;
        private readonly ICreateConcernsRecord _createConcernsRecord;
        private readonly IUpdateConcernsRecord _updateConcernsRecord;
        private readonly IGetConcernsRecordsByCaseUrn _getConcernsRecordsByCaseUrn;

        public ConcernsRecordController(
            ILogger<ConcernsRecordController> logger, 
            ICreateConcernsRecord createConcernsRecord,
            IUpdateConcernsRecord updateConcernsRecord,
            IGetConcernsRecordsByCaseUrn getConcernsRecordsByCaseUrn)
        {
            _logger = logger;
            _createConcernsRecord = createConcernsRecord;
            _updateConcernsRecord = updateConcernsRecord;
            _getConcernsRecordsByCaseUrn = getConcernsRecordsByCaseUrn;
        }
        
        [HttpPost]
        public ActionResult<ApiSingleResponseV2<ConcernsRecordResponse>> Create(ConcernsRecordRequest request)
        {
            var createdConcernsRecord = _createConcernsRecord.Execute(request);
            var response = new ApiSingleResponseV2<ConcernsRecordResponse>(createdConcernsRecord);
            
            return new ObjectResult(response) {StatusCode = StatusCodes.Status201Created};
        }
        
        [HttpPatch("{urn}")]
        public ActionResult<ApiSingleResponseV2<ConcernsRecordResponse>> Update(int urn, ConcernsRecordRequest request)
        {
            _logger.LogInformation($"Attempting to update Concerns Record {urn}");
            var updatedAcademyConcernsRecord = _updateConcernsRecord.Execute(urn, request);
            if (updatedAcademyConcernsRecord == null)
            {
                _logger.LogInformation($"Updating Concerns Record failed: No Concerns Record matching Urn {urn} was found");
                return NotFound();
            }

            _logger.LogInformation($"Successfully Updated Concerns Record {urn}");
            _logger.LogDebug(JsonSerializer.Serialize(updatedAcademyConcernsRecord));
			
            var response = new ApiSingleResponseV2<ConcernsRecordResponse>(updatedAcademyConcernsRecord);
            return Ok(response);
        }
        
        [HttpGet("case/urn/{urn}")]
        public ActionResult<ApiResponseV2<ConcernsRecordResponse>> GetByCaseUrn(int urn, int page = 1, int count = 50)
        {
            _logger.LogInformation($"Attempting to get Concerns Case {urn}");
            var records = _getConcernsRecordsByCaseUrn.Execute(urn);

            _logger.LogInformation($"Returning Records for Case urn: {urn}");
            var pagingResponse = PagingResponseFactory.Create(page, count, records.Count, Request);
            var response = new ApiResponseV2<ConcernsRecordResponse>(records, pagingResponse);
            return Ok(response);
        }
    }
}