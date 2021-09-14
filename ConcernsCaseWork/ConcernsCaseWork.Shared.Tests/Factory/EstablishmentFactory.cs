using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class EstablishmentFactory
	{
		public static EstablishmentSummaryDto BuildEstablishmentSummaryDto()
		{
			return new EstablishmentSummaryDto("establishment-urn", "establishment-name", "establishment-ukprn");
		}

		public static List<EstablishmentSummaryDto> BuildListEstablishmentSummaryDto()
		{
			return new List<EstablishmentSummaryDto> { new EstablishmentSummaryDto("establishment-urn", "establishment-name", "establishment-ukprn") };
		}

		public static List<EstablishmentSummaryModel> BuildListEstablishmentSummaryModel()
		{
			return new List<EstablishmentSummaryModel> { new EstablishmentSummaryModel("establishment-urn", "establishment-name", "establishment-ukprn") };
		}

		public static EstablishmentDto BuildEstablishmentDto()
		{
			return new EstablishmentDto(
				"establishment-urn",
				"establishment-local-authority-code",
				"establishment-local-authority-name",
				"establishment-number",
				"establishment-name");
		}

		public static List<EstablishmentDto> BuildListEstablishmentDto()
		{
			return new List<EstablishmentDto>
			{
				new EstablishmentDto(
					"establishment-urn",
					"establishment-local-authority-code",
					"establishment-local-authority-name",
					"establishment-number",
					"establishment-name")
			};
		}

		public static List<EstablishmentModel> BuildListEstablishmentModel()
		{
			return new List<EstablishmentModel>
			{
				new EstablishmentModel(
					"establishment-urn",
					"establishment-local-authority-code",
					"establishment-local-authority-name",
					"establishment-number",
					"establishment-name")
			};
		}
	}
}