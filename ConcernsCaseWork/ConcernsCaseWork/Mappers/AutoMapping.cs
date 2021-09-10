﻿using AutoMapper;
using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Rating;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Trusts;
using Service.TRAMS.Type;

namespace ConcernsCaseWork.Mappers
{
	public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			// Case | Record | Rating | Status | Types
			CreateMap<CaseDto, CaseModel>();
			CreateMap<CreateCaseDto, CaseDto>();
			CreateMap<RecordDto, RecordModel>();
			CreateMap<CreateRecordDto, RecordDto>();
			CreateMap<RatingDto, RatingModel>();
			CreateMap<StatusDto, StatusModel>();
			CreateMap<TypeDto, TypeModel>();

			// Trust summary
			CreateMap<TrustSummaryDto, TrustSummaryModel>();
			CreateMap<EstablishmentSummaryDto, EstablishmentSummaryModel>();
			
			// Trust details
			CreateMap<TrustDetailsDto, TrustDetailsModel>();
			CreateMap<GiasDataDto, GiasDataModel>();
			CreateMap<GroupContactAddressDto, GroupContactAddressModel>();
			CreateMap<EstablishmentDto, EstablishmentModel>();
		}
	}
}