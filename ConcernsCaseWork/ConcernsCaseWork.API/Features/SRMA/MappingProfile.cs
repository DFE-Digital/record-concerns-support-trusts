﻿using AutoMapper;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.SRMA
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<SRMACase, SRMAResponse>()
				.ForMember(dest => dest.DateVisitStart, opt => opt.MapFrom(src => src.StartDateOfVisit))
				.ForMember(dest => dest.DateVisitEnd, opt => opt.MapFrom(src => src.EndDateOfVisit))
				.ForMember(dest => dest.Reason, opt => opt.MapFrom(source => (SRMAReasonOffered?)source.ReasonId))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(source => (Contracts.Srma.SRMAStatus)source.StatusId))
				.ForMember(dest => dest.CloseStatus, opt => opt.MapFrom(source => (Contracts.Srma.SRMAStatus)(source.CloseStatusId ?? 0)));

			CreateMap<SRMACase, SRMAResponse>()
				.ForMember(dest => dest.DateVisitStart, opt => opt.MapFrom(src => src.StartDateOfVisit))
				.ForMember(dest => dest.DateVisitEnd, opt => opt.MapFrom(src => src.EndDateOfVisit))
				.ForMember(dest => dest.Reason, opt => opt.MapFrom(source => (SRMAReasonOffered?)source.ReasonId))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(source => (Contracts.Srma.SRMAStatus)source.StatusId))
				.ForMember(dest => dest.CloseStatus, opt => opt.MapFrom(source => (Contracts.Srma.SRMAStatus)(source.CloseStatusId ?? 0)));
		}
	}
}
