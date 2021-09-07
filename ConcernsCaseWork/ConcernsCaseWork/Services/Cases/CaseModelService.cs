using AutoMapper;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Redis;
using Microsoft.Extensions.Logging;
using Service.Redis.Cases;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public sealed class CaseModelService : ICaseModelService
	{
		private readonly ICaseService _caseService;
		private readonly ICasesCachedService _casesCachedService;
		private readonly IMapper _mapper;
		private readonly ILogger<CaseModelService> _logger;
		
		public CaseModelService(ICaseService caseService, ICasesCachedService casesCachedService, IMapper mapper, ILogger<CaseModelService> logger)
		{
			_caseService = caseService;
			_casesCachedService = casesCachedService;
			_mapper = mapper;
			_logger = logger;
		}
		
		/// <summary>
		/// Return first parameter -> Active cases
		/// Return second parameter -> Monitoring cases
		/// </summary>
		/// <param name="caseworker"></param>
		/// <returns></returns>
		public async Task<(IList<HomeModel>, IList<HomeModel>)> GetCasesByCaseworker(string caseworker)
		{
			try
			{
				// Find cases within redis cache first
				var caseStateModel = await _casesCachedService.GetCaseData<CaseStateModel>(caseworker);
				if (caseStateModel != null) return caseStateModel.;
				
				// Get from TRAMS API all trusts for the caseworker
				var casesDto = await _caseService.GetCasesByCaseworker(caseworker);
				
				// Map trusts dto to model
				_mapper.Map<IList<CaseModel>>(casesDto);
				
				
				return (null, null);
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseModelService::GetCasesByCaseworker exception {ex.Message}");
			}
			
			return (Array.Empty<HomeModel>(), Array.Empty<HomeModel>());
		}
	}
}