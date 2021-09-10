using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System;
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

		public async Task<CaseDto> PostCase(CreateCaseDto createCaseDto, string caseworker)
		{
			_logger.LogInformation("CaseCachedService::PostCase");

			// Create case on TRAMS API
			var newCase = await _caseService.PostCase(createCaseDto);
			if (newCase is null) throw new ApplicationException("Error::CaseCachedService::PostCase");
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				caseState = new UserState { CasesDetails = { { newCase.Urn, new CaseWrapper { CaseDto = newCase } } } };
			}
			else
			{
				caseState.CasesDetails.Add(newCase.Urn, new CaseWrapper { CaseDto = newCase });
			}
			await StoreData(caseworker, caseState);
			
			return newCase;
		}
	}
}