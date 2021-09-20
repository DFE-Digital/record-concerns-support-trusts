using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
		
		public CaseCachedService(ICacheProvider cacheProvider, ICaseService caseService, ILogger<CaseCachedService> logger) 
			: base(cacheProvider)
		{
			_caseService = caseService;
			_logger = logger;
		}

		public async Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker, string statusUrn = "Live")
		{
			_logger.LogInformation("CaseCachedService::GetCasesByCaseworker");

			var userState = await GetData<UserState>(caseworker);
			if (userState != null) return userState.CasesDetails.Values.Select(c => c.CaseDto).ToList();
			
			var cases = await _caseService.GetCasesByCaseworker(caseworker, statusUrn);
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

			// TODO Enable only when TRAMS API is live
			// Create case on TRAMS API
			//var newCase = await _caseService.PostCase(createCaseDto);
			//if (newCase is null) throw new ApplicationException("Error::CaseCachedService::PostCase");
			
			// TODO Start Remove when TRAMS API is live
			var createCaseDtoStr = JsonConvert.SerializeObject(createCaseDto);
			var newCase = JsonConvert.DeserializeObject<CaseDto>(createCaseDtoStr);
			// TODO End Remove when TRAMS API is live
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(createCaseDto.CreatedBy);
			if (userState is null)
			{
				userState = new UserState { CasesDetails = { { newCase.Urn, new CaseWrapper { CaseDto = newCase } } } };
			}
			else
			{
				userState.CasesDetails.Add(newCase.Urn, new CaseWrapper { CaseDto = newCase });
			}
			await StoreData(createCaseDto.CreatedBy, userState);

			return newCase;
		}

		public async Task PatchCaseByUrn(CaseDto caseDto)
		{
			_logger.LogInformation("CaseCachedService::PatchCaseByUrn");
			
			// TODO Enable only when TRAMS API is live
			// Patch case on TRAMS API
			//var patchCase = await _caseService.PatchCaseByUrn(caseDto);
			//if (patchCase is null) throw new ApplicationException("Error::CaseCachedService::PatchCaseByUrn");
			
			// Store in cache for 24 hours (default)
			var userState = await GetData<UserState>(caseDto.CreatedBy);
			if (userState is null)
			{
				userState = new UserState { CasesDetails = { { caseDto.Urn, new CaseWrapper { CaseDto = caseDto } } } };
			}
			else
			{
				if (userState.CasesDetails.TryGetValue(caseDto.Urn, out var caseWrapper))
				{
					caseWrapper.CaseDto = caseDto;
				}
				else
				{
					userState.CasesDetails.Add(caseDto.Urn, new CaseWrapper { CaseDto = caseDto });
				}
			}
			await StoreData(caseDto.CreatedBy, userState);
		}

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