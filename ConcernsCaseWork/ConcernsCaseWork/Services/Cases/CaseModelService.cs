using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Redis;
using Microsoft.Extensions.Logging;
using Service.Redis.Services;
using Service.TRAMS.Cases;
using Service.TRAMS.Rating;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
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
		private readonly IRatingService _ratingService;
		private readonly IRecordService _recordService;
		private readonly IStatusService _statusService;
		private readonly ITrustService _trustService;
		private readonly ICaseService _caseService;
		private readonly IMapper _mapper;
		
		public CaseModelService(ICaseService caseService, ITrustService trustService, 
			IRecordService recordService, IRatingService ratingService, IStatusService statusService,
			ICachedService cachedService, IMapper mapper, ILogger<CaseModelService> logger)
		{
			_cachedService = cachedService;
			_recordService = recordService;
			_ratingService = ratingService;
			_statusService = statusService;
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
					var trustsDetailsDto = casesDto.Where(c => c.TrustUkPrn != null)
						.Select(c => _trustService.GetTrustByUkPrn(c.TrustUkPrn));
					await Task.WhenAll(trustsDetailsDto);
					
					// Fetch records by case urn
					var recordsDto = casesDto.Select(c => _recordService.GetRecordsByCaseUrn(c.Urn));
					await Task.WhenAll(recordsDto);
					
					// Fetch rag rating
					var ragsRatingDto = await _ratingService.GetRatings();
					
					// Fetch status
					

					// Map trusts dto to model
					var casesModel = _mapper.Map<IList<CaseModel>>(casesDto);
					var trustsDetailsModel = _mapper.Map<IList<TrustDetailsModel>>(trustsDetailsDto);
					var recordsModel = _mapper.Map<IList<RecordModel>>(recordsDto);
					var ragsRatingModel = _mapper.Map<IList<RatingModel>>(ragsRatingDto);

					return HomeMapping.Map(casesModel, trustsDetailsModel, recordsModel, ragsRatingModel);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseModelService::GetCasesByCaseworker exception {ex.Message}");
			}
			
			var emptyResults = Array.Empty<HomeUiModel>();
			return (emptyResults, emptyResults);
		}
	}
}