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

		public static EstablishmentDto BuildEstablishmentDto()
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
				Fixture.Create<string>(),
				Fixture.Create<string>()
				);
		}

		public static List<EstablishmentDto> BuildListEstablishmentDto()
		{
			return new List<EstablishmentDto>
			{
				new EstablishmentDto(
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<string>(),
					Fixture.Create<EstablishmentTypeDto>(),
					Fixture.Create<string>(),
					Fixture.Create<string>()
					)
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
					Fixture.Create<string>(),
					Fixture.Create<string>()
					)
			};
		}
	}
}