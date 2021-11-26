using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public sealed class CaseCachedService : CachedService, ICaseCachedService
	{
		private readonly ILogger<CaseCachedService> _logger;
		private readonly ICaseService _caseService;
		private readonly ICaseSearchService _caseSearchService;
		
		public CaseCachedService(ICacheProvider cacheProvider, ICaseService caseService, ICaseSearchService caseSearchService, ILogger<CaseCachedService> logger) 
			: base(cacheProvider)
		{
			_caseService = caseService;
			_caseSearchService = caseSearchService;
			_logger = logger;
		}

		public async Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(string caseworker, long statusUrn)
		{
			_logger.LogInformation("CaseCachedService::GetCasesByCaseworkerAndStatus");

			var userState = await GetData<UserState>(caseworker);
			if (userState != null) return userState.CasesDetails.Values.Select(c => c.CaseDto).ToList();

			var cases = await _caseSearchService.GetCasesByCaseworkerAndStatus(new CaseCaseWorkerSearch(caseworker, statusUrn));

			if (!cases.Any()) return cases;
			
			userState = new UserState();
			
			foreach (var caseDto in cases)
			{
				userState.CasesDetails.Add(caseDto.Urn, new CaseWrapper { CaseDto = caseDto });
			}
				
			await StoreData(caseworker, userState);
			
			return cases;
		}

		public async Task<CaseDto> GetCaseByUrn(string caseworker, long urn)
		{
			_logger.LogInformation("CaseCachedService::GetCaseByUrn");
			
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
			_logger.LogInformation("CaseCachedService::PostCase");
			
			// Create case on Academies API
			var newCase = await _caseService.PostCase(createCaseDto);
			if (newCase is null) throw new ApplicationException("Error::CaseCachedService::PostCase");
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(newCase.CreatedBy);
			if (userState is null)
			{
				userState = new UserState { CasesDetails = { { newCase.Urn, new CaseWrapper { CaseDto = newCase } } } };
			}
			else
			{
				// Maybe we need to check if a case urn already exists on CaseDetails, extract CaseWrapper and update.
				userState.CasesDetails.Add(newCase.Urn, new CaseWrapper { CaseDto = newCase });
			}
			await StoreData(newCase.CreatedBy, userState);

			return newCase;
		}

		public async Task PatchCaseByUrn(CaseDto caseDto)
		{
			_logger.LogInformation("CaseCachedService::PatchCaseByUrn");
			
			// Patch case on Academies API
			var patchCaseDto = await _caseService.PatchCaseByUrn(caseDto);
			if (patchCaseDto is null) throw new ApplicationException("Error::CaseCachedService::PatchCaseByUrn");
			
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

		/// <summary>
		/// TODO Primary is under review to maybe be removed in terms of business logic.
		/// </summary>
		/// <param name="caseworker"></param>
		/// <param name="caseUrn"></param>
		/// <returns></returns>
		public async Task<Boolean> IsCasePrimary(string caseworker, long caseUrn)
		{
			_logger.LogInformation("CaseCachedService::IsCasePrimary");
			
			// Fetch from cache expiration 24 hours (default)
			var userState = await GetData<UserState>(caseworker);
			if (userState is null) {
			
				// TODO Enable only when TRAMS API is live
				// Fetch cases by user
				//var cases = await _caseService.GetCasesByCaseworker(caseworker);
				//return !cases.Any();

				return true;
			}

			if (userState.CasesDetails.ContainsKey(caseUrn) && userState.CasesDetails.TryGetValue(caseUrn, out var caseWrapper))
			{
				return !caseWrapper.Records.Any();
			}

			return true;
		}
	}
}