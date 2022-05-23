using ConcernsCaseWork.Models.CaseActions;
using Service.TRAMS.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Mappers
{
	public static class FinancialPlanMapping
	{

		public static FinancialPlanModel MapDtoToModel(FinancialPlanDto financialPlanDto, IList<FinancialPlanStatusDto> statuses)
		{
			var selectedStatus = statuses.FirstOrDefault(s => s.Id.CompareTo(financialPlanDto.StatusId) == 0);
			var selectedStatusId = selectedStatus != null ? selectedStatus.Id : (long?)null;

			var financialPlanModel = new FinancialPlanModel(
				financialPlanDto.Id,
				financialPlanDto.CaseUrn,
				financialPlanDto.CreatedAt,
				financialPlanDto.DatePlanRequested,
				financialPlanDto.DateViablePlanReceived,
				financialPlanDto.Notes,
				FinancialPlanStatusMapping.MapDtoToModel(statuses, selectedStatusId)
				);

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
					FinancialPlanStatusMapping.MapDtoToModel(statuses, financialPlanDto.StatusId));

				return financialPlanModel;
			}
			));


			return financialPlanModels;
		}
	
		public static FinancialPlanDto MapPatchFinancialPlanModelToDto(PatchFinancialPlanModel patchFinancialPlanModel,
			FinancialPlanDto financialPlanDto, IList<FinancialPlanStatusDto> statuses)
		{
			var selectedStatus = statuses.FirstOrDefault(s => s.Id.CompareTo(patchFinancialPlanModel.StatusId) == 0);
			var selectedStatusId = selectedStatus != null ? selectedStatus.Id : (long?)null;

			var updatedFinancialPlanDto = new FinancialPlanDto(
				financialPlanDto.Id,
				financialPlanDto.CaseUrn,
				financialPlanDto.CreatedAt,
				financialPlanDto.ClosedAt,
				financialPlanDto.CreatedBy,
				selectedStatusId,
				patchFinancialPlanModel.DatePlanRequested,
				patchFinancialPlanModel.DateViablePlanReceived,
				patchFinancialPlanModel.Notes);

			return updatedFinancialPlanDto;
		}
	
	}
}
