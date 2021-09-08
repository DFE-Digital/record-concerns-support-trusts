using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Rating;
using Service.Redis.Status;
using Service.Redis.Type;
using Service.TRAMS.Cases;
using Service.TRAMS.Records;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public sealed class CaseModelService : ICaseModelService
	{
		private readonly IRatingCachedService _ratingCachedService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ILogger<CaseModelService> _logger;
		private readonly ICachedService _cachedService;
		private readonly IRecordService _recordService;
		private readonly ITrustService _trustService;
		private readonly ICaseService _caseService;
		private readonly IMapper _mapper;
		
		public CaseModelService(ICaseService caseService, ITrustService trustService, 
			IRecordService recordService, IRatingCachedService ratingCachedService, 
			IStatusCachedService statusCachedService, ITypeCachedService typeCachedService,
			ICachedService cachedService, IMapper mapper, ILogger<CaseModelService> logger)
		{
			_ratingCachedService = ratingCachedService;
			_statusCachedService = statusCachedService;
			_typeCachedService = typeCachedService;
			_cachedService = cachedService;
			_recordService = recordService;
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
				// Find cases redis cache
				var caseState = await _cachedService.GetData<CaseState>(caseworker);
				if (caseState != null)
				{
					return await FetchFromCache(caseState);
				}
				
				// Find cases TramsApi
				return await FetchFromTramsApi(caseworker);
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseModelService::GetCasesByCaseworker exception {ex.Message}");
			}

			return Empty();
		}


		private async Task<(IList<HomeUiModel>, IList<HomeUiModel>)> FetchFromTramsApi(string caseworker)
		{
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
				var ragsRatingDto = await _ratingCachedService.GetRatings();
							
				// Fetch types
				var typesDto = await _typeCachedService.GetTypes();

				// Map dto to model
				var casesModel = _mapper.Map<IList<CaseModel>>(casesDto);
				var trustsDetailsModel = _mapper.Map<IList<TrustDetailsModel>>(trustsDetailsDto);
				var recordsModel = _mapper.Map<IList<RecordModel>>(recordsDto);
				var ragsRatingModel = _mapper.Map<IList<RatingModel>>(ragsRatingDto);
				var typesModel = _mapper.Map<IList<TypeModel>>(typesDto);

				return HomeMapping.Map(casesModel, trustsDetailsModel, recordsModel, 
					ragsRatingModel, typesModel);
			}

			return Empty();
		}

		private async Task<(IList<HomeUiModel>, IList<HomeUiModel>)> FetchFromCache(CaseState caseState)
		{
			// Get cases for case worker
			var caseDetails = caseState.CasesDetails;
					
			// Get cases from cache
			var casesDto = caseDetails.Select(c => c.Value.Cases);
					
			// Fetch trusts by ukprn
			var trustsDetailsDto = casesDto.Where(c => c.TrustUkPrn != null)
				.Select(c => _trustService.GetTrustByUkPrn(c.TrustUkPrn));
			await Task.WhenAll(trustsDetailsDto);
					
			// Fetch records by case urn
			var recordsDto = caseDetails.Select(c => c.Value.Records)
				.Select(r => r.Values)
				.FirstOrDefault();
					
			// Fetch rag rating
			var ragsRatingDto = await _ratingCachedService.GetRatings();
						
			// Fetch types
			var typesDto = await _typeCachedService.GetTypes();
					
			// Map dto to model
			var casesModel = _mapper.Map<IList<CaseModel>>(casesDto);
			var trustsDetailsModel = _mapper.Map<IList<TrustDetailsModel>>(trustsDetailsDto);
			var recordsModel = _mapper.Map<IList<RecordModel>>(recordsDto);
			var ragsRatingModel = _mapper.Map<IList<RatingModel>>(ragsRatingDto);
			var typesModel = _mapper.Map<IList<TypeModel>>(typesDto);

			return HomeMapping.Map(casesModel, trustsDetailsModel, recordsModel, 
				ragsRatingModel, typesModel);
		}

		private static (IList<HomeUiModel>, IList<HomeUiModel>) Empty()
		{
			var emptyResults = Array.Empty<HomeUiModel>();
			return (emptyResults, emptyResults);
		}
	}
}