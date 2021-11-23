using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Rating;
using Service.Redis.RecordRatingHistory;
using Service.Redis.Records;
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
		private readonly ICaseHistoryCachedService _caseHistoryCachedService;
		private readonly IRatingCachedService _ratingCachedService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly IRecordCachedService _recordCachedService;
		private readonly ITrustCachedService _trustCachedService;
		private readonly ICaseCachedService _caseCachedService;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ICaseSearchService _caseSearchService;
		private readonly ILogger<CaseModelService> _logger;
		private readonly ICachedService _cachedService;

		public CaseModelService(ICaseCachedService caseCachedService, 
			ITrustCachedService trustCachedService, 
			IRecordCachedService recordCachedService, 
			IRatingCachedService ratingCachedService,
			ITypeCachedService typeCachedService, 
			ICachedService cachedService, 
			IRecordRatingHistoryCachedService recordRatingHistoryCachedService,
			IStatusCachedService statusCachedService,
			ICaseSearchService caseSearchService,
			ICaseHistoryCachedService caseHistoryCachedService,
			ILogger<CaseModelService> logger)
		{
			_recordRatingHistoryCachedService = recordRatingHistoryCachedService;
			_caseHistoryCachedService = caseHistoryCachedService;
			_statusCachedService = statusCachedService;
			_ratingCachedService = ratingCachedService;
			_recordCachedService = recordCachedService;
			_trustCachedService = trustCachedService;
			_caseCachedService = caseCachedService;
			_typeCachedService = typeCachedService;
			_caseSearchService = caseSearchService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task<IList<HomeModel>> GetCasesByCaseworkerAndStatus(string caseworker, StatusEnum status)
		{
			try
			{
				// Find cases redis cache
				var caseState = await _cachedService.GetData<UserState>(caseworker);
				if (caseState != null)
				{
					return await FetchFromCache(caseState, status);
				}
				
				// Find cases Academies Api
				return await FetchFromTramsApi(caseworker, status);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::GetCasesByCaseworker exception {Message}", ex.Message);
			}

			return Array.Empty<HomeModel>();
		}

		public async Task<CaseModel> GetCaseByUrn(string caseworker, long urn)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(caseworker, urn);
				var caseModel = CaseMapping.Map(caseDto);
				
				return caseModel;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::GetCaseByUrn exception {Message}", ex.Message);
				
				throw;
			}
		}

		/// <summary>
		/// Use case get all cases by trust ukprn
		/// Trust overview scenario where service displays open and close cases by trust
		/// </summary>
		/// <param name="trustUkprn"></param>
		/// <returns></returns>
		public async Task<IList<TrustCasesModel>> GetCasesByTrustUkprn(string trustUkprn)
		{
			try
			{
				var casesDto = await _caseSearchService.GetCasesByCaseTrustSearch(new CaseTrustSearch(trustUkprn));
				if (!casesDto.Any()) return Array.Empty<TrustCasesModel>();
				
				// Fetch live and close status
				var liveStatus = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
				var monitoringStatus = await _statusCachedService.GetStatusByName(StatusEnum.Monitoring.ToString());
				var closeStatus = await _statusCachedService.GetStatusByName(StatusEnum.Close.ToString());
				
				// Filter cases that are for monitoring
				casesDto = casesDto.Where(c => c.StatusUrn.CompareTo(monitoringStatus.Urn) != 0).ToList();
				
				// Fetch records by case urn
				var recordsTasks = casesDto.Select(c => _recordCachedService.GetRecordsByCaseUrn(c.CreatedBy, c.Urn)).ToList();
				await Task.WhenAll(recordsTasks);
			
				// Get results from tasks and filter only primary records
				var recordsDto = recordsTasks.SelectMany(recordTask => recordTask.Result).ToList();
				
				// Filter primary records
				recordsDto = recordsDto.Where(r => r.Primary).ToList();
				if (!recordsDto.Any()) return Array.Empty<TrustCasesModel>();
				
				// Fetch Ratings
				var ragsRatingDto = await _ratingCachedService.GetRatings();

				// Fetch Types
				var typesDto = await _typeCachedService.GetTypes();

				return CaseMapping.MapTrustCases(recordsDto, ragsRatingDto, typesDto, casesDto, liveStatus, closeStatus);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::GetCasesByTrustUkprn exception {Message}", ex.Message);
				
				throw;
			}
		}

		public async Task PatchClosure(PatchCaseModel patchCaseModel)
		{
			try
			{
				// Fetch Status
				var statusDto = await _statusCachedService.GetStatusByName(patchCaseModel.StatusName);
				
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
				var recordDto = recordsDto.First(r => r.Primary);
				
				// Patch source dtos
				recordDto = RecordMapping.MapClosure(patchCaseModel, recordDto, statusDto);
				caseDto = CaseMapping.MapClosure(patchCaseModel, caseDto, statusDto);
				
				await _recordCachedService.PatchRecordByUrn(recordDto, patchCaseModel.CreatedBy);
				await _caseCachedService.PatchCaseByUrn(caseDto);
				
				// Create case history event
				var caseHistoryEnum = patchCaseModel.StatusName.Equals(StatusEnum.Monitoring.ToString(), StringComparison.OrdinalIgnoreCase)
					? CaseHistoryEnum.ClosedForMonitoring
					: CaseHistoryEnum.Closed;
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(caseHistoryEnum, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchClosure exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchConcernType(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
				var recordDto = recordsDto.First(r => r.Primary);
				
				// Patch source dtos
				recordDto = RecordMapping.MapConcernType(patchCaseModel, recordDto);
				caseDto = CaseMapping.Map(patchCaseModel, caseDto);
				
				await _recordCachedService.PatchRecordByUrn(recordDto, patchCaseModel.CreatedBy);
				await _caseCachedService.PatchCaseByUrn(caseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchConcernType exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchRiskRating(PatchCaseModel patchCaseModel)
		{
			try
			{
				// Fetch Rating
				var ratingDto = await _ratingCachedService.GetRatingByName(patchCaseModel.RatingName);
				patchCaseModel.RatingUrn = ratingDto.Urn;
				
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
				var recordDto = recordsDto.First(r => r.Primary);
				
				// Patch source dtos
				recordDto = RecordMapping.MapRiskRating(patchCaseModel, recordDto);
				caseDto = CaseMapping.Map(patchCaseModel, caseDto);

				var timeNow = DateTimeOffset.Now;
				var recordRatingHistory = new RecordRatingHistoryDto(timeNow, recordDto.Urn, ratingDto.Urn);
				
				await _recordCachedService.PatchRecordByUrn(recordDto, patchCaseModel.CreatedBy);
				await _caseCachedService.PatchCaseByUrn(caseDto);
				await _recordRatingHistoryCachedService.PostRecordRatingHistory(recordRatingHistory, patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.RiskRating, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchRiskRating exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchDirectionOfTravel(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapDirectionOfTravel(patchCaseModel, caseDto);

				await _caseCachedService.PatchCaseByUrn(caseDto);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.DirectionOfTravel, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchDirectionOfTravel exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchIssue(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapIssue(patchCaseModel, caseDto);

				await _caseCachedService.PatchCaseByUrn(caseDto);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.Issue, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchIssue exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchCaseAim(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapCaseAim(patchCaseModel, caseDto);

				await _caseCachedService.PatchCaseByUrn(caseDto);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.CaseAim, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchCaseAim exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchCurrentStatus(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapCurrentStatus(patchCaseModel, caseDto);

				await _caseCachedService.PatchCaseByUrn(caseDto);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.CurrentStatus, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchCurrentStatus exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchDeEscalationPoint(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapDeEscalationPoint(patchCaseModel, caseDto);

				await _caseCachedService.PatchCaseByUrn(caseDto);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.DeEscalationPoint, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchDeEscalationPoint exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchNextSteps(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);

				// Patch source dtos
				caseDto = CaseMapping.MapNextSteps(patchCaseModel, caseDto);

				await _caseCachedService.PatchCaseByUrn(caseDto);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.NextSteps, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchNextSteps exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task<long> PostCase(CreateCaseModel createCaseModel)
		{
			try
			{
				// Fetch Status
				var statusDto = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
				
				// In a 1:1 case -> record (not multiple concerns) this flag is always true.
				// When multiple concerns is develop take into consideration the number of records attached to the case.
				const bool isCasePrimary = true;
				
				// Create a case
				createCaseModel.StatusUrn = statusDto.Urn;
				var newCase = await _caseCachedService.PostCase(CaseMapping.Map(createCaseModel));

				// Create a record
				var currentDate = DateTimeOffset.Now;
				var createRecordDto = new CreateRecordDto(currentDate, currentDate, currentDate, 
					currentDate, createCaseModel.Type, createCaseModel.SubType, createCaseModel.TypeDisplay, newCase.Urn, 
					createCaseModel.TypeUrn, createCaseModel.RagRatingUrn, isCasePrimary, statusDto.Urn);
				var newRecord = await _recordCachedService.PostRecordByCaseUrn(createRecordDto, createCaseModel.CreatedBy);
				
				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.Concern, newCase.Urn), newCase.CreatedBy);
				
				// Create a rating history
				var createRecordRatingHistoryDto = new RecordRatingHistoryDto(DateTimeOffset.Now, newRecord.Urn, createCaseModel.RagRatingUrn);
				await _recordRatingHistoryCachedService.PostRecordRatingHistory(createRecordRatingHistoryDto, createCaseModel.CreatedBy, newCase.Urn);

				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.Case, newCase.Urn), newCase.CreatedBy);
				
				// Return case urn, if required return type can be changed to CaseModel.
				return newCase.Urn;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PostCase exception {Message}", ex.Message);

				throw;
			}
		}
		
		private async Task<IList<HomeModel>> FetchFromTramsApi(string caseworker, StatusEnum status)
		{
			// Fetch Status
			var statusDto = await _statusCachedService.GetStatusByName(status.ToString());

			// Get from TRAMS API all trusts for the caseworker
			var casesDto = await _caseCachedService.GetCasesByCaseworkerAndStatus(caseworker, statusDto.Urn);

			if (!casesDto.Any()) return Array.Empty<HomeModel>();
			
			var trustsDetailsTasks = casesDto.Where(c => c.TrustUkPrn != null)
				.Select(c => _trustCachedService.GetTrustByUkPrn(c.TrustUkPrn)).ToList();
			await Task.WhenAll(trustsDetailsTasks);
			
			// Get results from tasks
			var trustsDetailsDto = trustsDetailsTasks.Select(trustDetailsTask => trustDetailsTask.Result)
				.ToList();

			// Fetch records by case urn
			var recordsTasks = casesDto.Select(c => _recordCachedService.GetRecordsByCaseUrn(c.CreatedBy, c.Urn)).ToList();
			await Task.WhenAll(recordsTasks);
			
			// Get results from tasks
			var recordsDto = recordsTasks.SelectMany(recordTask => recordTask.Result).ToList();
								
			// Fetch rag rating
			var ragsRatingDto = await _ratingCachedService.GetRatings();
								
			// Fetch types
			var typesDto = await _typeCachedService.GetTypes();
				
			return HomeMapping.Map(casesDto, trustsDetailsDto, recordsDto, ragsRatingDto, typesDto, statusDto);
		}

		private async Task<IList<HomeModel>> FetchFromCache(UserState userState, StatusEnum status)
		{
			// Fetch Status
			var statusDto = await _statusCachedService.GetStatusByName(status.ToString());

			// Fetch cases
			var caseDetails = userState.CasesDetails;
					
			// Fetch cases from cache
			var casesDto = caseDetails.Select(c => c.Value.CaseDto)
				.Where(c => c.StatusUrn.CompareTo(statusDto.Urn) == 0).ToList();
					
			if (!casesDto.Any()) return Array.Empty<HomeModel>();
			
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
			
			return HomeMapping.Map(casesDto, trustsDetailsDto, recordsDto, ragsRatingDto, typesDto, statusDto);
		}
	}
}