using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Trusts;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class IfdDataFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static IfdDataDto BuildIfdDataDto()
		{
			return new IfdDataDto(
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				GroupContactAddressFactory.BuildGroupContactAddressDto());
		}
		
		public static IfdDataModel BuildIfdDataModel()
		{
			return new IfdDataModel(
				Fixture.Create<string>(),
				Fixture.Create<string>(),
				GroupContactAddressFactory.BuildGroupContactAddressModel());
		}
	}
}