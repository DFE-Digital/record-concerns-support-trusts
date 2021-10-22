using AutoFixture;
using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class EstablishmentFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static EstablishmentSummaryDto BuildEstablishmentSummaryDto()
		{
			return new EstablishmentSummaryDto(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>());
		}

		public static List<EstablishmentSummaryDto> BuildListEstablishmentSummaryDto()
		{
			return new List<EstablishmentSummaryDto> { new EstablishmentSummaryDto(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>()) };
		}

		public static List<EstablishmentSummaryModel> BuildListEstablishmentSummaryModel()
		{
			return new List<EstablishmentSummaryModel> { new EstablishmentSummaryModel(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>()) };
		}

		public static EstablishmentDto BuildEstablishmentDto(string schoolCapacity = "1000")
		{
			return new EstablishmentDto(
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				Fixture.Create<EstablishmentTypeDto>(),
				BuildCensusDto(),
				Fixture.Create<string>(),
				schoolCapacity
				);
		}

		public static List<EstablishmentDto> BuildListEstablishmentDto()
		{
			return new List<EstablishmentDto>
			{
				BuildEstablishmentDto()
			};
		}

		public static List<EstablishmentModel> BuildListEstablishmentModel()
		{
			return new List<EstablishmentModel>
			{
				new EstablishmentModel(
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<EstablishmentTypeModel>(),
					Fixture.Create<CensusModel>(),
					Fixture.Create<string>(),
					Fixture.Create<string>()
					)
			};
		}
	
		public static CensusDto BuildCensusDto(string numberOfPupils = "100")
		{
			return new CensusDto(numberOfPupils);
		}
	
	}
}