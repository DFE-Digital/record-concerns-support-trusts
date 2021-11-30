using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public sealed class CaseCachedService : CachedService, ICaseCachedService
	{
		private readonly ILogger<CaseCachedService> _logger;
		private readonly ICaseService _caseService;
		private readonly ICaseSearchService _caseSearchService;
		
		private readonly SemaphoreSlim _semaphoreCasesCaseworkStatus = new SemaphoreSlim(1, 1);
		
		public CaseCachedService(ICacheProvider cacheProvider, ICaseService caseService, ICaseSearchService caseSearchService, ILogger<CaseCachedService> logger) 
			: base(cacheProvider)
		{
			_caseService = caseService;
			_caseSearchService = caseSearchService;
			_logger = logger;
		}

		public async Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(string caseworker, long statusUrn)
		{
			_logger.LogInformation("CaseCachedService::GetCasesByCaseworkerAndStatus {Caseworker} - {StatusUrn}", caseworker, statusUrn);

			var userState = await GetData<UserState>(caseworker);
			if (userState != null)
			{
				var cachedCases = userState.CasesDetails.Values.Where(caseWrapper => caseWrapper.CaseDto.StatusUrn.CompareTo(statusUrn) == 0)
					.Select(caseWrapper => caseWrapper.CaseDto).ToList();

				if (cachedCases.Any()) return cachedCases;
			}
			
			IList<CaseDto> casesDto = await _caseSearchService.GetCasesByCaseworkerAndStatus(new CaseCaseWorkerSearch(caseworker, statusUrn));
			if (!casesDto.Any()) return casesDto;
			
			await _semaphoreCasesCaseworkStatus.WaitAsync();

			userState = await GetData<UserState>(caseworker);
			userState ??= new UserState();

			Parallel.ForEach(casesDto, caseDto => userState.CasesDetails.TryAdd(caseDto.Urn, new CaseWrapper { CaseDto = caseDto }));
			
			await StoreData(caseworker, userState);
			
			_semaphoreCasesCaseworkStatus.Release();
				
			return casesDto;
		}
		
		public async Task<CaseDto> GetCaseByUrn(string caseworker, long urn)
		{
			_logger.LogInformation("CaseCachedService::GetCaseByUrn {Caseworker} - {CaseUrn}", caseworker, urn);
			
			var userState = await GetData<UserState>(caseworker);
			if (userState != null && userState.CasesDetails.TryGetValue(urn, out var caseWrapper))
			{
				return caseWrapper.CaseDto;
			}

			var caseDto = await _caseService.GetCaseByUrn(urn);
			userState ??= new UserState();
			userState.CasesDetails.Add(urn, new CaseWrapper { CaseDto = caseDto });

			await StoreData(caseworker, userState);
			
			return caseDto;
		}

		public async Task<CaseDto> PostCase(CreateCaseDto createCaseDto)
		{
			_logger.LogInformation("CaseCachedService::PostCase {Caseworker}", createCaseDto.CreatedBy);
			
			// Create case on Academies API
			var newCase = await _caseService.PostCase(createCaseDto);
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(newCase.CreatedBy);
			if (userState is null)
			{
				userState = new UserState { CasesDetails = { { newCase.Urn, new CaseWrapper { CaseDto = newCase } } } };
			}
			else
			{
				userState.CasesDetails.Add(newCase.Urn, new CaseWrapper { CaseDto = newCase });
			}
			
			await StoreData(newCase.CreatedBy, userState);

			return newCase;
		}

		public async Task PatchCaseByUrn(CaseDto caseDto)
		{
			_logger.LogInformation("CaseCachedService::PatchCaseByUrn {Caseworker} - {CaseUrn}", caseDto.CreatedBy, caseDto.Urn);
			
			// Patch case on Academies API
			var patchCaseDto = await _caseService.PatchCaseByUrn(caseDto);
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(patchCaseDto.CreatedBy);
			if (userState is null)
			{
				userState = new UserState { CasesDetails = { { patchCaseDto.Urn, new CaseWrapper { CaseDto = patchCaseDto } } } };
			}
			else
			{
				if (userState.CasesDetails.TryGetValue(patchCaseDto.Urn, out var caseWrapper))
				{
					caseWrapper.CaseDto = patchCaseDto;
				}
				else
				{
					userState.CasesDetails.Add(patchCaseDto.Urn, new CaseWrapper { CaseDto = patchCaseDto });
				}
			}
			
			await StoreData(patchCaseDto.CreatedBy, userState);
		}
	}
}