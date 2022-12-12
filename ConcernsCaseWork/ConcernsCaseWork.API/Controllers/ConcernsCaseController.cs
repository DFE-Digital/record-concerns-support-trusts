using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-cases")]
    public class ConcernsCaseController: ControllerBase
    {
        private readonly ILogger<ConcernsCaseController> _logger;
        private readonly ICreateConcernsCase _createConcernsCase;
        private readonly IGetConcernsCaseByUrn _getConcernsCaseByUrn;
        private readonly IGetConcernsCaseByTrustUkprn _getConcernsCaseByTrustUkprn;
        private readonly IUpdateConcernsCase _updateConcernsCase;
        private readonly IGetConcernsCasesByOwnerId _getConcernsCasesByOwnerId;
        private readonly IGetActiveConcernsCaseSummariesByOwner _getActiveConcernsCaseSummariesByOwner;
        private readonly IGetClosedConcernsCaseSummariesByOwner _getClosedConcernsCaseSummariesByOwner;
        private readonly IGetActiveConcernsCaseSummariesByTrust _getActiveConcernsCaseSummariesByTrust;
        private readonly IGetClosedConcernsCaseSummariesByTrust _getClosedConcernsCaseSummariesByTrust;

        public ConcernsCaseController(
            ILogger<ConcernsCaseController> logger,
            ICreateConcernsCase createConcernsCase,
            IGetConcernsCaseByUrn getConcernsCaseByUrn,
            IGetConcernsCaseByTrustUkprn getConcernsCaseByTrustUkprn,
            IUpdateConcernsCase updateConcernsCase,
            IGetConcernsCasesByOwnerId getConcernsCasesByOwnerId, 
            IGetActiveConcernsCaseSummariesByOwner getActiveConcernsCaseSummaries, 
            IGetClosedConcernsCaseSummariesByOwner getClosedConcernsCaseSummaries, 
            IGetActiveConcernsCaseSummariesByTrust getActiveConcernsCaseSummariesByTrust, 
            IGetClosedConcernsCaseSummariesByTrust getClosedConcernsCaseSummariesByTrust)
        {
            _logger = logger;
            _createConcernsCase = createConcernsCase;
            _getConcernsCaseByUrn = getConcernsCaseByUrn;
            _getConcernsCaseByTrustUkprn = getConcernsCaseByTrustUkprn;
            _updateConcernsCase = updateConcernsCase;
            _getConcernsCasesByOwnerId = getConcernsCasesByOwnerId;
            _getActiveConcernsCaseSummariesByOwner = getActiveConcernsCaseSummaries;
            _getClosedConcernsCaseSummariesByOwner = getClosedConcernsCaseSummaries;
            _getActiveConcernsCaseSummariesByTrust = getActiveConcernsCaseSummariesByTrust;
            _getClosedConcernsCaseSummariesByTrust = getClosedConcernsCaseSummariesByTrust;
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>> Create(ConcernCaseRequest request)
        {
            var validator = new ConcernsCaseRequestValidator();
            if (validator.Validate(request).IsValid)
            {
                var createdConcernsCase = _createConcernsCase.Execute(request);
                var response = new ApiSingleResponseV2<ConcernsCaseResponse>(createdConcernsCase);

                return new ObjectResult(response) {StatusCode = StatusCodes.Status201Created};
            }
            _logger.LogInformation($"Failed to create Concerns Case due to bad request");
            return BadRequest();
        }

        [HttpGet]
        [Route("urn/{urn}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>> GetByUrn(int urn)
        {
            _logger.LogInformation($"Attempting to get Concerns Case by Urn {urn}");
            var concernsCase = _getConcernsCaseByUrn.Execute(urn);

            if (concernsCase == null)
            {
                _logger.LogInformation($"No Concerns case found for URN {urn}");
                return NotFound();
            }

            _logger.LogInformation($"Returning Concerns case with Urn {urn}");
            _logger.LogDebug(JsonSerializer.Serialize(concernsCase));
            var response = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCase);

            return Ok(response);
        }

        [HttpGet]
        [Route("ukprn/{trustUkprn}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiResponseV2<ConcernsCaseResponse>> GetByTrustUkprn(string trustUkprn, int page = 1, int count = 50)
        {
            _logger.LogInformation($"Attempting to get Concerns Cases by Trust Ukprn {trustUkprn}, page {page}, count {count}");
            var concernsCases = _getConcernsCaseByTrustUkprn.Execute(trustUkprn, page, count);

            _logger.LogInformation($"Returning Concerns cases with Trust Ukprn {trustUkprn}, page {page}, count {count}");
            _logger.LogDebug(JsonSerializer.Serialize(concernsCases));
            var pagingResponse = PagingResponseFactory.Create(page, count, concernsCases.Count, Request);
            var response = new ApiResponseV2<ConcernsCaseResponse>(concernsCases, pagingResponse);

            return Ok(response);
        }

        [HttpPatch("{urn}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>> Update(int urn, ConcernCaseRequest request)
        {
            _logger.LogInformation($"Attempting to update Concerns Case {urn}");
            var validator = new ConcernsCaseRequestValidator();
            if (validator.Validate(request).IsValid)
            {
                var updatedAcademyConcernsCase = _updateConcernsCase.Execute(urn, request);
                if (updatedAcademyConcernsCase == null)
                {
                    _logger.LogInformation(
                        $"Updating Concerns Case failed: No Concerns Case matching Urn {urn} was found");
                    return NotFound();
                }

                _logger.LogInformation($"Successfully Updated Concerns Case {urn}");
                _logger.LogDebug(JsonSerializer.Serialize(updatedAcademyConcernsCase));

                var response = new ApiSingleResponseV2<ConcernsCaseResponse>(updatedAcademyConcernsCase);
                return Ok(response);
            }
            _logger.LogInformation($"Failed to update Concerns Case due to bad request");
            return BadRequest();
        }

        [HttpGet]
        [Route("owner/{ownerId}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiResponseV2<ConcernsCaseResponse>> GetByOwnerId(string ownerId, int? status = null, int page = 1, int count = 50)
        {
            _logger.LogInformation($"Attempting to get Concerns Cases by Owner Id {ownerId}, page {page}, count {count}");
            var concernsCases = _getConcernsCasesByOwnerId.Execute(ownerId, status, page, count);

            _logger.LogInformation($"Returning Concerns cases with Owner Id {ownerId}, page {page}, count {count}");
            _logger.LogDebug(JsonSerializer.Serialize(concernsCases));
            var pagingResponse = PagingResponseFactory.Create(page, count, concernsCases.Count, Request);
            var response = new ApiResponseV2<ConcernsCaseResponse>(concernsCases, pagingResponse);

            return Ok(response);
        }

        [HttpGet]
        [Route("summary/{ownerId}/active")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ActiveCaseSummaryResponse>>> GetActiveSummariesByOwnerId(string ownerId)
        {
	        _logger.LogInformation($"Attempting to get active Concerns Case summaries by Owner Id {ownerId}");
	        var caseSummaries = await _getActiveConcernsCaseSummariesByOwner.Execute(ownerId);

	        _logger.LogInformation($"Returning active Concerns cases with Owner Id {ownerId}");
	        var response = new ApiResponseV2<ActiveCaseSummaryResponse>(caseSummaries, null);
            
	        return Ok(response);
        }
        
        [HttpGet]
        [Route("summary/{ownerId}/closed")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ClosedCaseSummaryResponse>>> GetClosedSummariesByOwnerId(string ownerId)
        {
	        _logger.LogInformation($"Attempting to get closed Concerns Case summaries by Owner Id {ownerId}");
	        var caseSummaries = await _getClosedConcernsCaseSummariesByOwner.Execute(ownerId);

	        _logger.LogInformation($"Returning closed Concerns cases with Owner Id {ownerId}");
	        var response = new ApiResponseV2<ClosedCaseSummaryResponse>(caseSummaries, null);
            
	        return Ok(response);
        }
        
        [HttpGet]
        [Route("summary/bytrust/{trustukprn}/active")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ActiveCaseSummaryResponse>>> GetActiveSummariesByTrust(string trustukprn)
        {
	        _logger.LogInformation($"Attempting to get active Concerns Case summaries by Trust {trustukprn}");
	        var caseSummaries = await _getActiveConcernsCaseSummariesByTrust.Execute(trustukprn);

	        _logger.LogInformation($"Returning Concerns active cases with Trust {trustukprn}");
	        var response = new ApiResponseV2<ActiveCaseSummaryResponse>(caseSummaries, null);
            
	        return Ok(response);
        }
        
        [HttpGet]
        [Route("summary/bytrust/{trustukprn}/closed")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ClosedCaseSummaryResponse>>> GetClosedSummariesByTrust(string trustukprn)
        {
	        _logger.LogInformation($"Attempting to get closed Concerns Case summaries by Trust {trustukprn}");
	        var caseSummaries = await _getClosedConcernsCaseSummariesByTrust.Execute(trustukprn);

	        _logger.LogInformation($"Returning closed Concerns cases with Trust {trustukprn}");
	        var response = new ApiResponseV2<ClosedCaseSummaryResponse>(caseSummaries, null);
            
	        return Ok(response);
        }
    }
}