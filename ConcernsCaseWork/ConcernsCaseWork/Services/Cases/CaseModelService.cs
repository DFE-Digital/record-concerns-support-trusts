using AutoMapper;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Redis;
using Microsoft.Extensions.Logging;
using Service.Redis.Services;
using Service.TRAMS.Cases;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public sealed class CaseModelService : ICaseModelService
	{
		private readonly ILogger<CaseModelService> _logger;
		private readonly ICachedService _cachedService;
		private readonly ITrustService _trustService;
		private readonly ICaseService _caseService;
		private readonly IMapper _mapper;
		
		public CaseModelService(ICaseService caseService, ICachedService cachedService, 
			ITrustService trustService, IMapper mapper, ILogger<CaseModelService> logger)
		{
			_cachedService = cachedService;
			_trustService = trustService;
			_caseService = caseService;
			_mapper = mapper;
			_logger = logger;
		}
		
		/// <summary>
		/// Return first parameter -> Active cases
		/// Return second parameter -> Monitoring cases
		/// </summary>
		/// <param name="caseworker"></param>
		/// <returns></returns>
		public async Task<(IList<HomeUiModel>, IList<HomeUiModel>)> GetCasesByCaseworker(string caseworker)
		{
			try
			{
				// TODO Find cases within redis cache first until we don't have TRAMS API.
				var caseStateModel = await _cachedService.GetData<CaseStateModel>(caseworker);
				if (caseStateModel != null)
				{
					
					
					
					
				}
				
				
				
				// Get from TRAMS API all trusts for the caseworker
				var casesDto = await _caseService.GetCasesByCaseworker(caseworker);
				
				// If cases available, fetch trust by ukprn data.
				if (casesDto.Any())
				{
					// Fetch trusts by ukprn
					var trusts = casesDto.Where(c => c.TrustUkPrn != null)
						.Select(c => _trustService.GetTrustByUkPrn(c.TrustUkPrn));
					await Task.WhenAll(trusts);
					
					// Fetch rag rating
					
					


				}
				
				
				// Map trusts dto to model
				_mapper.Map<IList<CaseModel>>(casesDto);
				
				
				return (null, null);
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseModelService::GetCasesByCaseworker exception {ex.Message}");
			}
			
			return (Array.Empty<HomeUiModel>(), Array.Empty<HomeUiModel>());
		}
	}
}