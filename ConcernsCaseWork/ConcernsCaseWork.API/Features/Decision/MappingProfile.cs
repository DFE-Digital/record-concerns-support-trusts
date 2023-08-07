using AutoMapper;

namespace ConcernsCaseWork.API.Features.Decision
{
	using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Decision, GetByID.Result>();
				//.ForMember(dest => dest.CaseUrn, opt => opt.MapFrom(src => src.ConcernsCase.Urn));
		}
	}
}
