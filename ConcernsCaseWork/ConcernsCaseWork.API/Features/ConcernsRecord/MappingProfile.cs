using AutoMapper;

namespace ConcernsCaseWork.API.Features.ConcernsRecord
{
	using ConcernsCaseWork.API.Contracts.Concerns;
	using ConcernsCaseWork.Data.Models;

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<ConcernsRecord, ConcernsRecordResponse>()
				.ForMember(dest => dest.CaseUrn, opt => opt.MapFrom(src => src.ConcernsCase.Urn));

			CreateMap<ConcernsRecord, ConcernsRecordResponse>()
				.ForMember(dest => dest.CaseUrn, opt => opt.MapFrom(src => src.CaseId));

		}
	}
}
