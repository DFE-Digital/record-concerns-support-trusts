using AutoMapper;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Trusts;
using ConcernsCasework.Service.Trusts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trusts
{
	public sealed class TrustModelService : ITrustModelService
	{
		private readonly ITrustSearchService _trustSearchService;
		private readonly ITrustCachedService _trustCachedService;
		private readonly IMapper _mapper;
		private readonly ILogger<TrustModelService> _logger;

		public TrustModelService(ITrustSearchService trustSearchService, ITrustCachedService trustCachedService, IMapper mapper, ILogger<TrustModelService> logger)
		{
			_trustSearchService = trustSearchService;
			_trustCachedService = trustCachedService;
			_mapper = mapper;
			_logger = logger;
		}
		
		public async Task<IList<TrustSearchModel>> GetTrustsBySearchCriteria(TrustSearch trustSearch)
		{
			_logger.LogInformation("TrustModelService::GetTrustsBySearchCriteria");
			
			// Fetch trusts by criteria
			var trustsSearchDto = await _trustSearchService.GetTrustsBySearchCriteria(trustSearch);
			
			// Map trusts dto to model
			var trustsSummary = _mapper.Map<IList<TrustSearchModel>>(trustsSearchDto);

			// Filter trusts that haven't correct properties and sort
			var trustsSummaryOrderedFiltered = from t in trustsSummary 
				where (t.GroupName != null && t.UkPrn != null) 
				orderby t.GroupName select t;

			return trustsSummaryOrderedFiltered.ToList();
		}

		public async Task<TrustDetailsModel> GetTrustByUkPrn(string ukPrn)
		{
			_logger.LogInformation("TrustModelService::GetTrustByUkPrn");
			
			// Fetch trust by ukprn
			var trustDetailsDto = await _trustCachedService.GetTrustByUkPrn(ukPrn);
			
			// Map trust dto to model
			var trustDetailsModel = _mapper.Map<TrustDetailsModel>(trustDetailsDto);

			return trustDetailsModel;
		}
	}
}