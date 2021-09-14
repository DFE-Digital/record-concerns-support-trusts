using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TrustFactory
	{
		public static IList<TrustSummaryDto> BuildListTrustSummaryDto()
		{
			return new List<TrustSummaryDto>
			{
				new TrustSummaryDto(
					"trust-ukprn", 
					"trust-urn", 
					"trust-group-name", 
					"trust-companies-house-number", 
					EstablishmentFactory.BuildListEstablishmentSummaryDto())
			};
		}
		
		public static TrustSearch BuildTrustSearch(string groupName = "", string ukprn = "", string companiesHouseNumber = "")
		{
			return new TrustSearch(groupName, ukprn, companiesHouseNumber);
		}

		public static IList<TrustSummaryModel> BuildListTrustSummaryModel()
		{
			return new List<TrustSummaryModel>
			{
				new TrustSummaryModel(
					"trust-ukprn", 
					"trust-urn", 
					"trust-group-name", 
					"trust-companies-house-number", 
					EstablishmentFactory.BuildListEstablishmentSummaryModel())
			};
		}

		public static TrustDetailsDto BuildTrustDetailsDto()
		{
			return new TrustDetailsDto(new GiasDataDto("trust-ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number", 
				new GroupContactAddressDto("street", "locality", "additional-line", "town", "county", "postcode")),
				EstablishmentFactory.BuildListEstablishmentDto());
		}
		
		public static List<TrustDetailsDto> BuildListTrustDetailsDto()
		{
			return new List<TrustDetailsDto> {
				new TrustDetailsDto(new GiasDataDto("ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number", 
					new GroupContactAddressDto("street", "locality", "additional-line", "town", "county", "postcode")),
				EstablishmentFactory.BuildListEstablishmentDto()),
				new TrustDetailsDto(new GiasDataDto("ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number", 
						new GroupContactAddressDto("street", "locality", "additional-line", "town", "county", "postcode")),
					EstablishmentFactory.BuildListEstablishmentDto())
			};
		}
		
		public static TrustDetailsModel BuildTrustDetailsModel()
		{
			return new TrustDetailsModel(new GiasDataModel("ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number", 
				new GroupContactAddressModel("street", "locality", "additional-line", "town", "county", "postcode")),
				EstablishmentFactory.BuildListEstablishmentModel());
		}
	}
}