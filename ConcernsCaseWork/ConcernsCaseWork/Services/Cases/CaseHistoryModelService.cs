using AutoMapper;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Cases;
using Microsoft.Extensions.Logging;
using ConcernsCaseWork.Service.Cases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public sealed class CaseHistoryModelService : ICaseHistoryModelService
	{
		private readonly ICaseHistoryCachedService _caseHistoryCachedService;
		private readonly ILogger<CaseHistoryModelService> _logger;
		private readonly IMapper _mapper;

		public CaseHistoryModelService(ICaseHistoryCachedService caseHistoryCachedService, 
			IMapper mapper, 
			ILogger<CaseHistoryModelService> logger)
		{
			_caseHistoryCachedService = caseHistoryCachedService;
			_mapper = mapper;
			_logger = logger;
		}
		
		public async Task<IList<CaseHistoryModel>> GetCasesHistory(string caseworker, long caseUrn)
		{
			_logger.LogInformation("CaseHistoryModelService::GetCasesHistory");
			
			var casesHistoryDto = await _caseHistoryCachedService.GetCasesHistory(new CaseSearch(caseUrn), caseworker);

			// Map case history dto to model
			var casesHistoryModel = _mapper.Map<IList<CaseHistoryModel>>(casesHistoryDto);

			return casesHistoryModel;
		}
	}
}