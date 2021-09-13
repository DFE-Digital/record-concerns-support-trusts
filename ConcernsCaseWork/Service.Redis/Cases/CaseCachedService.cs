using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public sealed class CaseCachedService : CachedService, ICaseCachedService
	{
		private readonly ILogger<CaseCachedService> _logger;
		private readonly ICaseService _caseService;

		/// <summary>
		/// TODO Remove IMapper and project references when TRAMS API is live
		/// </summary>
		public CaseCachedService(ICacheProvider cacheProvider, ICaseService caseService, ILogger<CaseCachedService> logger) 
			: base(cacheProvider)
		{
			_caseService = caseService;
			_logger = logger;
		}

		public async Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker, string statusUrn = "Live")
		{
			_logger.LogInformation("CaseCachedService::GetCasesByCaseworker");

			var caseState = await GetData<UserState>(caseworker);
			if (caseState is { }) return caseState.CasesDetails.Values.Select(c => c.CaseDto).ToImmutableList();
			
			var cases = await _caseService.GetCasesByCaseworker(caseworker, statusUrn);
			var userState = new UserState();
			foreach (var caseDto in cases)
			{
				userState.CasesDetails.Add(caseDto.Urn, new CaseWrapper { CaseDto = caseDto });
			}
				
			await StoreData(caseworker, userState);

			return cases;
		}

		public async Task<CaseDto> PostCase(CreateCaseDto createCaseDto)
		{
			_logger.LogInformation("CaseCachedService::PostCase");

			// TODO Enable only when TRAMS API is live
			// Create case on TRAMS API
			//var newCase = await _caseService.PostCase(createCaseDto);
			//if (newCase is null) throw new ApplicationException("Error::CaseCachedService::PostCase");
			
			// TODO Remove when TRAMS API is live
			var createCaseDtoStr = JsonConvert.SerializeObject(createCaseDto);
			var newCase = JsonConvert.DeserializeObject<CaseDto>(createCaseDtoStr);
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(createCaseDto.CreatedBy);
			if (caseState is null)
			{
				caseState = new UserState { CasesDetails = { { newCase.Urn, new CaseWrapper { CaseDto = newCase } } } };
			}
			else
			{
				caseState.CasesDetails.Add(newCase.Urn, new CaseWrapper { CaseDto = newCase });
			}
			await StoreData(createCaseDto.CreatedBy, caseState);
			
			return newCase;
		}

		public async Task<Boolean> IsCasePrimary(string caseworker)
		{
			_logger.LogInformation("CaseCachedService::IsCasePrimary");
			
			// Fetch from cache expiration 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null) {
			
				// TODO Enable only when TRAMS API is live
				// Fetch cases by user
				//var cases = await _caseService.GetCasesByCaseworker(caseworker);
				//return !cases.Any();

				return true;
			}

			return !caseState.CasesDetails.Any();
		}
	}
}