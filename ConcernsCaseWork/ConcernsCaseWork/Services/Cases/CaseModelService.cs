﻿using AutoMapper;
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
using Service.TRAMS.Trusts;
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
		private readonly ICaseService _caseService;
		private readonly IMapper _mapper;

		public CaseModelService(ICaseCachedService caseCachedService, ITrustCachedService trustCachedService, 
			IRecordCachedService recordCachedService, IRatingCachedService ratingCachedService,
			ITypeCachedService typeCachedService, ICachedService cachedService, 
			IRecordRatingHistoryCachedService recordRatingHistoryCachedService,
			IStatusCachedService statusCachedService, 
			ISequenceCachedService sequenceCachedService,
			ICaseService caseService,
			IMapper mapper,
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
			_caseService = caseService;
			_mapper = mapper;
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
				
				// Find cases TramsApi
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

				// Fetch Trust
				var trustDetailsDto = await _trustCachedService.GetTrustByUkPrn(caseModel.TrustUkPrn);
				caseModel.TrustDetailsModel = _mapper.Map<TrustDetailsDto, TrustDetailsModel>(trustDetailsDto);

				// Fetch records
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto);
				var recordDto = recordsDto.FirstOrDefault(r => r.Primary) ?? recordsDto.First();
				
				// Fetch type
				var typesDto = await _typeCachedService.GetTypes();
				var typeDto = typesDto.First(t => t.Urn.CompareTo(recordDto.TypeUrn) == 0);
				
				// Map case model
				caseModel.CaseType = typeDto.Name;
				caseModel.CaseSubType = typeDto.Description;
				
				// Fetch Ratings
				var ragsRatingDto = await _ratingCachedService.GetRatings();
				var ragRating = ragsRatingDto.First(r => r.Urn.CompareTo(recordDto.RatingUrn) == 0);
				caseModel.RagRating = RagMapping.FetchRag(ragRating.Name);
				caseModel.RagRatingCss = RagMapping.FetchRagCss(ragRating.Name);
				caseModel.RagRatingName = ragRating.Name;
				
				return caseModel;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::GetCaseByUrn exception {Message}", ex.Message);
				throw;
			}
		}

		public async Task<IList<CaseModel>> GetCasesByTrustUkprn(string trustUkprn)
		{
			try
			{
				var casesDto = await _caseService.GetCasesByTrustUkPrn(trustUkprn);
				var casesModel = casesDto.Select(caseDto => CaseMapping.Map(caseDto)).ToList();
				
				return casesModel;
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
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto);
				var recordDto = recordsDto.First(r => r.Primary);
				
				// Patch source dtos
				recordDto = RecordMapping.MapClosure(patchCaseModel, recordDto, statusDto);
				caseDto = CaseMapping.MapClosure(patchCaseModel, caseDto, statusDto);
				
				await _recordCachedService.PatchRecordByUrn(recordDto, patchCaseModel.CreatedBy);
				await _caseCachedService.PatchCaseByUrn(caseDto);
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
				// Fetch Type
				var typeDto = await _typeCachedService.GetTypeByNameAndDescription(
					patchCaseModel.CaseType, patchCaseModel.CaseSubType);
				patchCaseModel.TypeUrn = typeDto.Urn;
				
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto);
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
				var ratingDto = await _ratingCachedService.GetRatingByName(patchCaseModel.RiskRating);
				patchCaseModel.RatingUrn = ratingDto.Urn;
				
				var caseDto = await _caseCachedService.GetCaseByUrn(patchCaseModel.CreatedBy, patchCaseModel.Urn);
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseDto);
				var recordDto = recordsDto.First(r => r.Primary);
				
				// Patch source dtos
				recordDto = RecordMapping.MapRiskRating(patchCaseModel, recordDto);
				caseDto = CaseMapping.Map(patchCaseModel, caseDto);

				var recordRatingHistory = new RecordRatingHistoryDto(DateTimeOffset.Now, recordDto.Urn, ratingDto.Urn);
				
				await _recordCachedService.PatchRecordByUrn(recordDto, patchCaseModel.CreatedBy);
				await _caseCachedService.PatchCaseByUrn(caseDto);
				await _recordRatingHistoryCachedService.PostRecordRatingHistory(recordRatingHistory, patchCaseModel.CreatedBy, patchCaseModel.Urn);
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
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchNextSteps exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task<CaseModel> PostCase(CreateCaseModel createCaseModel)
		{
			try
			{
				// Fetch Status
				var statusDto = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());

				// Fetch Type
				var typeDto = await _typeCachedService.GetTypeByNameAndDescription(
					createCaseModel.Type, createCaseModel.SubType);

				// Fetch Rating
				var ratingDto = await _ratingCachedService.GetRatingByName(createCaseModel.RagRatingName);

				// In a 1:1 case -> record (not multiple concerns) this flag is always true.
				// When multiple concerns is develop take into consideration the number of records attached to the case.
				const bool isCasePrimary = true;
				
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
			var recordsTasks = casesDto.Select(c => _recordCachedService.GetRecordsByCaseUrn(c)).ToList();
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
				.Where(c => c.Status.CompareTo(statusDto.Urn) == 0).ToList();
					
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