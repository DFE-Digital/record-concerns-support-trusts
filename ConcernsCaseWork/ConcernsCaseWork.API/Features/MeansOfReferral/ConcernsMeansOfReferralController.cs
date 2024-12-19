using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Features.Paging;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.API.Features.MeansOfReferral
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/concerns-meansofreferral")]
	public class ConcernsMeansOfReferralController(
		ILogger<ConcernsMeansOfReferralController> logger,
		IIndexConcernsMeansOfReferrals indexConcernsMeansOfReferrals) : ControllerBase
	{
		[HttpGet]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiResponseV2<ConcernsMeansOfReferralResponse>>> Index(CancellationToken cancellationToken = default)
		{
			logger.LogInformation("Attempting to get Concerns Means of Referrals");
			var meansOfReferrals = indexConcernsMeansOfReferrals.Execute();

			logger.LogInformation("Returning Concerns Means of Referrals");
			var pagingResponse = PagingResponseFactory.Create(1, 50, meansOfReferrals.Count, Request);
			var response = new ApiResponseV2<ConcernsMeansOfReferralResponse>(meansOfReferrals, pagingResponse);
			return Ok(response);
		}
	}
}