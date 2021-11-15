using AutoMapper;
using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Trusts;

namespace ConcernsCaseWork.Mappers
{
	public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			// Trust summary
			CreateMap<TrustSummaryDto, TrustSummaryModel>();
			CreateMap<EstablishmentSummaryDto, EstablishmentSummaryModel>();
			
			// Trust details
			CreateMap<TrustDetailsDto, TrustDetailsModel>();
			CreateMap<GiasDataDto, GiasDataModel>();
			CreateMap<GroupContactAddressDto, GroupContactAddressModel>();
			CreateMap<EstablishmentDto, EstablishmentModel>();
			CreateMap<EstablishmentTypeDto, EstablishmentTypeModel>();
			CreateMap<CensusDto, CensusModel>();
			CreateMap<IfdDataDto, IfdDataModel>();
			
			// Case
			CreateMap<CaseHistoryDto, CaseHistoryModel>();
		}
	}
}