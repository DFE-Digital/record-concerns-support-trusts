using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Rating;
using Service.Redis.Type;
using Service.TRAMS.RecordRatingHistory;
using Service.TRAMS.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public sealed class CaseModelService : ICaseModelService
	{
		private readonly IRatingCachedService _ratingCachedService;
		private readonly IRecordRatingHistory _recordRatingHistory;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ILogger<CaseModelService> _logger;
		private readonly ICachedService _cachedService;
		private readonly IRecordService _recordService;
		private readonly ITrustService _trustService;
		private readonly ICaseService _caseService;
		private readonly IMapper _mapper;
		
		public CaseModelService(ICaseService caseService, ITrustService trustService, 
			IRecordService recordService, IRatingCachedService ratingCachedService, 
			ITypeCachedService typeCachedService, ICachedService cachedService, 
			IRecordRatingHistory recordRatingHistory, IMapper mapper, 
			ILogger<CaseModelService> logger)
		{
			_ratingCachedService = ratingCachedService;
			_recordRatingHistory = recordRatingHistory;
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
		public async Task<(IList<HomeModel>, IList<HomeModel>)> GetCasesByCaseworker(string caseworker)
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

		public Task<HomeModel> GetCasesByUrn(string urn)
		{
			throw new NotImplementedException();
		}

		public Task<IList<HomeModel>> GetCasesByTrustUkPrn(string trustUkprn)
		{
			throw new NotImplementedException();
		}

		public Task<IList<HomeModel>> GetCasesByPagination(CaseSearch caseSearch)
		{
			// TODO when Admin page is created.
			throw new NotImplementedException();
		}

		public async Task<HomeModel> PostCase(HomeModel homeModel)
		{
			try
			{
				// TODO Enable only when TRAMS API Concerns endpoints are live.
				
				
				// Create a case
				// TODO status urn, created by
				
				var currentDate = DateTime.Now;
				
				
				var createCaseDto = new CreateCaseDto(currentDate, currentDate, currentDate, currentDate, "created-by",
					"description", "crm-enquiry", "trust-ukprn", "reason-at-review",
					currentDate, "issue", "current-status", "next-steps", "resolution-strategy",
					"direction-of-travel", BigInteger.Zero);
				var newCase = await _caseService.PostCase(createCaseDto);

				// Create a record
				// TODO get type urn, rating urn, status urn, is it primary?
				
				var createRecordDto = new CreateRecordDto(currentDate, currentDate, currentDate, currentDate, "name",
					"description", "reason", BigInteger.Zero, BigInteger.Zero, BigInteger.Zero, true, 
					BigInteger.Zero);
				var newRecord = await _recordService.PostRecordByCaseUrn(createRecordDto);

				// Create a rating history
				// TODO get rating urn
				
				var createRecordRatingHistoryDto = new RecordRatingHistoryDto(currentDate, newRecord.Urn, BigInteger.Zero);
				await _recordRatingHistory.PostRecordRatingHistory(createRecordRatingHistoryDto);


				// Store all records in cache
				
				
				
				
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseModelService::GetCasesByPagination exception {ex.Message}");
			}

			return null;
		}

		public Task<HomeModel> PatchCaseByUrn(HomeModel homeModel)
		{
			throw new NotImplementedException();
		}


		private async Task<(IList<HomeModel>, IList<HomeModel>)> FetchFromTramsApi(string caseworker)
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

		private async Task<(IList<HomeModel>, IList<HomeModel>)> FetchFromCache(CaseState caseState)
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

		private static (IList<HomeModel>, IList<HomeModel>) Empty()
		{
			var emptyResults = Array.Empty<HomeModel>();
			return (emptyResults, emptyResults);
		}
	}
}