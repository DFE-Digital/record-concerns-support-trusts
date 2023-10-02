using ConcernsCaseWork.API.Features.Case;
using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.Validators;
using ConcernsCaseWork.Data.Gateways;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConcernsCaseWork.API.Features.Case
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-cases")]
	public class ConcernsCaseController : ControllerBase
	{
		private readonly ILogger<ConcernsCaseController> _logger;
		private readonly ICreateConcernsCase _createConcernsCase;
		private readonly IGetConcernsCaseByUrn _getConcernsCaseByUrn;
		private readonly IGetConcernsCaseByTrustUkprn _getConcernsCaseByTrustUkprn;
		private readonly IUpdateConcernsCase _updateConcernsCase;
		private readonly IDeleteConcernsCase _deleteConcernsCase;
		private readonly IGetConcernsCasesByOwnerId _getConcernsCasesByOwnerId;
		private readonly IGetActiveConcernsCaseSummariesForUsersTeam _getActiveConcernsCaseSummariesForUsersTeam;
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
			IDeleteConcernsCase deleteConcernsCase,
			IGetConcernsCasesByOwnerId getConcernsCasesByOwnerId,
			IGetClosedConcernsCaseSummariesByOwner getClosedConcernsCaseSummaries,
			IGetActiveConcernsCaseSummariesByTrust getActiveConcernsCaseSummariesByTrust,
			IGetClosedConcernsCaseSummariesByTrust getClosedConcernsCaseSummariesByTrust,
			IGetActiveConcernsCaseSummariesForUsersTeam getActiveConcernsCaseSummariesForUsersTeam,
			IGetActiveConcernsCaseSummariesByOwner getActiveConcernsCaseSummariesByOwner)
		{
			_logger = logger;
			_createConcernsCase = createConcernsCase;
			_getConcernsCaseByUrn = getConcernsCaseByUrn;
			_getConcernsCaseByTrustUkprn = getConcernsCaseByTrustUkprn;
			_updateConcernsCase = updateConcernsCase;
			_deleteConcernsCase = deleteConcernsCase;
			_getConcernsCasesByOwnerId = getConcernsCasesByOwnerId;
			_getClosedConcernsCaseSummariesByOwner = getClosedConcernsCaseSummaries;
			_getActiveConcernsCaseSummariesByTrust = getActiveConcernsCaseSummariesByTrust;
			_getClosedConcernsCaseSummariesByTrust = getClosedConcernsCaseSummariesByTrust;
			_getActiveConcernsCaseSummariesForUsersTeam = getActiveConcernsCaseSummariesForUsersTeam;
			_getActiveConcernsCaseSummariesByOwner = getActiveConcernsCaseSummariesByOwner;
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>>> Create(ConcernCaseRequest request, CancellationToken cancellationToken = default)
		{
			var validator = new ConcernsCaseRequestValidator();

			var validationResult = validator.Validate(request);

			if(!validationResult.IsValid)
			{
				_logger.LogInformation($"Failed to create Concerns Case due to bad request");
				return new BadRequestObjectResult(validationResult.Errors);
			}

			var createdConcernsCase = _createConcernsCase.Execute(request);
			var response = new ApiSingleResponseV2<ConcernsCaseResponse>(createdConcernsCase);
			return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
		}

		[HttpGet]
		[Route("urn/{urn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>>> GetByUrn(int urn, CancellationToken cancellationToken = default)
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
		public async Task<ActionResult<ApiResponseV2<ConcernsCaseResponse>>> GetByTrustUkprn(string trustUkprn, int page = 1, int count = 50, CancellationToken cancellationToken = default)
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
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>>> Update(int urn, ConcernCaseRequest request, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Attempting to update Concerns Case {urn}");
			var validator = new ConcernsCaseRequestValidator();

			var validationResult = validator.Validate(request);

			if (!validationResult.IsValid)
			{
				_logger.LogInformation($"Failed to update Concerns Case due to bad request");
				return new BadRequestObjectResult(validationResult.Errors);
			}

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

		[HttpGet]
		[Route("owner/{ownerId}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ConcernsCaseResponse>>> GetByOwnerId(string ownerId, int? status = null, int page = 1, int count = 50, CancellationToken cancellationToken = default)
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
		public async Task<ActionResult<ApiResponseV2<ActiveCaseSummaryResponse>>> GetActiveSummariesForUser(
			string ownerId,
			int? page = null,
			int? count = null,
			CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Attempting to get active Concerns Case summaries for User Id {ownerId} page {page} count {count}");

			var parameters = new GetCaseSummariesByOwnerParameters()
			{
				Owner = ownerId,
				Page = page,
				Count = count
			};

			(IList<ActiveCaseSummaryResponse> caseSummaries, int recordCount) = await _getActiveConcernsCaseSummariesByOwner.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			_logger.LogInformation($"Returning active Concerns cases for User Id {ownerId} page {page} count {count}", ownerId);
			var response = new ApiResponseV2<ActiveCaseSummaryResponse>(caseSummaries, pagingResponse);
            
	        return Ok(response);
        }

		[HttpGet]
		[Route("summary/{userId}/active/team")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ActiveCaseSummaryResponse>>> GetActiveSummariesForUsersTeam(string userId,
			int? page = null,
			int? count = null,
			CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("Attempting to get active Concerns Case summaries for User Id {UserId}", userId);

			var parameters = new GetCaseSummariesForUsersTeamParameters()
			{
				UserID = userId,
				Page = page,
				Count = count
			};

			(IList<ActiveCaseSummaryResponse> caseSummaries, int recordCount) = await _getActiveConcernsCaseSummariesForUsersTeam.Execute(parameters, cancellationToken);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			_logger.LogInformation("Returning active Concerns cases for User Id {UserId}", userId);
			var response = new ApiResponseV2<ActiveCaseSummaryResponse>(caseSummaries, pagingResponse);

			return Ok(response);
		}

		[HttpGet]
        [Route("summary/{ownerId}/closed")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ClosedCaseSummaryResponse>>> GetClosedSummariesByOwnerId(
			string ownerId,
			int? page = null,
			int? count = null,
			CancellationToken cancellationToken = default)
        {
	        _logger.LogInformation($"Attempting to get closed Concerns Case summaries by Owner Id {ownerId} page {page} count {count}");

			var parameters = new GetCaseSummariesByOwnerParameters()
			{
				Owner = ownerId,
				Page = page,
				Count = count
			};

			(IList<ClosedCaseSummaryResponse> caseSummaries, int recordCount) = await _getClosedConcernsCaseSummariesByOwner.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			_logger.LogInformation($"Returning closed Concerns cases with Owner Id {ownerId} page {page} count {count}");
	        var response = new ApiResponseV2<ClosedCaseSummaryResponse>(caseSummaries, pagingResponse);
            
	        return Ok(response);
        }
        
        [HttpGet]
        [Route("summary/bytrust/{trustukprn}/active")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiResponseV2<ActiveCaseSummaryResponse>>> GetActiveSummariesByTrust(
			string trustukprn,
			int? page = null,
			int? count = null,
			CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Attempting to get active Concerns Case summaries by Trust {trustukprn} page {page} count {count}");

			var parameters = new GetCaseSummariesByTrustParameters()
			{
				TrustUkPrn = trustukprn,
				Page = page,
				Count = count
			};

			(IList<ActiveCaseSummaryResponse> caseSummaries, int recordCount) = await _getActiveConcernsCaseSummariesByTrust.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			_logger.LogInformation($"Returning Concerns active cases with Trust {trustukprn} page {page} count {count}");
			var response = new ApiResponseV2<ActiveCaseSummaryResponse>(caseSummaries, pagingResponse);

			return Ok(response);
		}

		[HttpGet]
		[Route("summary/bytrust/{trustukprn}/closed")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ClosedCaseSummaryResponse>>> GetClosedSummariesByTrust(
			string trustukprn,
			int? page = null,
			int? count = null,
			CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Attempting to get closed Concerns Case summaries by Trust {trustukprn} page {page} count {count}");

			var parameters = new GetCaseSummariesByTrustParameters()
			{
				TrustUkPrn = trustukprn,
				Page = page,
				Count = count
			};

			(IList<ClosedCaseSummaryResponse> caseSummaries, int recordCount) = await _getClosedConcernsCaseSummariesByTrust.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			_logger.LogInformation($"Returning closed Concerns cases with Trust {trustukprn} page {page} count {count}");
			var response = new ApiResponseV2<ClosedCaseSummaryResponse>(caseSummaries, pagingResponse);

			return Ok(response);
		}


		[HttpDelete("{urn}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete(int urn, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Attempting to delete Concerns Case {urn}");


			_logger.LogInformation($"Attempting to get Concerns Case by Urn {urn}");
			var concernsCase = _getConcernsCaseByUrn.Execute(urn);

			if (concernsCase == null)
			{
				_logger.LogInformation($"Deleting Concerns Case failed: No Concerns Case matching Urn {urn} was found");
				return NotFound();
			}

			_logger.LogInformation($"Found Concerns case with Urn {urn}");

			var result = _deleteConcernsCase.Execute(urn);
			if (result.IsFailure)
			{
				_logger.LogInformation($"Failed to delete Concerns Case due to bad request");
				return BadRequest(result.Error.Message);
			}

			_logger.LogInformation($"Successfully deleted Concerns Case for URN {urn}");

			return NoContent();
		}


		private PagingResponse BuildPaginationResponse(int recordCount, int? page = null, int? count = null)
		{
			PagingResponse result = null;

			if (page.HasValue && count.HasValue)
				result = PagingResponseFactory.Create(page.Value, count.Value, recordCount, Request);

			return result;
		}
	}
}