using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Trusts;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class GroupContactAddressFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static GroupContactAddressDto BuildGroupContactAddressDto()
		{
			return new GroupContactAddressDto(
				Fixture.Create<string>(), 
			Fixture.Create<string>(), 
			Fixture.Create<string>(), 
			Fixture.Create<string>(), 
			Fixture.Create<string>(), 
			Fixture.Create<string>());
		}
		
		public static GroupContactAddressModel BuildGroupContactAddressModel()
		{
			return new GroupContactAddressModel(				
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>(), 
				Fixture.Create<string>());
		}
	}
}