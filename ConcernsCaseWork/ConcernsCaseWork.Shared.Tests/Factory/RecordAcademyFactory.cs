using Service.TRAMS.RecordAcademy;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordAcademyFactory
	{
		public static List<RecordAcademyDto> BuildListRecordAcademyDto()
		{
			return new List<RecordAcademyDto>
			{
				new RecordAcademyDto(1, 1, 1), 
				new RecordAcademyDto(2, 2, 2)
			};
		}

		public static RecordAcademyDto BuildRecordAcademyDto()
		{
			return new RecordAcademyDto(1, 1, 1);
		}

		public static CreateRecordAcademyDto BuildCreateRecordAcademyDto()
		{
			return new CreateRecordAcademyDto(1, 1);
		}
	}
}