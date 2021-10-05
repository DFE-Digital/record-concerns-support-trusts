using AutoFixture;
using Service.TRAMS.RecordWhistleblower;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordWhistleblowerFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static List<RecordWhistleblowerDto> BuildListRecordWhistleblowerDto()
		{
			return new List<RecordWhistleblowerDto>
			{
				new RecordWhistleblowerDto(Fixture.Create<string>(), Fixture.Create<string>(),
					Fixture.Create<string>(), 1, 1),
				new RecordWhistleblowerDto(Fixture.Create<string>(), Fixture.Create<string>(),
					Fixture.Create<string>(), 2, 2)
			};
		}

		public static RecordWhistleblowerDto BuildRecordWhistleblowerDto()
		{
			return new RecordWhistleblowerDto(Fixture.Create<string>(), Fixture.Create<string>(),
				Fixture.Create<string>(), 1, 1);
		}

		public static CreateRecordWhistleblowerDto BuildCreateRecordWhistleblowerDto()
		{
			return new CreateRecordWhistleblowerDto(Fixture.Create<string>(), Fixture.Create<string>(),
				Fixture.Create<string>(), 1);
		}
	}
}