using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Rating;
using Service.Redis.RecordRatingHistory;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Type;
using Service.TRAMS.Cases;
using Service.TRAMS.RecordRatingHistory;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public sealed class CaseModelService : ICaseModelService
	{
		private readonly IRecordRatingHistoryCachedService _recordRatingHistoryCachedService;
		private readonly IRatingCachedService _ratingCachedService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly ITrustCachedService _trustCachedService;
		private readonly ICaseCachedService _caseCachedService;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ILogger<CaseModelService> _logger;
		private readonly ICachedService _cachedService;
		private readonly IRecordService _recordService;
		private readonly IMapper _mapper;
		
		public CaseModelService(ICaseCachedService caseCachedService, ITrustCachedService trustCachedService, 
			IRecordService recordService, IRatingCachedService ratingCachedService,
			ITypeCachedService typeCachedService, ICachedService cachedService, 
			IRecordRatingHistoryCachedService recordRatingHistoryCachedService,
			IStatusCachedService statusCachedService, 
			IMapper mapper, ILogger<CaseModelService> logger)
		{
			_recordRatingHistoryCachedService = recordRatingHistoryCachedService;
			_statusCachedService = statusCachedService;
			_ratingCachedService = ratingCachedService;
			_trustCachedService = trustCachedService;
			_caseCachedService = caseCachedService;
			_typeCachedService = typeCachedService;
			_cachedService = cachedService;
			_recordService = recordService;
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
				var caseState = await _cachedService.GetData<UserState>(caseworker);
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

		public async Task<CaseModel> PostCase(CreateCaseModel createCaseModel)
		{
			try
			{
				// Fetch Status
				var statusDto = await _statusCachedService.GetStatusByName(Status.Live.ToString());

				// Fetch Type
				var typeDto = await _typeCachedService.GetTypeByNameAndDescription(
					createCaseModel.RecordType, createCaseModel.RecordSubType);

				// Fetch Rating
				var ratingDto = await _ratingCachedService.GetRatingByName(createCaseModel.RagRating);

				// Is first case
				var isCasePrimary = await _caseCachedService.IsCasePrimary(createCaseModel.CreatedBy);
				
				// Create a case
				createCaseModel.Status = statusDto.Urn;
				var newCase = await _caseCachedService.PostCase(CaseMapping.Map(createCaseModel));

				// Create a record
				var newRecord = await _recordService.PostRecordByCaseUrn(RecordMapping.Map(typeDto, newCase.Urn, ratingDto.Urn, statusDto.Urn, isCasePrimary));

				// Create a rating history
				var createRecordRatingHistoryDto = new RecordRatingHistoryDto(DateTimeOffset.Now, newRecord.Urn, ratingDto.Urn);
				await _recordRatingHistoryCachedService.PostRecordRatingHistory(createRecordRatingHistoryDto, createCaseModel.CreatedBy, newCase.Urn);

				// Return case model
				return _mapper.Map<CaseModel>(newCase);
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseModelService::PostCase exception {ex.Message}");

				throw;
			}
		}
		
		private async Task<(IList<HomeModel>, IList<HomeModel>)> FetchFromTramsApi(string caseworker)
		{
			// Get from TRAMS API all trusts for the caseworker
			var caseStatus = new List<string> { Status.Live.ToString(), Status.Monitoring.ToString() };
			var tasksCasesDto = caseStatus.Select(status => _caseCachedService.GetCasesByCaseworker(caseworker, status));
			
			var resultTasksCasesDto = await Task.WhenAll(tasksCasesDto);
			foreach (IList<CaseDto> casesDto in resultTasksCasesDto)
			{
				// If cases available, fetch trust by ukprn data.
				if (!casesDto.Any()) continue;
				
				// Fetch trusts by ukprn
				var trustsDetailsTasks = casesDto.Where(c => c.TrustUkPrn != null)
					.Select(c => _trustCachedService.GetTrustByUkPrn(c.TrustUkPrn)).ToList();
				await Task.WhenAll(trustsDetailsTasks);
				// Get results from tasks
				var trustsDetailsDto = trustsDetailsTasks.Select(trustDetailsTask => trustDetailsTask.Result);

				// Fetch records by case urn
				var recordsTasks = casesDto.Select(c => _recordService.GetRecordsByCaseUrn(c.Urn)).ToList();
				await Task.WhenAll(recordsTasks);
				// Get results from tasks
				var recordsDto = recordsTasks.Select(recordTask => recordTask.Result);
								
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

		private async Task<(IList<HomeModel>, IList<HomeModel>)> FetchFromCache(UserState userState)
		{
			// Get cases
			var caseDetails = userState.CasesDetails;
					
			// Get cases from cache
			var casesDto = caseDetails.Select(c => c.Value.CaseDto);
					
			// Fetch trusts by ukprn
			var trustsDetailsTasks = casesDto.Where(c => c.TrustUkPrn != null)
				.Select(c => _trustCachedService.GetTrustByUkPrn(c.TrustUkPrn)).ToList();
			await Task.WhenAll(trustsDetailsTasks);
			// Get results from tasks
			var trustsDetailsDto = trustsDetailsTasks.Select(trustDetailsTask => trustDetailsTask.Result);

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