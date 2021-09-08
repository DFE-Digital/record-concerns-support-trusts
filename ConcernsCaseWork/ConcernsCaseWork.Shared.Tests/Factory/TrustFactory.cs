using ConcernsCaseWork.Models;
using Service.TRAMS.Dto;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TrustFactory
	{
		public static IList<TrustSummaryDto> CreateListTrustSummaryDto()
		{
			return new List<TrustSummaryDto>
			{
				new TrustSummaryDto(
					"trust-ukprn", 
					"trust-urn", 
					"trust-groupname", 
					"trust-companieshousenumber", 
					EstablishmentFactory.CreateListEstablishmentSummaryDto())
			};
		}
		
		public static TrustSearch CreateTrustSearch(string groupName = "", string ukprn = "", string companiesHouseNumber = "")
		{
			return new TrustSearch(groupName, ukprn, companiesHouseNumber);
		}

		public static IList<TrustSummaryModel> CreateListTrustSummaryModel()
		{
			return new List<TrustSummaryModel>
			{
				new TrustSummaryModel(
					"trust-ukprn", 
					"trust-urn", 
					"trust-groupname", 
					"trust-companieshousenumber", 
					EstablishmentFactory.CreateListEstablishmentSummaryModel())
			};
		}

		public static TrustDetailsDto CreateTrustDetailsDto()
		{
			return new TrustDetailsDto(new GiasDataDto("ukprn", "groupid", "groupname", "Multi-academy trust", "companieshousenumber", 
				new GroupContactAddressDto("street", "locality", "additionalline", "town", "county", "postcode")));
		}
		
		public static TrustDetailsModel CreateTrustDetailsModel()
		{
			return new TrustDetailsModel(new GiasDataModel("ukprn", "groupid", "groupname", "Multi-academy trust", "companieshousenumber", 
				new GroupContactAddressModel("street", "locality", "additionalline", "town", "county", "postcode")));
		}
	}
}