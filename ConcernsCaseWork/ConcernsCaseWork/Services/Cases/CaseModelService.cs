using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Records;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	// They are very similarly named and it is not obvious which of the two to use.
	public sealed class CaseModelService : ICaseModelService
	{
		private readonly IRecordService _recordService;
		private readonly ICaseService _caseService;
		private readonly ILogger<CaseModelService> _logger;

		public CaseModelService(
			IRecordService recordService, 
			ICaseService caseService,
			ILogger<CaseModelService> logger)
		{
			_recordService = recordService;
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
				var caseDto = await _caseService.GetCaseByUrn(patchCaseModel.Urn);
				
				caseDto = CaseMapping.MapClosure(patchCaseModel, caseDto);
				
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
				
		public async Task PatchTerritory(int caseUrn, string userName, Territory? territory)
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

		public async Task PatchRegion(int caseUrn, Region? region)
		{
			var caseDto = await _caseService.GetCaseByUrn(caseUrn);

			var newCaseDto = caseDto with { UpdatedAt = DateTimeOffset.Now, Region = region };

			await _caseService.PatchCaseByUrn(newCaseDto);
		}

		public async Task<long> PostCase(CreateCaseModel createCaseModel)
		{
			try
			{
				// Create a case
				createCaseModel.StatusId = (int)CaseStatus.Live;
				var newCase = await _caseService.PostCase(CaseMapping.Map(createCaseModel));

				await PostConcerns(createCaseModel, newCase.Urn);
				
				return newCase.Urn;
			}
			catch (Exception ex)
			{
				_logger.LogError("CaseModelService::PostCase exception {Message}", ex.Message);

				throw;
			}
		}
		
		public async Task<long> PatchCase(int caseUrn,CreateCaseModel createCaseModel)
		{
			
			try
			{
				var caseDto = await _caseService.GetCaseByUrn(caseUrn);

				var newCaseDto = caseDto with { 
					UpdatedAt = DateTimeOffset.Now,
					CaseHistory = createCaseModel.CaseHistory,
					Issue = createCaseModel.Issue,
					CaseAim = createCaseModel.CaseAim,
					CrmEnquiry = createCaseModel.CrmEnquiry,
					CurrentStatus = createCaseModel.CurrentStatus,
					NextSteps = createCaseModel.NextSteps,
					RatingId = createCaseModel.RatingId,
					DeEscalationPoint = createCaseModel.DeEscalationPoint,
					DirectionOfTravel = createCaseModel.DirectionOfTravel
					
					
				};
				createCaseModel.StatusId = caseDto.StatusId;
				var urn =await _caseService.PatchCaseByUrn(newCaseDto);
				if (createCaseModel.CreateRecordsModel.Any())
				{
					 await PostConcerns(createCaseModel, caseUrn);
				}
				return urn.Urn;
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				throw;
			}
				
		}
		
		public async Task PatchOwner(int caseUrn, string owner)
		{
			try
			{
				var caseDto = await _caseService.GetCaseByUrn(caseUrn);

				var newCaseDto = caseDto with { UpdatedAt = DateTimeOffset.Now, CreatedBy = owner };

				await _caseService.PatchCaseByUrn(newCaseDto);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				throw;
			}
		}

		private async Task PostConcerns(CreateCaseModel createCaseModel, long newCaseId)
		{
			// Create records
			var recordTasks = createCaseModel.CreateRecordsModel.Select(recordModel =>
			{
				var currentDate = DateTimeOffset.Now;
				var createRecordDto = new CreateRecordDto(
					currentDate,
					currentDate,
					currentDate,
					null,
					null,
					null,
					newCaseId,
					recordModel.TypeId,
					recordModel.RatingId,
					createCaseModel.StatusId,
					recordModel.MeansOfReferralId);

				return _recordService.PostRecordByCaseUrn(createRecordDto);
			});

			await Task.WhenAll(recordTasks);
		}
	}
}