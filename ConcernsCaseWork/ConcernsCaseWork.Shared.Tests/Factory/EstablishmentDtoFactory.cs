using Service.TRAMS.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class EstablishmentDtoFactory
	{
		public static List<EstablishmentSummaryDto> CreateListEstablishmentSummaryDto()
		{
			return new List<EstablishmentSummaryDto>
			{
				new EstablishmentSummaryDto("establishment-urn", "establishment-name", "establishment-ukprn")
			};
		}
	}
}