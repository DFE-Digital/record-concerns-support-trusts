﻿using AutoFixture;
using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TrustFactory
	{
		public static IList<TrustSummaryDto> BuildListTrustSummaryDto()
		{
			Fixture fixture = new Fixture();
			return new List<TrustSummaryDto>
			{
				new TrustSummaryDto(
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

		public static IList<TrustSummaryModel> BuildListTrustSummaryModel()
		{
			Fixture fixture = new Fixture();
			return new List<TrustSummaryModel>
			{
				new TrustSummaryModel(
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