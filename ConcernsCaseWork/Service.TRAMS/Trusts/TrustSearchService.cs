using Microsoft.Extensions.Logging;
using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Trusts
{
	public sealed class TrustSearchService : ITrustSearchService
	{
		private readonly ITrustService _trustService;
		private readonly ILogger<TrustSearchService> _logger;

		private const int HardLimitTrustsByPagination = 10;

		public TrustSearchService(ITrustService trustService, ILogger<TrustSearchService> logger)
		{
			_trustService = trustService;
			_logger = logger;
		}
		
		public async Task<IList<TrustDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch)
		{
			_logger.LogInformation("TrustSearchService::GetTrustsBySearchCriteria execution");
			
			var trustList = new List<TrustDto>();
			IList<TrustDto> trusts;
			var nrRequests = 0;
			
			do
			{
				trusts = await _trustService.GetTrustsByPagination(trustSearch);
				if (!trusts.Any()) continue;
				
				trustList.AddRange(trusts);
				trustSearch.PageIncrement();
				
				// Safe guard in case we have more than 10 pages.
				// We don't have a scenario at the moment, but feels like we need a limit.
				if ((nrRequests = Interlocked.Increment(ref nrRequests)) > HardLimitTrustsByPagination)
				{
					break;
				}

			} while (trusts.Any());
			
			return trustList;
		}
	}
}