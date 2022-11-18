using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Trusts;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class GiasDataFactory
	{
		private readonly static Fixture _fixture = new Fixture();
		
		public static GiasDataDto BuildGiasDataDto(string ukPrn = "trust-ukprn")
		{
			return new GiasDataDto(ukPrn, 
				_fixture.Create<string>(), 
				_fixture.Create<string>(), 
				_fixture.Create<string>(), 
				_fixture.Create<string>(),
				GroupContactAddressFactory.BuildGroupContactAddressDto(),
				_fixture.Create<string>());
		}
		
		public static GiasDataModel BuildGiasDataModel()
		{
			return new GiasDataModel(_fixture.Create<string>(), 
				_fixture.Create<string>(), 
				_fixture.Create<string>(), 
				_fixture.Create<string>(), 
				_fixture.Create<string>(), 
				GroupContactAddressFactory.BuildGroupContactAddressModel(),
				_fixture.Create<string>());
		}
	}
}