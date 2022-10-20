using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Redis.Models;
using Microsoft.Extensions.Logging;
using ConcernsCaseWork.Service.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.FinancialPlan
{
	public class FinancialPlanModelService : IFinancialPlanModelService
	{
		private readonly IFinancialPlanCachedService _financialPlanCachedService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;
		private readonly ILogger<FinancialPlanModelService> _logger;

		public FinancialPlanModelService(IFinancialPlanCachedService financialPlanCachedService, IFinancialPlanStatusCachedService financialPlanStatusCachedService, ILogger<FinancialPlanModelService> logger)
		{
			_financialPlanCachedService = financialPlanCachedService;
			_financialPlanStatusCachedService = financialPlanStatusCachedService;
			_logger = logger;
		}

		public async Task<IList<FinancialPlanModel>> GetFinancialPlansModelByCaseUrn(long caseUrn, string caseworker)
		{
			_logger.LogInformation("FinancialPlanModelService::GetFinancialPlansModelByCaseUrn");

			var financialPlansDtoTask = _financialPlanCachedService.GetFinancialPlansByCaseUrn(caseUrn, caseworker);
			var statusesDtoTask = _financialPlanStatusCachedService.GetAllFinancialPlanStatusesAsync();

			await Task.WhenAll(financialPlansDtoTask, statusesDtoTask);

			var financialPlansDto = financialPlansDtoTask.Result;
			var statusesDto = statusesDtoTask.Result;

			// Map the financial plan dtos to model 
			return FinancialPlanMapping.MapDtoToModel(financialPlansDto, statusesDto);
		}

		public async Task<FinancialPlanModel> GetFinancialPlansModelById(long caseUrn, long financialPlanId, string caseworker)
		{
			try
			{
				var statusesDto = await _financialPlanStatusCachedService.GetAllFinancialPlanStatusesAsync();

				var financialPlanDto = await _financialPlanCachedService.GetFinancialPlansById(caseUrn, financialPlanId, caseworker);
				var financialPlanModel = FinancialPlanMapping.MapDtoToModel(financialPlanDto, statusesDto);

				return financialPlanModel;
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanModelService::GetFinancialPlansModelById exception {Message}", ex.Message);

				throw;
			}
		}	

		public async Task PatchFinancialById(PatchFinancialPlanModel patchFinancialPlanModel, string caseworker)
		{
			try
			{
				// Fetch Financial Plan & statuses
				var financialPlanDto = await _financialPlanCachedService.GetFinancialPlansById(patchFinancialPlanModel.CaseUrn, patchFinancialPlanModel.Id, caseworker);
				var statusesDto = await _financialPlanStatusCachedService.GetAllFinancialPlanStatusesAsync();

				financialPlanDto = FinancialPlanMapping.MapPatchFinancialPlanModelToDto(patchFinancialPlanModel, financialPlanDto, statusesDto);

				await _financialPlanCachedService.PatchFinancialPlanById(financialPlanDto, caseworker);
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanModelService::PatchFinancialPlanNotes exception {Message}", ex.Message);

				throw;
			}
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
