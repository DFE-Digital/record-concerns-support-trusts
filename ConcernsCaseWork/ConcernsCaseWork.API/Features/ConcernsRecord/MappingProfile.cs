using AutoMapper;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	using ConcernsCaseWork.Data.Models;

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<ConcernsRecord, GetByID.Result>()
				.ForMember(dest => dest.CaseUrn, opt => opt.MapFrom(src => src.ConcernsCase.Urn));
		}
	}
}
