using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Microsoft.Extensions.Logging;
using Service.Redis.FinancialPlan;
using Service.Redis.Models;
using Service.TRAMS.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.FinancialPlan
{
	public class FinancialPlanModelService : IFinancialPlanModelService
	{
		private readonly List<FinancialPlanModel> FinancialPlans;
		private readonly Random random;
		private readonly IFinancialPlanCachedService _financialPlanCachedService;
		private readonly ILogger<FinancialPlanModelService> _logger;

		public FinancialPlanModelService(IFinancialPlanCachedService financialPlanCachedService, ILogger<FinancialPlanModelService> logger)
		{
			_financialPlanCachedService = financialPlanCachedService;
			_logger = logger;
		}

		public Task<IList<FinancialPlanModel>> GetFinancialPlansModelByCaseUrn(long caseUrn, string caseworker)
		{
			_logger.LogInformation("FinancialPlanModelService::GetFinancialPlansModelByCaseUrn");

			var financialPlansDtoTask = _financialPlanCachedService.GetFinancialPlansByCaseUrn(caseUrn, caseworker);

			Task.WaitAll(financialPlansDtoTask);

			var financialPlansDto = financialPlansDtoTask.Result;

			// Map the financial plan dtos to model 
			var financialPlanModels = FinancialPlanMapping.MapDtoToModel(financialPlansDto);

			return Task.FromResult(financialPlanModels);
		}

		public async Task PatchFinancialPlanNotes(PatchFinancialPlanModel patchFinancialPlanModel, string caseworker)
		{
			try
			{
				// Fetch Records & statuses
				//var statusesDto = await _statusCachedService.GetStatuses();
				var financialPlanDtos =  await _financialPlanCachedService.GetFinancialPlansByCaseUrn(patchFinancialPlanModel.CaseUrn, caseworker);
				var financialPlanDto = financialPlanDtos.FirstOrDefault(fp => fp.Id.CompareTo(patchFinancialPlanModel.Id) == 0);
				//var statusDto = statusesDto.FirstOrDefault(s => s.Urn.CompareTo(patchRecordModel.StatusUrn) == 0);

				financialPlanDto = FinancialPlanMapping.MapNotes(patchFinancialPlanModel, financialPlanDto);

				await _financialPlanCachedService.PatchFinancialPlanById(financialPlanDto, caseworker);
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanModelService::PatchFinancialPlanNotes exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task PatchFinancialPlanStatus(PatchFinancialPlanModel patchFinancialPlanModel, string caseworker)
		{

			throw new NotImplementedException();


			//try
			//{
			//	// Fetch Records & statuses
			//	//var statusesDto = await _statusCachedService.GetStatuses();
			//	var financialPlanDtos = await _financialPlanCachedService.GetFinancialPlansByCaseUrn(patchFinancialPlanModel.CaseUrn, caseworker);

			//	var financialPlanDto = financialPlanDtos.FirstOrDefault(fp => fp.Id.CompareTo(patchFinancialPlanModel.Id) == 0);
			//	//var statusDto = statusesDto.FirstOrDefault(s => s.Urn.CompareTo(patchRecordModel.StatusUrn) == 0);

			//	//recordDto = RecordMapping.MapClosure(patchRecordModel, recordDto, statusDto);

			//	financialPlanDto = FinancialPlanMapping.MapDtoToModel(patchFinancialPlanModel);

			//	//_financialPlanCachedService.PatchFinancialPlanById()
			//}
			//catch (Exception ex)
			//{
			//	_logger.LogError("RecordModelService::PatchRecordStatus exception {Message}", ex.Message);

			//	throw;
			//}
		}

		public async Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanModel createFinancialPlanModel, string caseworker)
		{
			try
			{
				var createFinancialPlanDto = new CreateFinancialPlanDto(
					createFinancialPlanModel.CaseUrn,
					createFinancialPlanModel.CreatedAt,
					createFinancialPlanModel.CreatedBy,
					createFinancialPlanModel.StatusId,
					createFinancialPlanModel.DatePlanRequested,
					createFinancialPlanModel.DateViablePlanReceived,
					createFinancialPlanModel.Notes);

				var financialPlanDto = await _financialPlanCachedService.PostFinancialPlanByCaseUrn(createFinancialPlanDto, caseworker);

				return financialPlanDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanModelService::PostFinancialPlanByCaseUrn exception {Message}", ex.Message);

				throw;
			}
		}
	}
}
