using AutoMapper;
using ConcernsCaseWork.Models;
using Service.TRAMS.Models;

namespace ConcernsCaseWork.Mappers
{
	public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			CreateMap<CaseDto, CaseModel>(); // means you want to map from CaseDto to CaseModel
		}
	}
}