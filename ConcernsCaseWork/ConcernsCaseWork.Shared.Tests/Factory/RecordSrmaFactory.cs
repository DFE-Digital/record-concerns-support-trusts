using AutoFixture;
using ConcernsCasework.Service.RecordSrma;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordSrmaFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static List<RecordSrmaDto> BuildListRecordSrmaDto()
		{
			return new List<RecordSrmaDto>
			{
				new RecordSrmaDto(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 1, 1),
				new RecordSrmaDto(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 2, 2),
				new RecordSrmaDto(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 3, 3)
			};
		}

		public static RecordSrmaDto BuildRecordSrmaDto()
		{
			return new RecordSrmaDto(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 1, 1);
		}
		
		public static RecordSrmaDto BuildRecordSrmaDto(CreateRecordSrmaDto createRecordSrmaDto)
		{
			return new RecordSrmaDto(createRecordSrmaDto.Name, createRecordSrmaDto.Details, createRecordSrmaDto.Reason, createRecordSrmaDto.RecordUrn, Fixture.Create<int>());
		}

		public static CreateRecordSrmaDto BuildCreateRecordSrmaDto()
		{
			return new CreateRecordSrmaDto(Fixture.Create<string>(), Fixture.Create<string>(), Fixture.Create<string>(), 1);
		}
	}
}