using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.TRAMS.Configuration;
using Service.TRAMS.RecordWhistleblower;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordWhistleblower
{
	public sealed class TrustSearchService : ITrustSearchService
	{
		private readonly ITrustService _trustService;
		private readonly ILogger<TrustSearchService> _logger;
		private readonly int _hardLimitTrustsByPagination;

		public TrustSearchService(ITrustService trustService, IOptions<TrustSearchOptions> options, ILogger<TrustSearchService> logger)
		{
			_trustService = trustService;
			_logger = logger;
			_hardLimitTrustsByPagination = options.Value.TrustsLimitByPage;
		}
		
		public async Task<IList<TrustSummaryDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch)
		{
			_logger.LogInformation("TrustSearchService::GetTrustsBySearchCriteria execution");
			
			var trustList = new List<TrustSummaryDto>();
			IList<TrustSummaryDto> trusts;
			var nrRequests = 0;
			
			do
			{
				trusts = await _trustService.GetTrustsByPagination(trustSearch);
				if (!trusts.Any()) continue;
				
				trustList.AddRange(trusts);
				trustSearch.PageIncrement();
				
				// Safe guard in case we have more than 10 pages.
				// We don't have a scenario at the moment, but feels like we need a limit.
				if ((nrRequests = Interlocked.Increment(ref nrRequests)) > _hardLimitTrustsByPagination)
				{
					break;
				}

			} while (trusts.Any());
			
			return trustList;
		}
	}
}