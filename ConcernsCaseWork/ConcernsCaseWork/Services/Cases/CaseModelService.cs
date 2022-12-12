using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Ratings;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Redis.Types;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Service.Status;
using Microsoft.Extensions.Logging;
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
		private readonly IRecordService _recordService;
		private readonly ITrustCachedService _trustCachedService;
		private readonly ITypeCachedService _typeCachedService;
		private readonly ICaseSearchService _caseSearchService;
		private readonly ICaseService _caseService;
		private readonly ILogger<CaseModelService> _logger;

		public CaseModelService(
			ITrustCachedService trustCachedService, 
			IRecordService recordService, 
			IRatingCachedService ratingCachedService,
			ITypeCachedService typeCachedService,
			IStatusCachedService statusCachedService,
			ICaseSearchService caseSearchService,
			ICaseService caseService,
			ILogger<CaseModelService> logger)
		{
			_statusCachedService = statusCachedService;
			_ratingCachedService = ratingCachedService;
			_recordService = recordService;
			_trustCachedService = trustCachedService;
			_typeCachedService = typeCachedService;
			_caseSearchService = caseSearchService;
			_caseService = caseService;
			_logger = logger;
		}

		public async Task<CaseModel> GetCaseByUrn(long urn)
		{
			try
			{
				var caseDto = await _caseService.GetCaseByUrn(urn);
				var caseModel = CaseMapping.Map(caseDto);
				
				return caseModel;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::GetCaseByUrn exception {Message}", ex.Message);
				
				throw;
			}
		}

		public async Task PatchClosure(PatchCaseModel patchCaseModel)
		{
			try
			{
				var statusDto = await _statusCachedService.GetStatusByName(patchCaseModel.StatusName);
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				caseDto = CaseMapping.MapClosure(patchCaseModel, caseDto, statusDto);
				
				await _caseService.PatchCaseByUrn(caseDto);
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);

				caseDto = CaseMapping.MapRating(patchCaseModel, caseDto);
				
				await _caseService.PatchCaseByUrn(caseDto);
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
				var recordsDto = await _recordService.GetRecordsByCaseUrn(patchRecordModel.CaseUrn);

				var recordDto = recordsDto.FirstOrDefault(r => r.Id.CompareTo(patchRecordModel.Id) == 0);
				recordDto = RecordMapping.MapRating(patchRecordModel, recordDto);

				await _recordService.PatchRecordById(recordDto);
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				caseDto = CaseMapping.MapDirectionOfTravel(patchCaseModel, caseDto);

				await _caseService.PatchCaseByUrn(caseDto);
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapIssue(patchCaseModel, caseDto);

				await _caseService.PatchCaseByUrn(caseDto);
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapCaseAim(patchCaseModel, caseDto);

				await _caseService.PatchCaseByUrn(caseDto);
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapCurrentStatus(patchCaseModel, caseDto);

				await _caseService.PatchCaseByUrn(caseDto);
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapDeEscalationPoint(patchCaseModel, caseDto);

				await _caseService.PatchCaseByUrn(caseDto);
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				// Patch source dtos
				caseDto = CaseMapping.MapNextSteps(patchCaseModel, caseDto);

				await _caseService.PatchCaseByUrn(caseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchNextSteps exception {Message}", ex.Message);

				throw;
			}
		}
		
		public async Task PatchCaseHistory(long caseUrn, string userName, string caseHistory)
		{
			try
			{
				var caseDto = await _caseService.GetCaseByUrn(caseUrn);
				
				var newCaseDto = caseDto with { UpdatedAt = DateTimeOffset.Now, CaseHistory = caseHistory };

				await _caseService.PatchCaseByUrn(newCaseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PatchCaseHistory exception {Message}", ex.Message);

				throw;
			}
		}
				
		public async Task PatchTerritory(int caseUrn, string userName, TerritoryEnum? territory)
		{
			try
			{
				var caseDto = await _caseService.GetCaseByUrn(caseUrn);
				
				var newCaseDto = caseDto with { UpdatedAt = DateTimeOffset.Now, Territory = territory };

				await _caseService.PatchCaseByUrn(newCaseDto);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

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
				var newCase = await _caseService.PostCase(CaseMapping.Map(createCaseModel));

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
					
					return _recordService.PostRecordByCaseUrn(createRecordDto);
				});

				await Task.WhenAll(recordTasks);
				
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