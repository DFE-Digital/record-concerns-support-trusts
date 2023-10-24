using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.Service.Permissions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.FinancialPlan
{
	public class FinancialPlanModelService : IFinancialPlanModelService
	{
		private readonly IFinancialPlanService _financialPlanService;
		private readonly ICasePermissionsService _casePermissionsService;
		private readonly ILogger<FinancialPlanModelService> _logger;

		public FinancialPlanModelService(
			IFinancialPlanService financialPlanCachedService, 
			ICasePermissionsService casePermissionsService,
			ILogger<FinancialPlanModelService> logger)
		{
			_financialPlanService = financialPlanCachedService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task<IList<FinancialPlanModel>> GetFinancialPlansModelByCaseUrn(long caseUrn)
		{
			_logger.LogInformation("FinancialPlanModelService::GetFinancialPlansModelByCaseUrn");

			var financialPlansDto = await _financialPlanService.GetFinancialPlansByCaseUrn(caseUrn);

			// Map the financial plan dtos to model 
			return FinancialPlanMapping.MapDtoToModel(financialPlansDto);
		}

		public async Task<FinancialPlanModel> GetFinancialPlansModelById(long caseUrn, long financialPlanId)
		{
			try
			{
				var permissions = await _casePermissionsService.GetCasePermissions(caseUrn);

				var financialPlanDto = await _financialPlanService.GetFinancialPlansById(caseUrn, financialPlanId);
				var financialPlanModel = FinancialPlanMapping.MapDtoToModel(financialPlanDto, permissions);

				return financialPlanModel;
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanModelService::GetFinancialPlansModelById exception {Message}", ex.Message);

				throw;
			}
		}	

		public async Task PatchFinancialById(PatchFinancialPlanModel patchFinancialPlanModel)
		{
			try
			{
				// Fetch Financial Plan & statuses
				var financialPlanDto = await _financialPlanService.GetFinancialPlansById(patchFinancialPlanModel.CaseUrn, patchFinancialPlanModel.Id);

				financialPlanDto = FinancialPlanMapping.MapPatchFinancialPlanModelToDto(patchFinancialPlanModel, financialPlanDto);

				await _financialPlanService.PatchFinancialPlanById(financialPlanDto);
			}
			catch (Exception ex)
			{
				_logger.LogError("FinancialPlanModelService::PatchFinancialPlanNotes exception {Message}", ex.Message);

				throw;
			}
		}

		public async Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanModel createFinancialPlanModel)
		{
			try
			{
				var createFinancialPlanDto = new CreateFinancialPlanDto(
					createFinancialPlanModel.CaseUrn,
					createFinancialPlanModel.CreatedAt,
					createFinancialPlanModel.CreatedBy,
					createFinancialPlanModel.StatusId,
					createFinancialPlanModel.DatePlanRequested,
					createFinancialPlanModel.Notes,
					createFinancialPlanModel.UpdatedAt);

				var financialPlanDto = await _financialPlanService.PostFinancialPlanByCaseUrn(createFinancialPlanDto);

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
