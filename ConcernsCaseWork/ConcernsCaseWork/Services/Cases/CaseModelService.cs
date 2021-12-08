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

		public async Task<IList<HomeModel>> GetCasesByCaseworkerAndStatus(IList<string> caseworkers, StatusEnum statusEnum)
		{
			try
			{
				var allCaseworkerCasesTasks = caseworkers.Select(caseworker => GetCasesByCaseworkerAndStatus(caseworker, statusEnum)).ToList();
				await Task.WhenAll(allCaseworkerCasesTasks);
				
				return allCaseworkerCasesTasks.SelectMany(homeModelTask => homeModelTask.Result).ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::GetCasesByCaseworkerAndStatus exception {Message}", ex.Message);
			}

			return Array.Empty<HomeModel>();
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
				var listHomeModel = casesDto.Select(caseDto =>
				{
					var trustDetailsTask = _trustCachedService.GetTrustByUkPrn(caseDto.TrustUkPrn);
					var recordsTask = _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);

					Task.WaitAll(trustDetailsTask, recordsTask);

					var trustDetailsDto = trustDetailsTask.Result;
					var records = recordsTask.Result;
					
					// Get first record
					var firstRecordDto = records.FirstOrDefault();
					if (firstRecordDto is null) return null;

					// Find primary type
					var primaryCaseType = typesDto.FirstOrDefault(t => t.Urn.CompareTo(firstRecordDto.TypeUrn) == 0);
					if (primaryCaseType is null) return null;
				
					// Rag rating
					var ratingName = ratingsDto.Where(r => r.Urn.CompareTo(firstRecordDto.RatingUrn) == 0)
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
						caseDto.CreatedBy,
						trustName,
						academies,
						primaryCaseType.Name,
						primaryCaseType.Description,
						rag,
						ragCss);
					
				}).Where(homeModel => homeModel != null).ToList();
				
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
				var statusDto = await _statusCachedService.GetStatusByName(patchCaseModel.StatusName);
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
				
				// TODO multiple concerns will need some refactor
				var recordDto = recordsDto.FirstOrDefault();
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
				
				// TODO multiple concerns will need some refactor
				var recordDto = recordsDto.FirstOrDefault();
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);
				
				// TODO multiple concerns will need some refactor
				var recordDto = recordsDto.FirstOrDefault();
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);
				
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
				// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
				caseDto = await RecoverCaseRating(caseDto);

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
				
				// TODO Multiple Concerns
				// When multiple concerns is develop take into consideration the number of records attached to the case.

				// Create a case
				createCaseModel.StatusUrn = statusDto.Urn;
				var newCase = await _caseCachedService.PostCase(CaseMapping.Map(createCaseModel));

				// Create a record
				var currentDate = DateTimeOffset.Now;
				var createRecordDto = new CreateRecordDto(currentDate, currentDate, currentDate, 
					currentDate, createCaseModel.Type, createCaseModel.SubType, createCaseModel.TypeDisplay, newCase.Urn, 
					createCaseModel.TypeUrn, createCaseModel.RagRatingUrn, statusDto.Urn);
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

		private async Task<CaseDto> RecoverCaseRating(CaseDto caseDto)
		{
			// TODO multiple concerns work temporary fix for existing cases that don't have rating urn
			if (caseDto.RatingUrn != 0) return caseDto;
			
			// Fetch Ratings
			var defaultRating = await _ratingCachedService.GetDefaultRating();
			caseDto.RatingUrn = defaultRating.Urn;

			return caseDto;
		}
	}
}