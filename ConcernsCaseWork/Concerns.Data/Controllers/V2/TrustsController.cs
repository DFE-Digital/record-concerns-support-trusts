using Concerns.Data.ResponseModels;
using Concerns.Data.UseCases;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Concerns.Data.Controllers.V2
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/")]
    public class TrustsController : ControllerBase
    {
        private readonly IGetTrustByUkprn _getTrustByUkPrn;
        private readonly ISearchTrusts _searchTrusts;
        private readonly ILogger<TrustsController> _logger;

        public TrustsController(IGetTrustByUkprn getTrustByUkPrn, ISearchTrusts searchTrusts, ILogger<TrustsController> logger)
        {
            _getTrustByUkPrn = getTrustByUkPrn;
            _searchTrusts = searchTrusts;
            _logger = logger;
        }
        
        [HttpGet("trusts")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiResponseV2<TrustSummaryResponse>> SearchTrusts(string groupName, string ukPrn, string companiesHouseNumber, int page = 1, int count = 50)
        {
            _logger.LogInformation(
                "Searching for trusts by groupName \"{name}\", UKPRN \"{prn}\", companiesHouseNumber \"{number}\", page {page}, count {count}",
                groupName, ukPrn, companiesHouseNumber, page, count);

            var trusts = _searchTrusts
                .Execute(page, count, groupName, ukPrn, companiesHouseNumber)
                .ToList();
            
            _logger.LogInformation(
                "Found {count} trusts for groupName \"{name}\", UKPRN \"{prn}\", companiesHouseNumber \"{number}\", page {page}, count {count}",
                trusts.Count, groupName, ukPrn, companiesHouseNumber, page, count);
            
            _logger.LogDebug(JsonSerializer.Serialize(trusts));
            
            var pagingResponse = PagingResponseFactory.Create(page, count, trusts.Count, Request);
            var response = new ApiResponseV2<TrustSummaryResponse>(trusts, pagingResponse);
            return new OkObjectResult(response);
        }
        
        [HttpGet]
        [Route("trust/{ukprn}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<TrustResponse>> GetTrustByUkPrn(string ukPrn)
        {
            _logger.LogInformation("Attempting to get trust by UKPRN {prn}", ukPrn);
            var trust = _getTrustByUkPrn.Execute(ukPrn);

            if (trust == null)
            {
                _logger.LogInformation("No trust found for UKPRN {prn}", ukPrn);
                return new NotFoundResult();
            }

            _logger.LogInformation("Returning trust found by UKPRN {prn}", ukPrn);
            _logger.LogDebug(JsonSerializer.Serialize(trust));

            var response = new ApiSingleResponseV2<TrustResponse>(trust);
            return new OkObjectResult(response);
        }
    }
}