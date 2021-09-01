using ConcernsCaseWork.Models;
using Service.TRAMS.Models;
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
	}
}