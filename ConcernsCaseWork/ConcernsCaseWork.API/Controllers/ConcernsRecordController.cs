using Azure.Core;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-records")]
    
    public class ConcernsRecordController : ControllerBase
    {
        private readonly ILogger<ConcernsRecordController> _logger;
        private readonly ICreateConcernsRecord _createConcernsRecord;
        private readonly IUpdateConcernsRecord _updateConcernsRecord;
		private readonly IGetConcernsRecord _getConcernsRecord;
		private readonly IDeleteConcernsRecord _deleteConcernsRecord;
        private readonly IGetConcernsRecordsByCaseUrn _getConcernsRecordsByCaseUrn;

        public ConcernsRecordController(
            ILogger<ConcernsRecordController> logger, 
            ICreateConcernsRecord createConcernsRecord,
            IUpdateConcernsRecord updateConcernsRecord,
			IDeleteConcernsRecord deleteConcernsRecord,
            IGetConcernsRecordsByCaseUrn getConcernsRecordsByCaseUrn,
			IGetConcernsRecord getConcernsRecord)
        {
            _logger = logger;
            _createConcernsRecord = createConcernsRecord;
            _updateConcernsRecord = updateConcernsRecord;
			_deleteConcernsRecord = deleteConcernsRecord;
			_getConcernsRecordsByCaseUrn = getConcernsRecordsByCaseUrn;
			_getConcernsRecord = getConcernsRecord;
		}
        
        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<ConcernsRecordResponse>>> Create(ConcernsRecordRequest request, CancellationToken cancellationToken = default)
        {
            var createdConcernsRecord = _createConcernsRecord.Execute(request);
            var response = new ApiSingleResponseV2<ConcernsRecordResponse>(createdConcernsRecord);
            
            return new ObjectResult(response) {StatusCode = StatusCodes.Status201Created};
        }
        
        [HttpPatch("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<ConcernsRecordResponse>>> Update(int id, ConcernsRecordRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Attempting to update Concerns Record {id}");
            var updatedAcademyConcernsRecord = _updateConcernsRecord.Execute(id, request);
            if (updatedAcademyConcernsRecord == null)
            {
                _logger.LogInformation($"Updating Concerns Record failed: No Concerns Record matching Id {id} was found");
                return NotFound();
            }

            _logger.LogInformation($"Successfully Updated Concerns Record {id}");
            _logger.LogDebug(JsonSerializer.Serialize(updatedAcademyConcernsRecord));
			
            var response = new ApiSingleResponseV2<ConcernsRecordResponse>(updatedAcademyConcernsRecord);
            return Ok(response);
        }
        
        [HttpGet("case/urn/{urn}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ConcernsRecordResponse>>> GetByCaseUrn(int urn, int page = 1, int count = 50, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Attempting to get Concerns Case {urn}");
            var records = _getConcernsRecordsByCaseUrn.Execute(urn);

            _logger.LogInformation($"Returning Records for Case urn: {urn}");
            var pagingResponse = PagingResponseFactory.Create(page, count, records.Count, Request);
            var response = new ApiResponseV2<ConcernsRecordResponse>(records, pagingResponse);
            return Ok(response);
        }


		[HttpDelete("{id}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsRecordResponse>>> Delete(int id, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Attempting to delete Concerns Record {id}");

			var concernsRecord = _getConcernsRecord.Execute(id);
			if (concernsRecord == null)
			{
				_logger.LogInformation($"Deleting Concerns Record failed: No Concerns Record matching Id {id} was found");
				return NotFound();
			}

			_deleteConcernsRecord.Execute(id);

			_logger.LogInformation($"Successfully deleted Concerns Record {id}");

			return NoContent();
		}
	}
}