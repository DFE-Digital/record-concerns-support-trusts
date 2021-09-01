using ConcernsCaseWork.Models;
using Service.TRAMS.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class EstablishmentFactory
	{
		public static List<EstablishmentSummaryDto> CreateListEstablishmentSummaryDto()
		{
			return new List<EstablishmentSummaryDto>
			{
				new EstablishmentSummaryDto("establishment-urn", "establishment-name", "establishment-ukprn")
			};
		}

		public static List<EstablishmentSummaryModel> CreateListEstablishmentSummaryModel()
		{
			return new List<EstablishmentSummaryModel>
			{
				new EstablishmentSummaryModel("establishment-urn", "establishment-name", "establishment-ukprn")
			};
		}
	}
}