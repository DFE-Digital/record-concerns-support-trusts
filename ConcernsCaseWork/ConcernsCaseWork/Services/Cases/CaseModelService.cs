using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Ratings;
using Service.Redis.RecordRatingHistory;
using Service.Redis.Records;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Types;
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

		public CaseModelService(ICaseCachedService caseCachedService, 
			ITrustCachedService trustCachedService, 
			IRecordCachedService recordCachedService, 
			IRatingCachedService ratingCachedService,
			ITypeCachedService typeCachedService,
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
			_logger = logger;
		}
		
		public async Task<IList<HomeModel>> GetCasesByCaseworkerAndStatus(string caseworker, StatusEnum statusEnum)
		{
			try
			{
				_logger.LogInformation("CaseModelService::GetCasesByCaseworkerAndStatus {Caseworker} - {StatusEnum}", caseworker, statusEnum);
				
				// Fetch Status
				var statusDto = await _statusCachedService.GetStatusByName(statusEnum.ToString());

				// Get from Academies API all cases by caseworker and status
				var casesDto = await _caseCachedService.GetCasesByCaseworkerAndStatus(caseworker, statusDto.Urn);

				// Return if cases are empty
				if (!casesDto.Any()) return Array.Empty<HomeModel>();
				
				// Fetch rag rating
				var ratingsDto = await _ratingCachedService.GetRatings();
									
				// Fetch types
				var typesDto = await _typeCachedService.GetTypes();
				
				// Execute in parallel each case contained logic
				var listHomeModelTasks = casesDto.Select(async caseDto =>
				{
					var trustDetailsDto = await _trustCachedService.GetTrustByUkPrn(caseDto.TrustUkPrn);
					var records = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
					
					// Get primary record
					var primaryRecordDto = records.FirstOrDefault(recordDto => recordDto.Primary);
					if (primaryRecordDto is null) return null;

					// Find primary type
					var primaryCaseType = typesDto.FirstOrDefault(t => t.Urn.CompareTo(primaryRecordDto.TypeUrn) == 0);
					if (primaryCaseType is null) return null;
				
					// Rag rating
					var ratingName = ratingsDto.Where(r => r.Urn.CompareTo(primaryRecordDto.RatingUrn) == 0)
						.Select(r => r.Name)
						.First();

					var rag = RatingMapping.FetchRag(ratingName);
					var ragCss = RatingMapping.FetchRagCss(ratingName);

					var trustName = TrustMapping.FetchTrustName(trustDetailsDto);
					var academies = TrustMapping.FetchAcademies(trustDetailsDto);
					
					return new HomeModel(
						caseDto.Urn.ToString(), 
						caseDto.CreatedAt,
						caseDto.UpdatedAt,
						caseDto.ClosedAt,
						caseDto.ReviewAt,
						trustName,
						academies,
						primaryCaseType.Name,
						primaryCaseType.Description,
						rag,
						ragCss);
				}).ToList();
				
				await Task.WhenAll(listHomeModelTasks);
				var listHomeModel = listHomeModelTasks.Select(homeModelTask => homeModelTask.Result).Where(homeModel => homeModel != null).ToList();

				return statusEnum switch
				{
					StatusEnum.Live => listHomeModel.OrderByDescending(homeModel => homeModel.Updated).ToList(),
					StatusEnum.Monitoring => listHomeModel.OrderBy(homeModel => homeModel.Review).ToList(),
					StatusEnum.Close => listHomeModel.OrderByDescending(homeModel => homeModel.Closed).ToList(),
					_ => throw new ArgumentOutOfRangeException(nameof(statusEnum), statusEnum, $"Invalid status enum {statusEnum}")
				};
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::GetCasesByCaseworkerAndStatus exception {Message}", ex.Message);
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
			
				// Get results from tasks
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
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
				var recordDto = recordsDto.First(r => r.Primary);
				
				// Patch source dtos
				recordDto = RecordMapping.MapRiskRating(patchCaseModel, recordDto);
				caseDto = CaseMapping.Map(patchCaseModel, caseDto);

				var timeNow = DateTimeOffset.Now;
				var recordRatingHistory = new RecordRatingHistoryDto(timeNow, recordDto.Urn, patchCaseModel.RatingUrn);
				
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
				
				// Create a case
				createCaseModel.StatusUrn = statusDto.Urn;
				var newCase = await _caseCachedService.PostCase(CaseMapping.Map(createCaseModel));


				var currentDate = DateTimeOffset.Now;
				//TODOEA resolve type urn
				// create records
				createCaseModel.CreateRecordsModel.Select(async r =>
				{
					//var createRecordDto = new CreateRecordDto(
					//	currentDate, 
					//	currentDate, 
					//	currentDate,
					//	currentDate, 
					//	r.TypeModel.CheckedType, 
					//	r.TypeModel.CheckedSubType, 
					//	r.TypeModel.TypeDisplay, 
					//	newCase.Urn,
					//	100, 
					//	r.RatingModel.Urn, 
					//	statusDto.Urn);

					//var newRecord = await _recordCachedService.PostRecordByCaseUrn(createRecordDto, createCaseModel.CreatedBy);

					//// Create a rating history
					//var createRecordRatingHistoryDto = new RecordRatingHistoryDto(currentDate, newRecord.Urn, r.RatingModel.Urn);
					//await _recordRatingHistoryCachedService.PostRecordRatingHistory(createRecordRatingHistoryDto, createCaseModel.CreatedBy, newCase.Urn);
				}
				);

				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.Concern, newCase.Urn), newCase.CreatedBy);

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
	}
}