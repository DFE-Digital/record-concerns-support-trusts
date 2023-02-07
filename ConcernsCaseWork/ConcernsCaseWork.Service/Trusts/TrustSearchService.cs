using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ConcernsCaseWork.Service.Trusts
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

		public async Task<IList<TrustSearchDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch)
		{
			_logger.LogInformation("TrustSearchService::GetTrustsBySearchCriteria execution");

			var stopwatch = Stopwatch.StartNew();
			var trustList = new List<TrustSearchDto>();
			var nrRequests = 0;

			try
			{
				ApiListWrapper<TrustSearchDto> apiListWrapperTrusts;

				do
				{
					var stopwatch2 = Stopwatch.StartNew();
					apiListWrapperTrusts = await _trustService.GetTrustsByPagination(trustSearch);
					stopwatch2.Stop();
					Debug.WriteLine($"TrustSearchService::GetTrustsBySearchCriteria individual api execution time {stopwatch2.ElapsedMilliseconds} ms.");


					// The following condition will break the loop.
					if (apiListWrapperTrusts?.Data is null || !apiListWrapperTrusts.Data.Any())
					{
						continue;
					}

					var stopwatch3 = Stopwatch.StartNew();
					trustList.AddRange(apiListWrapperTrusts.Data);
					trustSearch.PageIncrement();

					// Safe guard in case we have more than 10 pages.
					// We don't have a scenario at the moment, but feels like we need a limit.
					if ((nrRequests = Interlocked.Increment(ref nrRequests)) > _hardLimitTrustsByPagination)
					{
						break;
					}
					Debug.WriteLine($"TrustSearchService::GetTrustsBySearchCriteria individual interlock page increment execution time {stopwatch3.ElapsedMilliseconds} ms.");
				} while (apiListWrapperTrusts?.Data != null && apiListWrapperTrusts.Data.Any() && apiListWrapperTrusts.Paging?.NextPageUrl != null);


			}
			finally
			{
				stopwatch.Stop();
				Debug.WriteLine($"TrustSearchService::GetTrustsBySearchCriteria execution time {stopwatch.ElapsedMilliseconds} ms. Number of requests: {nrRequests}");
				_logger.LogInformation("TrustSearchService::GetTrustsBySearchCriteria execution time {ElapsedMilliseconds} ms. Number of requests: {nrRequests}", stopwatch.ElapsedMilliseconds);
			}

			return trustList;
		}
	}
}