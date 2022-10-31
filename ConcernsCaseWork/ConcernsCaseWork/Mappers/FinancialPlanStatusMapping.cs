using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.FinancialPlan;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class FinancialPlanStatusMapping
	{
		public static FinancialPlanStatusModel MapDtoToModel(IList<FinancialPlanStatusDto> statusesDto, long? id)
		{
			var selectedStatusDto = statusesDto.FirstOrDefault(s => s.Id.CompareTo(id) == 0);

			return selectedStatusDto is null ? null : new FinancialPlanStatusModel(selectedStatusDto.Name, selectedStatusDto.Id, selectedStatusDto.IsClosedStatus);
		}
		
		public static FinancialPlanStatusModel MapDtoToModel(FinancialPlanStatusDto statusDto)
			=> new FinancialPlanStatusModel(statusDto.Name, statusDto.Id, statusDto.IsClosedStatus);
	}
}