using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Sequence;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public sealed class CaseHistoryCachedService : CachedService, ICaseHistoryCachedService
	{
		private readonly ILogger<CaseHistoryCachedService> _logger;
		private readonly ICaseSearchService _caseSearchService;
		private readonly ISequenceCachedService _sequenceCachedService;
		private readonly ICaseHistoryService _caseHistoryService;
		
		public CaseHistoryCachedService(ICacheProvider cacheProvider, 
			ICaseHistoryService caseHistoryService, 
			ICaseSearchService caseSearchService,
			ILogger<CaseHistoryCachedService> logger, 
			ISequenceCachedService sequenceCachedService) : base(cacheProvider)
		{
			_sequenceCachedService = sequenceCachedService;
			_caseHistoryService = caseHistoryService;
			_caseSearchService = caseSearchService;
			_logger = logger;
		}
		
		public async Task PostCaseHistory(CreateCaseHistoryDto createCaseHistoryDto, string caseworker)
		{
			_logger.LogInformation("CaseHistoryCachedService::PostCaseHistory");
			
			// TODO Enable only when Academies API is live
			 //var postCaseHistory = await _caseHistoryService.PostCaseHistory(createCaseHistoryDto);
			// if (postCaseHistory is null) throw new ApplicationException("Error::CaseHistoryCachedService::PostCaseHistory");

			// TODO Remove when Academies API is live
			createCaseHistoryDto.Urn = await _sequenceCachedService.Generator();
			var createCaseHistoryDtoStr = JsonConvert.SerializeObject(createCaseHistoryDto);
			var newCaseHistoryDto = JsonConvert.DeserializeObject<CaseHistoryDto>(createCaseHistoryDtoStr);
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null)
			{
				var caseWrapper = new CaseWrapper();
				caseWrapper.CasesHistoryDto.Add(newCaseHistoryDto);
				userState = new UserState(caseworker) { CasesDetails = { { createCaseHistoryDto.CaseUrn, caseWrapper } } };
			}
			else
			{
				if (userState.CasesDetails.ContainsKey(createCaseHistoryDto.CaseUrn) 
				    && userState.CasesDetails.TryGetValue(createCaseHistoryDto.CaseUrn, out var caseWrapper))
				{
					caseWrapper.CasesHistoryDto.Add(newCaseHistoryDto);
				}
				else
				{
					caseWrapper = new CaseWrapper();
					caseWrapper.CasesHistoryDto.Add(newCaseHistoryDto);
					
					userState.CasesDetails.Add(createCaseHistoryDto.CaseUrn, caseWrapper);
				}
			}
			
			await StoreData(caseworker, userState);
		}

		public async Task<IList<CaseHistoryDto>> GetCasesHistory(CaseSearch caseSearch, string caseworker)
		{
			_logger.LogInformation("CaseHistoryCachedService::GetCasesHistory");
			
			var userState = await GetData<UserState>(caseworker);
			// TODO Include when Academies API is live
			// if (userState != null && userState.CasesDetails.TryGetValue(caseSearch.CaseUrn, out var caseWrapper) && caseWrapper.CasesHistoryDto.Any())
			if (userState != null && userState.CasesDetails.TryGetValue(caseSearch.CaseUrn, out var caseWrapper))
			{
				return caseWrapper.CasesHistoryDto;
			}

			return Array.Empty<CaseHistoryDto>();

			// TODO Include when Academies API is live
			// var casesHistory = await _caseSearchService.GetCasesHistoryByCaseSearch(caseSearch);
			// userState ??= new UserState();
			// userState.CasesDetails.Add(caseSearch.CaseUrn, new CaseWrapper { CasesHistoryDto = casesHistory });
			//
			// await StoreData(caseworker, userState);
			//
			// return casesHistory;
		}
	}
}