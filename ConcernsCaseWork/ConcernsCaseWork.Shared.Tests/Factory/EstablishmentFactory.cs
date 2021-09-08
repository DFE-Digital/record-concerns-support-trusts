using ConcernsCaseWork.Models;
using Service.TRAMS.Dto;
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
		
		public static List<EstablishmentDto> CreateListEstablishmentDto()
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
		
		public static List<EstablishmentModel> CreateListEstablishmentModel()
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