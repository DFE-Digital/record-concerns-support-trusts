using AutoMapper;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Teams;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.MeansOfReferral;
using ConcernsCaseWork.Service.Teams;
using ConcernsCaseWork.Service.Trusts;

namespace ConcernsCaseWork.Mappers
{
	public sealed class AutoMapping : Profile
	{
		public AutoMapping()
		{
			// Trust summary
			CreateMap<TrustSearchDto, TrustSearchModel>();
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
			CreateMap<MeansOfReferralModel, MeansOfReferralDto>();
			CreateMap<MeansOfReferralDto, MeansOfReferralModel>();

			// Teams
			CreateMap<ConcernsCaseworkTeamDto, ConcernsTeamCaseworkModel>().ReverseMap();
		}
	}
}