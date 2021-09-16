using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Rating;
using Service.Redis.RecordRatingHistory;
using Service.Redis.Records;
using Service.Redis.Sequence;
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
		private readonly ISequenceCachedService _sequenceCachedService;
		private readonly IRatingCachedService _ratingCachedService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly IRecordCachedService _recordCachedService;
		private readonly ITrustCachedService _trustCachedService;
		private readonly ICaseCachedService _caseCachedService;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ILogger<CaseModelService> _logger;
		private readonly ICachedService _cachedService;
		
		public CaseModelService(ICaseCachedService caseCachedService, ITrustCachedService trustCachedService, 
			IRecordCachedService recordCachedService, IRatingCachedService ratingCachedService,
			ITypeCachedService typeCachedService, ICachedService cachedService, 
			IRecordRatingHistoryCachedService recordRatingHistoryCachedService,
			IStatusCachedService statusCachedService, 
			ISequenceCachedService sequenceCachedService,
			ILogger<CaseModelService> logger)
		{
			_recordRatingHistoryCachedService = recordRatingHistoryCachedService;
			_sequenceCachedService = sequenceCachedService;
			_statusCachedService = statusCachedService;
			_ratingCachedService = ratingCachedService;
			_recordCachedService = recordCachedService;
			_trustCachedService = trustCachedService;
			_caseCachedService = caseCachedService;
			_typeCachedService = typeCachedService;
			_cachedService = cachedService;
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
				// TODO Start Remove when Trams API is live
				createCaseModel.Urn = await _sequenceCachedService.Generator();
				// TODO End Remove when Trams API is live
				
				// Fetch Status
				var statusDto = await _statusCachedService.GetStatusByName(Status.Live.ToString());

				// Fetch Type
				var typeDto = await _typeCachedService.GetTypeByNameAndDescription(
					createCaseModel.RecordType, createCaseModel.RecordSubType);

				// Fetch Rating
				var ratingDto = await _ratingCachedService.GetRatingByName(createCaseModel.RagRating);

				// Is first case
				var isCasePrimary = await _caseCachedService.IsCasePrimary(createCaseModel.CreatedBy, createCaseModel.Urn);
				
				// Create a case
				createCaseModel.Status = statusDto.Urn;
				var newCase = await _caseCachedService.PostCase(CaseMapping.Map(createCaseModel));

				// Create a record
				var currentDate = DateTimeOffset.Now;
				var createRecordDto = new CreateRecordDto(currentDate, currentDate, currentDate, 
					currentDate, typeDto.Name, typeDto.Description, createCaseModel.Description, newCase.Urn, 
					typeDto.Urn, ratingDto.Urn, isCasePrimary, await _sequenceCachedService.Generator(), statusDto.Urn);
				
				var newRecord = await _recordCachedService.PostRecordByCaseUrn(createRecordDto, createCaseModel.CreatedBy);

				// Create a rating history
				var createRecordRatingHistoryDto = new RecordRatingHistoryDto(DateTimeOffset.Now, newRecord.Urn, ratingDto.Urn);
				await _recordRatingHistoryCachedService.PostRecordRatingHistory(createRecordRatingHistoryDto, createCaseModel.CreatedBy, newCase.Urn);

				// Return case model
				return CaseMapping.Map(newCase, statusDto.Name);
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseModelService::PostCase exception {ex.Message}");

				throw;
			}
		}
		
		private async Task<(IList<HomeModel>, IList<HomeModel>)> FetchFromTramsApi(string caseworker)
		{
			// Fetch Status
			var statusLiveDto = await _statusCachedService.GetStatusByName(Status.Live.ToString());
			var statusMonitoringDto = await _statusCachedService.GetStatusByName(Status.Monitoring.ToString());
			
			// Get from TRAMS API all trusts for the caseworker
			var caseStatus = new List<string> { statusLiveDto.Name, statusMonitoringDto.Name };
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
				var trustsDetailsDto = trustsDetailsTasks.Select(trustDetailsTask => trustDetailsTask.Result)
					.ToList();

				// Fetch records by case urn
				var recordsTasks = casesDto.Select(c => _recordCachedService.GetRecordsByCaseUrn(c)).ToList();
				await Task.WhenAll(recordsTasks);
				// Get results from tasks
				var recordsDto = recordsTasks.SelectMany(recordTask => recordTask.Result).ToList();
								
				// Fetch rag rating
				var ragsRatingDto = await _ratingCachedService.GetRatings();
								
				// Fetch types
				var typesDto = await _typeCachedService.GetTypes();
				
				return HomeMapping.Map(casesDto, trustsDetailsDto, recordsDto, ragsRatingDto, typesDto, statusLiveDto, statusMonitoringDto);
			}
			
			return Empty();
		}

		private async Task<(IList<HomeModel>, IList<HomeModel>)> FetchFromCache(UserState userState)
		{
			// Fetch Status
			var statusLiveDto = await _statusCachedService.GetStatusByName(Status.Live.ToString());
			var statusMonitoringDto = await _statusCachedService.GetStatusByName(Status.Monitoring.ToString());
			
			// Fetch cases
			var caseDetails = userState.CasesDetails;
					
			// Fetch cases from cache
			var casesDto = caseDetails.Select(c => c.Value.CaseDto).ToList();
					
			// Fetch trusts by ukprn
			var trustsDetailsTasks = casesDto.Where(c => c.TrustUkPrn != null)
				.Select(c => _trustCachedService.GetTrustByUkPrn(c.TrustUkPrn)).ToList();
			var trustsDetailsTasksResults = await Task.WhenAll(trustsDetailsTasks);
			
			// Fetch results from tasks
			var trustsDetailsDto = trustsDetailsTasksResults.ToList();

			// Fetch records by case urn
			var recordsDto = caseDetails.SelectMany(c => c.Value.Records.Values.Select(r => r.RecordDto))
				.ToList();
					
			// Fetch rag rating
			var ragsRatingDto = await _ratingCachedService.GetRatings();
						
			// Fetch types
			var typesDto = await _typeCachedService.GetTypes();
			
			return HomeMapping.Map(casesDto, trustsDetailsDto, recordsDto, ragsRatingDto, typesDto, statusLiveDto, statusMonitoringDto);
		}

		private static (IList<HomeModel>, IList<HomeModel>) Empty()
		{
			var emptyResults = Array.Empty<HomeModel>();
			return (emptyResults, emptyResults);
		}
	}
}