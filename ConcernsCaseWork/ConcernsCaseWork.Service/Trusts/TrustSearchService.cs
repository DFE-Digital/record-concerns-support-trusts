using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustSearchService : ITrustSearchService
	{
		private readonly ITrustService _trustService;
		private ILogger<TrustSearchService> _logger;
		private TrustSearchOptions _options;

		public TrustSearchService(ITrustService trustService, IOptions<TrustSearchOptions> options, ILogger<TrustSearchService> logger)
		{
			_logger = Guard.Against.Null(logger);
			_options = Guard.Against.Null(options.Value);
			_trustService = Guard.Against.Null(trustService);
		}

		public async Task<TrustSearchResponseDto> GetTrustsBySearchCriteria(TrustSearch searchCriteria)
		{
			_logger.LogMethodEntered();

			Guard.Against.Null(searchCriteria);

			var matchingTrusts = new List<TrustSearchDto>();
			var numberOfRequests = 0;
			var numberOfMatches = 0;
			TrustSearchResponseDto pageOfResults;

			var stopwatch = Stopwatch.StartNew();

			do
			{
				numberOfRequests++;
				pageOfResults = await RequestPage(searchCriteria);

				if (numberOfRequests == 1)
				{
					numberOfMatches = pageOfResults.NumberOfMatches;
				}

				if (PageOfResultsHasData(pageOfResults))
				{
					matchingTrusts.AddRange(pageOfResults.Trusts);
					searchCriteria.PageIncrement();
				}
			} while (numberOfRequests < _options.TrustsLimitByPage && PageOfResultsHasData(pageOfResults));

			stopwatch.Stop();
			Debug.WriteLine($"{nameof(GetTrustsBySearchCriteria)} execution time {stopwatch.ElapsedMilliseconds} ms. Number of requests: {numberOfRequests}");
			_logger.LogInformation("TrustSearchService::GetTrustsBySearchCriteria execution time {ElapsedMilliseconds} ms. Number of requests: {nrRequests}", stopwatch.ElapsedMilliseconds);

			return new TrustSearchResponseDto { NumberOfMatches = numberOfMatches, Trusts = matchingTrusts };
		}

		private bool PageOfResultsHasData(TrustSearchResponseDto pageOfResults)
		{
			return pageOfResults?.Trusts != null && pageOfResults.Trusts.Count > 0;
		}

		private async Task<TrustSearchResponseDto> RequestPage(TrustSearch searchCriteria)
		{
			try
			{
				_logger.LogMethodEntered();

				var stopwatch = Stopwatch.StartNew();
				var response = await _trustService.GetTrustsByPagination(searchCriteria, _options.TrustsPerPage);
				stopwatch.Stop();

				Debug.WriteLine($"{nameof(RequestPage)} api call execution time {stopwatch.ElapsedMilliseconds} ms.");

				return response;
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				throw;
			}
		}

		private class SearchResult
		{
			public string NumberOfMatches { get; set; }
		}
	}
}