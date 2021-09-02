using AutoMapper;
using ConcernsCaseWork.Models;
using Service.TRAMS.Models;

namespace ConcernsCaseWork.Mappers
{
	public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			// means you want to map from CaseDto to CaseModel
			CreateMap<CaseDto, CaseModel>();
			
			// Trust summary
			CreateMap<TrustSummaryDto, TrustSummaryModel>();
			CreateMap<EstablishmentSummaryDto, EstablishmentSummaryModel>();
			
			// Trust details
			CreateMap<TrustDetailsDto, TrustDetailsModel>();
			CreateMap<GiasDataDto, GiasDataModel>();
			CreateMap<GroupContactAddressDto, GroupContactAddressModel>();
		}
	}
}