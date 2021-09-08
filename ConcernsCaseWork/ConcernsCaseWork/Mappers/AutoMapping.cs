using AutoMapper;
using ConcernsCaseWork.Models;
using Service.TRAMS.Dto;

namespace ConcernsCaseWork.Mappers
{
	public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			// Case | Record | Rating | Status
			CreateMap<CaseDto, CaseModel>();
			CreateMap<RecordDto, RecordModel>();
			CreateMap<RatingDto, RatingModel>();
			CreateMap<StatusDto, StatusModel>();
			
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