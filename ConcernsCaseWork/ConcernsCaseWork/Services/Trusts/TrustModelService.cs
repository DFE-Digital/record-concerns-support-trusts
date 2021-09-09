using AutoMapper;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Trusts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trusts
{
	public sealed class TrustModelService : ITrustModelService
	{
		private readonly ITrustSearchService _trustSearchService;
		private readonly ITrustService _trustService;
		private readonly IMapper _mapper;
		private readonly ILogger<TrustModelService> _logger;

		public TrustModelService(ITrustSearchService trustSearchService, ITrustService trustService, IMapper mapper, ILogger<TrustModelService> logger)
		{
			_trustSearchService = trustSearchService;
			_trustService = trustService;
			_mapper = mapper;
			_logger = logger;
		}
		
		public async Task<IList<TrustSummaryModel>> GetTrustsBySearchCriteria(TrustSearch trustSearch)
		{
			_logger.LogInformation("TrustModelService::GetTrustsBySearchCriteria");
			
			// Fetch trusts by criteria
			var trustsDto = await _trustSearchService.GetTrustsBySearchCriteria(trustSearch);
			
			// Map trusts dto to model
			var trustsSummary = _mapper.Map<IList<TrustSummaryModel>>(trustsDto);

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
			var trustsDetailsDto = await _trustService.GetTrustByUkPrn(ukPrn);
			
			// Map trust to model
			var trustDetailsModel = _mapper.Map<TrustDetailsModel>(trustsDetailsDto);

			return trustDetailsModel;
		}
	}
}