using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using System;
using ConcernsCaseWork.Service.FinancialPlan;
using System.Collections.Generic;
using System.Linq;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.FinancialPlan;
using ConcernsCaseWork.Extensions;

namespace ConcernsCaseWork.Mappers
{
	public static class FinancialPlanMapping
	{
		public static FinancialPlanModel MapDtoToModel(
			FinancialPlanDto financialPlanDto, 
			GetCasePermissionsResponse casePermission)
		{
			var financialPlanModel = new FinancialPlanModel(
				financialPlanDto.Id,
				financialPlanDto.CaseUrn,
				financialPlanDto.CreatedAt,
				financialPlanDto.DatePlanRequested,
				financialPlanDto.DateViablePlanReceived,
				financialPlanDto.Notes,
				StatusIdToFinancialPlanStatus(financialPlanDto),
				financialPlanDto.ClosedAt,
				financialPlanDto.UpdatedAt
				);

			financialPlanModel.IsEditable = casePermission.HasEditPermissions();

			return financialPlanModel;
		}

		public static IList<FinancialPlanModel> MapDtoToModel(IList<FinancialPlanDto> financialPlanDtos)
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
					StatusIdToFinancialPlanStatus(financialPlanDto),
					financialPlanDto.ClosedAt, 
					financialPlanDto.UpdatedAt);

				return financialPlanModel;
			}
			));
			
			return financialPlanModels;
		}
	
		public static FinancialPlanDto MapPatchFinancialPlanModelToDto(PatchFinancialPlanModel patchFinancialPlanModel,
			FinancialPlanDto financialPlanDto)
		{
			var updatedFinancialPlanDto = new FinancialPlanDto(
				financialPlanDto.Id,
				financialPlanDto.CaseUrn,
				financialPlanDto.CreatedAt,
				patchFinancialPlanModel?.ClosedAt,
				financialPlanDto.CreatedBy,
				patchFinancialPlanModel.StatusId ?? financialPlanDto.StatusId,
				patchFinancialPlanModel?.DatePlanRequested,
				patchFinancialPlanModel?.DateViablePlanReceived,
				patchFinancialPlanModel?.Notes, 
				patchFinancialPlanModel?.UpdatedAt ?? DateTime.Now);

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
				status = model.Status.Description();
			}

			var result = new ActionSummaryModel()
			{
				ClosedDate = DateTimeHelper.ParseToDisplayDate(model.ClosedAt),
				Name = "Financial Plan",
				OpenedDate = DateTimeHelper.ParseToDisplayDate(model.CreatedAt),
				RelativeUrl = relativeUrl,
				StatusName = status,
				RawOpenedDate = model.CreatedAt,
				RawClosedDate = model.ClosedAt
			};

			return result;
		}

		private static FinancialPlanStatus? StatusIdToFinancialPlanStatus(FinancialPlanDto financialPlanDto)
		{
			return financialPlanDto.StatusId != null ? (FinancialPlanStatus)financialPlanDto.StatusId : null;
		}
	}
}
