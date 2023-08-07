using AutoMapper;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<FinancialPlanCase, GetByID.Result>();
			CreateMap<FinancialPlanStatus, GetByID.StatusModel>();
		}
	}
}
