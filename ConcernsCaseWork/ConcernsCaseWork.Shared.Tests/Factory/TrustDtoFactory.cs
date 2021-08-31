using Service.TRAMS.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TrustDtoFactory
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
					EstablishmentDtoFactory.CreateListEstablishmentSummaryDto())
			};
		}
	}
}