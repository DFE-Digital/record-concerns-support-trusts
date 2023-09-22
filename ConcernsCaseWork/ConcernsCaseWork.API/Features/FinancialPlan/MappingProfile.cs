﻿using AutoMapper;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.FinancialPlan
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<FinancialPlanCase, FinancialPlanResponse>();
			CreateMap<FinancialPlanStatus, FinancialPlanStatus>();
		}
	}
}
