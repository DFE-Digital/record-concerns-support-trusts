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
			return new TrustDetailsDto(GiasDataFactory.BuildGiasDataDto(), EstablishmentFactory.BuildListEstablishmentDto());
		}
		
		public static List<TrustDetailsDto> BuildListTrustDetailsDto()
		{
			return new List<TrustDetailsDto> {
				new TrustDetailsDto(GiasDataFactory.BuildGiasDataDto(),
				EstablishmentFactory.BuildListEstablishmentDto()),
				new TrustDetailsDto(GiasDataFactory.BuildGiasDataDto(),
					EstablishmentFactory.BuildListEstablishmentDto())
			};
		}
		
		public static TrustDetailsModel BuildTrustDetailsModel()
		{
			return new TrustDetailsModel(GiasDataFactory.BuildGiasDataModel(),
				EstablishmentFactory.BuildListEstablishmentModel());
		}
	}
}