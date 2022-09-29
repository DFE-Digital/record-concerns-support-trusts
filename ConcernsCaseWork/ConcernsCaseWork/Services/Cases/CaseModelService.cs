using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Ratings;
using Service.Redis.Records;
using Service.Redis.Status;
using Service.Redis.Trusts;
using Service.Redis.Types;
using Service.TRAMS.Cases;
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
			IStatusCachedService statusCachedService,
			ICaseSearchService caseSearchService,
			ICaseHistoryCachedService caseHistoryCachedService,
			ILogger<CaseModelService> logger)
		{
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
				var casesDto = await _caseCachedService.GetCasesByCaseworkerAndStatus(caseworker, statusDto.Id);

				// Return if cases are empty
				if (!casesDto.Any()) return Array.Empty<HomeModel>();
				
				// Fetch rag rating
				var ratingsDto = await _ratingCachedService.GetRatings();
									
				// Fetch types
				var typesDto = await _typeCachedService.GetTypes();
				
				// Fetch statuses
				var statusesDto = await _statusCachedService.GetStatuses();
				
				// Execute in parallel each case contained logic
				var listHomeModel = casesDto.Select(caseDto =>
				{
					var trustDetailsTask = _trustCachedService.GetTrustByUkPrn(caseDto.TrustUkPrn);
					var recordsTask = _recordCachedService.GetRecordsByCaseUrn(caseDto.CreatedBy, caseDto.Urn);

					Task.WaitAll(trustDetailsTask, recordsTask);

					var trustDetailsDto = trustDetailsTask.Result;
					var recordsDto = recordsTask.Result;

					// Map records dto to model
					var recordsModel = RecordMapping.MapDtoToModel(recordsDto, typesDto, ratingsDto, statusesDto);
					
					// Case rating
					var caseRatingModel = RatingMapping.MapDtoToModel(ratingsDto, caseDto.RatingId);
					var trustName = TrustMapping.FetchTrustName(trustDetailsDto);
					
					return new HomeModel(
						caseDto.Urn.ToString(), 
						caseDto.CreatedAt,
						caseDto.UpdatedAt,
						caseDto.ClosedAt,
						caseDto.ReviewAt,
						caseDto.CreatedBy,
						trustName,
						caseRatingModel,
						recordsModel
					);
					
				}).ToList();
				
				return statusEnum switch
				{
					StatusEnum.Live => listHomeModel.OrderByDescending(homeModel => homeModel.UpdatedUnixTime).ToList(),
					StatusEnum.Monitoring => listHomeModel.OrderBy(homeModel => homeModel.ReviewUnixTime).ToList(),
					StatusEnum.Close => listHomeModel.OrderByDescending(homeModel => homeModel.ClosedUnixTime).ToList(),
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

				// Fetch statuses
				var statuses = await _statusCachedService.GetStatuses();
				
				// Fetch monitoring status
				var monitoringStatus = await _statusCachedService.GetStatusByName(StatusEnum.Monitoring.ToString());

				// Filter cases that are for monitoring
				casesDto = casesDto.Where(c => c.StatusId.CompareTo(monitoringStatus.Id) != 0).ToList();
				
				// Fetch records by case urn
				var recordsTasks = casesDto.Select(c => _recordCachedService.GetRecordsByCaseUrn(c.CreatedBy, c.Urn)).ToList();
				await Task.WhenAll(recordsTasks);
			
				// Get results from tasks
				var recordsDto = recordsTasks.SelectMany(recordTask => recordTask.Result).ToList();
				if (!recordsDto.Any()) return Array.Empty<TrustCasesModel>();
				
				// Fetch Ratings
				var ratingsDto = await _ratingCachedService.GetRatings();

				// Fetch Types
				var typesDto = await _typeCachedService.GetTypes();

				return CaseMapping.MapTrustCases(recordsDto, ratingsDto, typesDto, casesDto, statuses);
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
				
				caseDto = CaseMapping.MapClosure(patchCaseModel, caseDto, statusDto);
				
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
		
		public async Task PatchCaseRating(PatchCaseModel patchCaseModel)
		{
			try
			{
				// Fetch Rating
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);

				caseDto = CaseMapping.MapRating(patchCaseModel, caseDto);
				
				await _caseCachedService.PatchCaseByUrn(caseDto);

				// Create case history event
				await _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.RiskRating, patchCaseModel.Urn), patchCaseModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchCaseRating exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchRecordRating(PatchRecordModel patchRecordModel)
		{
			try
			{
				// Fetch Records
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(patchRecordModel.CreatedBy, patchRecordModel.CaseUrn);

				var recordDto = recordsDto.FirstOrDefault(r => r.Id.CompareTo(patchRecordModel.Id) == 0);
				recordDto = RecordMapping.MapRating(patchRecordModel, recordDto);

				await _recordCachedService.PatchRecordById(recordDto, patchRecordModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchRecordRating exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchDirectionOfTravel(PatchCaseModel patchCaseModel)
		{
			try
			{
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				
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
				createCaseModel.StatusId = statusDto.Id;
				var newCase = await _caseCachedService.PostCase(CaseMapping.Map(createCaseModel));

				// Create records
				var currentDate = DateTimeOffset.Now;
				var recordTasks = createCaseModel.CreateRecordsModel.Select(recordModel => 
				{
					var createRecordDto = new CreateRecordDto(
						currentDate, 
						currentDate, 
						currentDate, 
						currentDate, 
						recordModel.Type, 
						recordModel.SubType, 
						recordModel.TypeDisplay, 
						newCase.Urn, 
						recordModel.TypeId, 
						recordModel.RatingId, 
						statusDto.Id,
						recordModel.MeansOfReferralId);
					
					return _recordCachedService.PostRecordByCaseUrn(createRecordDto, createCaseModel.CreatedBy);
				});

				await Task.WhenAll(recordTasks);
				
				// Create concern history event
				var concernEventTask = _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.Concern, newCase.Urn), newCase.CreatedBy);

				// Create case history event
				var caseEventTask = _caseHistoryCachedService.PostCaseHistory(CaseHistoryMapping.BuildCaseHistoryDto(CaseHistoryEnum.Case, newCase.Urn), newCase.CreatedBy);

				Task.WaitAll(concernEventTask, caseEventTask);
				
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