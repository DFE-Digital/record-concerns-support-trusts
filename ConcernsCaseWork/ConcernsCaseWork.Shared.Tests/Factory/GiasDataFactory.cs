using AutoFixture;
using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class GiasDataFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static GiasDataDto BuildGiasDataDto(string ukPrn = "trust-ukprn")
		{
			return new GiasDataDto(ukPrn, 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(),
				GroupContactAddressFactory.BuildGroupContactAddressDto());
		}
		
		public static GiasDataModel BuildGiasDataModel()
		{
			return new GiasDataModel(Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				GroupContactAddressFactory.BuildGroupContactAddressModel());
		}
	}
}