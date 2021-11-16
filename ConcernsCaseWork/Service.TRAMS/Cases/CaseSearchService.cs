using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.TRAMS.Base;
using Service.TRAMS.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public sealed class CaseSearchService : ICaseSearchService
	{
		private readonly ICaseHistoryService _caseHistoryService;
		private readonly ICaseService _caseService;
		private readonly ILogger<CaseSearchService> _logger;
		private readonly int _hardLimitByPagination;

		public CaseSearchService(ICaseService caseService, ICaseHistoryService caseHistoryService, IOptions<TrustSearchOptions> options, ILogger<CaseSearchService> logger)
		{
			_caseService = caseService;
			_caseHistoryService = caseHistoryService;
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

		public async Task<IList<CaseHistoryDto>> GetCasesHistoryByCaseSearch(CaseSearch caseSearch)
		{
			_logger.LogInformation("CaseSearchService::GetCasesHistoryByCaseSearch execution");
			
			var stopwatch = Stopwatch.StartNew();
			var casesHistoryList = new List<CaseHistoryDto>();
			
			try
			{
				ApiListWrapper<CaseHistoryDto> apiListWrapperCaseHistoryDto;
				var nrRequests = 0;
				
				do
				{
					apiListWrapperCaseHistoryDto = await _caseHistoryService.GetCasesHistory(caseSearch);
					
					// The following condition will break the loop.
					if (apiListWrapperCaseHistoryDto?.Data is null || !apiListWrapperCaseHistoryDto.Data.Any()) continue;
					
					casesHistoryList.AddRange(apiListWrapperCaseHistoryDto.Data);
					caseSearch.PageIncrement();
					
					// Safe guard in case we have more than 10 pages.
					// We don't have a scenario at the moment, but feels like we need a limit.
					if ((nrRequests = Interlocked.Increment(ref nrRequests)) > _hardLimitByPagination)
					{
						break;
					}

				} while (apiListWrapperCaseHistoryDto?.Data != null && apiListWrapperCaseHistoryDto.Data.Any() && apiListWrapperCaseHistoryDto.Paging?.NextPageUrl != null);
			}
			finally
			{
				stopwatch.Stop();
				_logger.LogInformation("CaseSearchService::GetCasesHistoryByCaseSearch execution time {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
			}
			
			return casesHistoryList;
		}
	}
}