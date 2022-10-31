using ConcernsCaseWork.Service.Base;
using ConcernsCaseWork.Service.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ConcernsCaseWork.Service.Cases
{
	public sealed class CaseSearchService : ICaseSearchService
	{
		private readonly ICaseService _caseService;
		private readonly ILogger<CaseSearchService> _logger;
		private readonly int _hardLimitByPagination;

		public CaseSearchService(ICaseService caseService, IOptions<TrustSearchOptions> options, ILogger<CaseSearchService> logger)
		{
			_caseService = caseService;
			_logger = logger;
			_hardLimitByPagination = options.Value.TrustsLimitByPage;
		}

		public async Task<IList<CaseDto>> GetCasesByCaseTrustSearch(CaseTrustSearch caseTrustSearch)
		{
			_logger.LogInformation("CaseSearchService::GetCasesByCaseTrustSearch execution");
			
			var stopwatch = Stopwatch.StartNew();
			var caseTrustsList = new List<CaseDto>();
			
			try
			{
				ApiListWrapper<CaseDto> apiListWrapperCaseDto;
				var nrRequests = 0;
				
				do
				{
					apiListWrapperCaseDto = await _caseService.GetCasesByTrustUkPrn(caseTrustSearch);
					
					// The following condition will break the loop.
					if (apiListWrapperCaseDto?.Data is null || !apiListWrapperCaseDto.Data.Any()) continue;
					
					caseTrustsList.AddRange(apiListWrapperCaseDto.Data);
					caseTrustSearch.PageIncrement();
					
					// Safe guard in case we have more than 10 pages.
					// We don't have a scenario at the moment, but feels like we need a limit.
					if ((nrRequests = Interlocked.Increment(ref nrRequests)) > _hardLimitByPagination)
					{
						break;
					}

				} while (apiListWrapperCaseDto?.Data != null && apiListWrapperCaseDto.Data.Any() && apiListWrapperCaseDto.Paging?.NextPageUrl != null);
			}
			finally
			{
				stopwatch.Stop();
				_logger.LogInformation("CaseSearchService::GetCasesByCaseTrustSearch execution time {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
			}
			
			return caseTrustsList;
		}

		public async Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(CaseCaseWorkerSearch caseCaseWorkerSearch)
		{
			_logger.LogInformation("CaseSearchService::GetCasesByCaseworkerAndStatus execution");

			var stopwatch = Stopwatch.StartNew();
			var casesList = new List<CaseDto>();

			try
			{
				ApiListWrapper<CaseDto> apiListWrapperCaseDto;
				var nrRequests = 0;

				do
				{
					apiListWrapperCaseDto = await _caseService.GetCasesByCaseworkerAndStatus(caseCaseWorkerSearch);

					// The following condition will break the loop.
					if (apiListWrapperCaseDto?.Data is null || !apiListWrapperCaseDto.Data.Any()) continue;

					casesList.AddRange(apiListWrapperCaseDto.Data);
					caseCaseWorkerSearch.PageIncrement();

					// Safe guard in case we have more than 10 pages.
					// We don't have a scenario at the moment, but feels like we need a limit.
					if ((nrRequests = Interlocked.Increment(ref nrRequests)) > _hardLimitByPagination)
					{
						break;
					}

				} while (apiListWrapperCaseDto?.Data != null && apiListWrapperCaseDto.Data.Any() && apiListWrapperCaseDto.Paging?.NextPageUrl != null);
			}
			finally
			{
				stopwatch.Stop();
				_logger.LogInformation("CaseSearchService::GetCasesByCaseworkerAndStatus execution time {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
			}

			return casesList;
		}

		public async Task<IList<CaseDto>> GetCasesByPageSearch(PageSearch pageSearch)
		{
			_logger.LogInformation("CaseSearchService::GetCasesByPageSearch execution");
			
			var stopwatch = Stopwatch.StartNew();
			var casesList = new List<CaseDto>();
			
			try
			{
				ApiListWrapper<CaseDto> apiListWrapperCaseDto;
				var nrRequests = 0;
				
				do
				{
					apiListWrapperCaseDto = await _caseService.GetCases(pageSearch);
					
					// The following condition will break the loop.
					if (apiListWrapperCaseDto?.Data is null || !apiListWrapperCaseDto.Data.Any()) continue;
					
					casesList.AddRange(apiListWrapperCaseDto.Data);
					pageSearch.PageIncrement();
					
					// Safe guard in case we have more than 10 pages.
					// We don't have a scenario at the moment, but feels like we need a limit.
					if ((nrRequests = Interlocked.Increment(ref nrRequests)) > _hardLimitByPagination)
					{
						break;
					}

				} while (apiListWrapperCaseDto?.Data != null && apiListWrapperCaseDto.Data.Any() && apiListWrapperCaseDto.Paging?.NextPageUrl != null);
			}
			finally
			{
				stopwatch.Stop();
				_logger.LogInformation("CaseSearchService::GetCasesByPageSearch execution time {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
			}
			
			return casesList;
		}
	}
}