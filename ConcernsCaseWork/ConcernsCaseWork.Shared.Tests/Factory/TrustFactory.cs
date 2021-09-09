using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;
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
					"trust-group-name", 
					"trust-companies-house-number", 
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
					"trust-group-name", 
					"trust-companies-house-number", 
					EstablishmentFactory.CreateListEstablishmentSummaryModel())
			};
		}

		public static TrustDetailsDto CreateTrustDetailsDto()
		{
			return new TrustDetailsDto(new GiasDataDto("ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number", 
				new GroupContactAddressDto("street", "locality", "additional-line", "town", "county", "postcode")),
				EstablishmentFactory.CreateListEstablishmentDto());
		}
		
		public static TrustDetailsModel CreateTrustDetailsModel()
		{
			return new TrustDetailsModel(new GiasDataModel("ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number", 
				new GroupContactAddressModel("street", "locality", "additional-line", "town", "county", "postcode")),
				EstablishmentFactory.CreateListEstablishmentModel());
		}
	}
}