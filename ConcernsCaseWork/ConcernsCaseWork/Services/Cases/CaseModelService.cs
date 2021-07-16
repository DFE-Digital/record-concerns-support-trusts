using AutoMapper;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public sealed class CaseModelService : ICaseModelService
	{
		private readonly ILogger<CaseModelService> _logger;
		private readonly ICaseService _caseService;
		private readonly IMapper _mapper;
		
		public CaseModelService(ILogger<CaseModelService> logger, ICaseService caseService, IMapper mapper)
		{
			_logger = logger;
			_caseService = caseService;
			_mapper = mapper;
		}
		
		public async Task<IList<CaseModel>> GetCasesByCaseworker(string caseworker)
		{
			try
			{
				// Get from TRAMS API all trusts for the caseworker
				var casesDto = await _caseService.GetCasesByCaseworker(caseworker);
				
				// Map trusts dto to model
				return _mapper.Map<IList<CaseModel>>(casesDto);
			}
			catch (Exception ex)
			{
				_logger.LogError("TrustModelService::GetTrustsByCaseworker exception {Message}", ex.Message);
			}
			
			return Array.Empty<CaseModel>();
		}
	}
}