﻿using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using System;
using ConcernsCaseWork.Service.FinancialPlan;
using System.Collections.Generic;
using System.Linq;
using ConcernsCaseWork.API.Contracts.Permissions;

namespace ConcernsCaseWork.Mappers
{
	public static class FinancialPlanMapping
	{
		public static FinancialPlanModel MapDtoToModel(
			FinancialPlanDto financialPlanDto, 
			IList<FinancialPlanStatusDto> statuses,
			GetCasePermissionsResponse casePermission)
		{
			var selectedStatus = statuses.FirstOrDefault(s => s.Id.CompareTo(financialPlanDto.StatusId) == 0);
			var selectedStatusId = selectedStatus?.Id;

			var financialPlanModel = new FinancialPlanModel(
				financialPlanDto.Id,
				financialPlanDto.CaseUrn,
				financialPlanDto.CreatedAt,
				financialPlanDto.DatePlanRequested,
				financialPlanDto.DateViablePlanReceived,
				financialPlanDto.Notes,
				FinancialPlanStatusMapping.MapDtoToModel(statuses, selectedStatusId),
				financialPlanDto.ClosedAt
				);

			financialPlanModel.IsEditable = casePermission.HasEditPermissions();

			return financialPlanModel;
		}

		public static IList<FinancialPlanModel> MapDtoToModel(IList<FinancialPlanDto> financialPlanDtos, IList<FinancialPlanStatusDto> statuses)
		{
			var financialPlanModels = new List<FinancialPlanModel>();

			if (financialPlanDtos is null || !financialPlanDtos.Any())
			{
				return financialPlanModels;
			}

			financialPlanModels.AddRange(financialPlanDtos.Select(financialPlanDto =>
			{
				var financialPlanModel = new FinancialPlanModel(
					financialPlanDto.Id,
					financialPlanDto.CaseUrn,
					financialPlanDto.CreatedAt,
					financialPlanDto.DatePlanRequested,
					financialPlanDto.DateViablePlanReceived,
					financialPlanDto.Notes,
					FinancialPlanStatusMapping.MapDtoToModel(statuses, financialPlanDto.StatusId),
					financialPlanDto.ClosedAt);

				return financialPlanModel;
			}
			));
			
			return financialPlanModels;
		}
	
		public static FinancialPlanDto MapPatchFinancialPlanModelToDto(PatchFinancialPlanModel patchFinancialPlanModel,
			FinancialPlanDto financialPlanDto, IEnumerable<FinancialPlanStatusDto> statuses)
		{
			var selectedStatus = statuses.FirstOrDefault(s => s.Id.CompareTo(patchFinancialPlanModel.StatusId) == 0);
			var selectedStatusId = selectedStatus?.Id;

			var updatedFinancialPlanDto = new FinancialPlanDto(
				financialPlanDto.Id,
				financialPlanDto.CaseUrn,
				financialPlanDto.CreatedAt,
				patchFinancialPlanModel?.ClosedAt ?? financialPlanDto.ClosedAt,
				financialPlanDto.CreatedBy,
				selectedStatusId ?? financialPlanDto.StatusId,
				patchFinancialPlanModel?.DatePlanRequested ?? financialPlanDto.DatePlanRequested,
				patchFinancialPlanModel?.DateViablePlanReceived ?? financialPlanDto.DateViablePlanReceived,
				patchFinancialPlanModel?.Notes ?? financialPlanDto.Notes);

			return updatedFinancialPlanDto;
		}

		public static ActionSummaryModel ToActionSummary(this FinancialPlanModel model)
		{
			var relativeUrl = $"/case/{model.CaseUrn}/management/action/financialplan/{model.Id}";

			if (model.IsClosed)
			{
				relativeUrl += "/closed";
			}

			var status = "In progress";

			if (model.Status != null)
			{
				var financialStatusEnum = (FinancialPlanStatus)Enum.Parse(typeof(FinancialPlanStatus), model.Status.Name);
				status = EnumHelper.GetEnumDescription(financialStatusEnum);
			}

			var result = new ActionSummaryModel()
			{
				ClosedDate = model.ClosedAt.ToDayMonthYear(),
				Name = "Financial Plan",
				OpenedDate = model.CreatedAt.ToDayMonthYear(),
				RelativeUrl = relativeUrl,
				StatusName = status,
				RawOpenedDate = model.CreatedAt,
				RawClosedDate = model.ClosedAt
			};

			return result;
		}
	}
}
