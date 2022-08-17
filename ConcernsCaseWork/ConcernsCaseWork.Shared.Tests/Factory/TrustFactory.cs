using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCasework.Service.Trusts;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TrustFactory
	{
		public static IList<TrustSearchDto> BuildListTrustSummaryDto()
		{
			Fixture fixture = new Fixture();
			return new List<TrustSearchDto>
			{
				new TrustSearchDto(
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(),
					fixture.Create<string>(),
					GroupContactAddressFactory.BuildGroupContactAddressDto(),
					EstablishmentFactory.BuildListEstablishmentSummaryDto())
			};
		}
		
		public static TrustSearch BuildTrustSearch(string groupName = "", string ukprn = "", string companiesHouseNumber = "")
		{
			return new TrustSearch(groupName, ukprn, companiesHouseNumber);
		}

		public static IList<TrustSearchModel> BuildListTrustSummaryModel()
		{
			Fixture fixture = new Fixture();
			return new List<TrustSearchModel>
			{
				new TrustSearchModel(
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(), 
					fixture.Create<string>(),
					fixture.Create<string>(),
					GroupContactAddressFactory.BuildGroupContactAddressModel(),
					EstablishmentFactory.BuildListEstablishmentSummaryModel())
			};
		}

		public static TrustDetailsDto BuildTrustDetailsDto(string ukPrn = "trust-ukprn")
		{
			return new TrustDetailsDto(
				GiasDataFactory.BuildGiasDataDto(ukPrn),
				IfdDataFactory.BuildIfdDataDto(),
				EstablishmentFactory.BuildListEstablishmentDto());
		}
		
		public static TrustDetailsModel BuildTrustDetailsModel()
		{
			return new TrustDetailsModel(GiasDataFactory.BuildGiasDataModel(),
				IfdDataFactory.BuildIfdDataModel(),
				EstablishmentFactory.BuildListEstablishmentModel());
		}
	}
}