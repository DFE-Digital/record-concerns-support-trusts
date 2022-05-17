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
		public static IList<FinancialPlanModel> MapDtoToModel(IList<FinancialPlanDto> financialPlanDtos)
		{
			var financialPlanModels = new List<FinancialPlanModel>();

			if (financialPlanDtos is null || !financialPlanDtos.Any())
			{
				return financialPlanModels;
			}


			// TODO - get accurate status
			financialPlanModels.AddRange(financialPlanDtos.Select(financialPlanDto =>
			{
				var financialPlanModel = new FinancialPlanModel(
					financialPlanDto.Id,
					financialPlanDto.CaseUrn,
					financialPlanDto.CreatedAt,
					Enums.FinancialPlanStatus.ViablePlanReceived,
					financialPlanDto.DatePlanRequested,
					financialPlanDto.DateViablePlanReceived,
					financialPlanDto.Notes);

				return financialPlanModel;
			}
			));


			return financialPlanModels;
		}
	
		public static FinancialPlanDto MapNotes(PatchFinancialPlanModel patchFinancialPlanModel,
			FinancialPlanDto financialPlanDto)
		{
			//return new FinancialPlanDto()
			return null;
		}
	
	}
}
