using AutoMapper;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Shared;
using Service.TRAMS.Cases;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public sealed class CaseCachedService : CachedService, ICaseCachedService
	{
		private readonly ILogger<CaseCachedService> _logger;
		private readonly ICaseService _caseService;
		private readonly IMapper _mapper;
		
		/// <summary>
		/// TODO Remove IMapper and project references when TRAMS API is live
		/// </summary>
		public CaseCachedService(IMapper mapper, ICacheProvider cacheProvider, ICaseService caseService, ILogger<CaseCachedService> logger) 
			: base(cacheProvider)
		{
			_caseService = caseService;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<CaseDto> PostCase(CreateCaseDto createCaseDto, string caseworker)
		{
			_logger.LogInformation("CaseCachedService::PostCase");

			// TODO Enable only when TRAMS API is live
			// Create case on TRAMS API
			//var newCase = await _caseService.PostCase(createCaseDto);
			//if (newCase is null) throw new ApplicationException("Error::CaseCachedService::PostCase");
			
			// TODO Remove when TRAMS API is live
			var newCase = _mapper.Map<CaseDto>(createCaseDto);
			newCase.Urn = BigIntegerSequence.Generator();
			
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