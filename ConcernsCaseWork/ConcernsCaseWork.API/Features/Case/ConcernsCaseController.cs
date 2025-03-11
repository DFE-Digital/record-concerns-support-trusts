using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.Features.Paging;
using ConcernsCaseWork.Data.Gateways; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConcernsCaseWork.API.Features.Case
{
	[ApiVersion("2.0")]
	[ApiController]
	//[Authorize(Policy= Policy.Default)]
	[Route("v{version:apiVersion}/concerns-cases")]
	public class ConcernsCaseController(
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
		IGetActiveConcernsCaseSummariesByOwner getActiveConcernsCaseSummariesByOwner) : ControllerBase
	{
		[HttpPost]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>>> Create(ConcernCaseRequest request, CancellationToken cancellationToken = default)
		{
			var validator = new ConcernsCaseRequestValidator();

			var validationResult = validator.Validate(request);

			if(!validationResult.IsValid)
			{
				logger.LogInformation($"Failed to create Concerns Case due to bad request");
				return new BadRequestObjectResult(validationResult.Errors);
			}

			var createdConcernsCase = createConcernsCase.Execute(request);
			var response = new ApiSingleResponseV2<ConcernsCaseResponse>(createdConcernsCase);
			return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
		}

		[HttpGet]
		[Route("urn/{urn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>>> GetByUrn(int urn, CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to get Concerns Case by Urn {urn}");
			var concernsCase = getConcernsCaseByUrn.Execute(urn);

			if (concernsCase == null)
			{
				logger.LogInformation($"No Concerns case found for URN {urn}");
				return NotFound();
			}

			logger.LogInformation($"Returning Concerns case with Urn {urn}");
			logger.LogDebug(JsonSerializer.Serialize(concernsCase));
			var response = new ApiSingleResponseV2<ConcernsCaseResponse>(concernsCase);

			return Ok(response);
		}

		[HttpGet]
		[Route("ukprn/{trustUkprn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ConcernsCaseResponse>>> GetByTrustUkprn(string trustUkprn, int page = 1, int count = 50, CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to get Concerns Cases by Trust Ukprn {trustUkprn}, page {page}, count {count}");
			var concernsCases = getConcernsCaseByTrustUkprn.Execute(trustUkprn, page, count);

			logger.LogInformation($"Returning Concerns cases with Trust Ukprn {trustUkprn}, page {page}, count {count}");
			logger.LogDebug(JsonSerializer.Serialize(concernsCases));
			var pagingResponse = PagingResponseFactory.Create(page, count, concernsCases.Count, Request);
			var response = new ApiResponseV2<ConcernsCaseResponse>(concernsCases, pagingResponse);

			return Ok(response);
		}

		[HttpPatch("{urn}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseResponse>>> Update(int urn, ConcernCaseRequest request, CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to update Concerns Case {urn}");
			var validator = new ConcernsCaseRequestValidator();

			var validationResult = validator.Validate(request);

			if (!validationResult.IsValid)
			{
				logger.LogInformation($"Failed to update Concerns Case due to bad request");
				return new BadRequestObjectResult(validationResult.Errors);
			}

			var updatedAcademyConcernsCase = updateConcernsCase.Execute(urn, request);
			if (updatedAcademyConcernsCase == null)
			{
				logger.LogInformation(
					$"Updating Concerns Case failed: No Concerns Case matching Urn {urn} was found");
				return NotFound();
			}

			logger.LogInformation($"Successfully Updated Concerns Case {urn}");
			logger.LogDebug(JsonSerializer.Serialize(updatedAcademyConcernsCase));

			var response = new ApiSingleResponseV2<ConcernsCaseResponse>(updatedAcademyConcernsCase);
			return Ok(response);			
		}

		[HttpGet]
		[Route("owner/{ownerId}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ConcernsCaseResponse>>> GetByOwnerId(string ownerId, int? status = null, int page = 1, int count = 50, CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to get Concerns Cases by Owner Id {ownerId}, page {page}, count {count}");
			var concernsCases = getConcernsCasesByOwnerId.Execute(ownerId, status, page, count);

			logger.LogInformation($"Returning Concerns cases with Owner Id {ownerId}, page {page}, count {count}");
			logger.LogDebug(JsonSerializer.Serialize(concernsCases));
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
			logger.LogInformation($"Attempting to get active Concerns Case summaries for User Id {ownerId} page {page} count {count}");

			var parameters = new GetCaseSummariesByOwnerParameters()
			{
				Owner = ownerId,
				Page = page,
				Count = count
			};

			(IList<ActiveCaseSummaryResponse> caseSummaries, int recordCount) = await getActiveConcernsCaseSummariesByOwner.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			logger.LogInformation($"Returning active Concerns cases for User Id {ownerId} page {page} count {count}", ownerId);
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
			logger.LogInformation("Attempting to get active Concerns Case summaries for User Id {UserId}", userId);

			var parameters = new GetCaseSummariesForUsersTeamParameters()
			{
				UserID = userId,
				Page = page,
				Count = count
			};

			(IList<ActiveCaseSummaryResponse> caseSummaries, int recordCount) = await getActiveConcernsCaseSummariesForUsersTeam.Execute(parameters, cancellationToken);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			logger.LogInformation("Returning active Concerns cases for User Id {UserId}", userId);
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
	        logger.LogInformation($"Attempting to get closed Concerns Case summaries by Owner Id {ownerId} page {page} count {count}");

			var parameters = new GetCaseSummariesByOwnerParameters()
			{
				Owner = ownerId,
				Page = page,
				Count = count
			};

			(IList<ClosedCaseSummaryResponse> caseSummaries, int recordCount) = await getClosedConcernsCaseSummaries.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			logger.LogInformation($"Returning closed Concerns cases with Owner Id {ownerId} page {page} count {count}");
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
			logger.LogInformation($"Attempting to get active Concerns Case summaries by Trust {trustukprn} page {page} count {count}");

			var parameters = new GetCaseSummariesByTrustParameters()
			{
				TrustUkPrn = trustukprn,
				Page = page,
				Count = count
			};

			(IList<ActiveCaseSummaryResponse> caseSummaries, int recordCount) = await getActiveConcernsCaseSummariesByTrust.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			logger.LogInformation($"Returning Concerns active cases with Trust {trustukprn} page {page} count {count}");
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
			logger.LogInformation($"Attempting to get closed Concerns Case summaries by Trust {trustukprn} page {page} count {count}");

			var parameters = new GetCaseSummariesByTrustParameters()
			{
				TrustUkPrn = trustukprn,
				Page = page,
				Count = count
			};

			(IList<ClosedCaseSummaryResponse> caseSummaries, int recordCount) = await getClosedConcernsCaseSummariesByTrust.Execute(parameters);

			PagingResponse pagingResponse = BuildPaginationResponse(recordCount, page, count);

			logger.LogInformation($"Returning closed Concerns cases with Trust {trustukprn} page {page} count {count}");
			var response = new ApiResponseV2<ClosedCaseSummaryResponse>(caseSummaries, pagingResponse);

			return Ok(response);
		}

		[Authorize(Policy = Policy.CanDelete)]
		[HttpDelete("{urn}")]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> Delete(int urn, CancellationToken cancellationToken = default)
		{
			logger.LogInformation($"Attempting to delete Concerns Case {urn}");


			logger.LogInformation($"Attempting to get Concerns Case by Urn {urn}");
			var concernsCase = getConcernsCaseByUrn.Execute(urn);

			if (concernsCase == null)
			{
				logger.LogInformation($"Deleting Concerns Case failed: No Concerns Case matching Urn {urn} was found");
				return NotFound();
			}

			logger.LogInformation($"Found Concerns case with Urn {urn}");

			var result = deleteConcernsCase.Execute(urn);
			if (result.IsFailure)
			{
				logger.LogInformation($"Failed to delete Concerns Case due to bad request");
				return BadRequest(result.Error.Message);
			}

			logger.LogInformation($"Successfully deleted Concerns Case for URN {urn}");

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