﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.TRAMS.Base;
using Service.TRAMS.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public sealed class CaseSearchService : ICaseSearchService
	{
		private readonly ICaseService _caseService;
		private readonly ILogger<CaseSearchService> _logger;
		private readonly int _hardLimitTrustsByPagination;

		public CaseSearchService(ICaseService caseService, IOptions<TrustSearchOptions> options, ILogger<CaseSearchService> logger)
		{
			_caseService = caseService;
			_logger = logger;
			_hardLimitTrustsByPagination = options.Value.TrustsLimitByPage;
		}

		public async Task<IList<CaseDto>> GetCasesBySearchCriteria(CaseTrustSearch caseTrustSearch)
		{
			_logger.LogInformation("CaseSearchService::GetCasesBySearchCriteria execution");
			
			var caseTrustsList = new List<CaseDto>();
			ApiWrapper<CaseDto> apiWrapperCaseDto;
			var nrRequests = 0;
			
			do
			{
				apiWrapperCaseDto = await _caseService.GetCasesByTrustUkPrn(caseTrustSearch);
				
				if (apiWrapperCaseDto?.Data is null || !apiWrapperCaseDto.Data.Any()) continue;
				
				caseTrustsList.AddRange(apiWrapperCaseDto.Data);
				caseTrustSearch.PageIncrement();
				
				// Safe guard in case we have more than 10 pages.
				// We don't have a scenario at the moment, but feels like we need a limit.
				if ((nrRequests = Interlocked.Increment(ref nrRequests)) > _hardLimitTrustsByPagination)
				{
					break;
				}

			} while (apiWrapperCaseDto?.Data != null && apiWrapperCaseDto.Data.Any() && apiWrapperCaseDto.Paging?.NextPageUrl != null);
			
			return caseTrustsList;
		}
	}
}