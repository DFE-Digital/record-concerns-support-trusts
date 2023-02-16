﻿using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Redis.Models;
using Microsoft.Extensions.Logging;
using ConcernsCaseWork.Service.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.Service.Permissions;

namespace ConcernsCaseWork.Services.FinancialPlan
{
	public class FinancialPlanModelService : IFinancialPlanModelService
	{
		private readonly IFinancialPlanService _financialPlanService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;
		private readonly ICasePermissionsService _casePermissionsService;
		private readonly ILogger<FinancialPlanModelService> _logger;

		public FinancialPlanModelService(
			IFinancialPlanService financialPlanCachedService, 
			IFinancialPlanStatusCachedService financialPlanStatusCachedService,
			ICasePermissionsService casePermissionsService,
			ILogger<FinancialPlanModelService> logger)
		{
			_financialPlanService = financialPlanCachedService;
			_financialPlanStatusCachedService = financialPlanStatusCachedService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task<IList<FinancialPlanModel>> GetFinancialPlansModelByCaseUrn(long caseUrn)
		{
			_logger.LogInformation("FinancialPlanModelService::GetFinancialPlansModelByCaseUrn");

			var financialPlansDtoTask = _financialPlanService.GetFinancialPlansByCaseUrn(caseUrn);
			var statusesDtoTask = _financialPlanStatusCachedService.GetAllFinancialPlanStatusesAsync();

			await Task.WhenAll(financialPlansDtoTask, statusesDtoTask);

			var financialPlansDto = financialPlansDtoTask.Result;
			var statusesDto = statusesDtoTask.Result;

			// Map the financial plan dtos to model 
			return FinancialPlanMapping.MapDtoToModel(financialPlansDto, statusesDto);
		}

		public async Task<FinancialPlanModel> GetFinancialPlansModelById(long caseUrn, long financialPlanId)
		{
			try
			{
				var statusesDto = await _financialPlanStatusCachedService.GetAllFinancialPlanStatusesAsync();
				var permissions = await _casePermissionsService.GetCasePermissions(caseUrn);

				var financialPlanDto = await _financialPlanService.GetFinancialPlansById(caseUrn, financialPlanId);
				var financialPlanModel = FinancialPlanMapping.MapDtoToModel(financialPlanDto, statusesDto, permissions);

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
				var statusesDto = await _financialPlanStatusCachedService.GetAllFinancialPlanStatusesAsync();

				financialPlanDto = FinancialPlanMapping.MapPatchFinancialPlanModelToDto(patchFinancialPlanModel, financialPlanDto, statusesDto);

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
