using Service.TRAMS.RecordSrma;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordSrmaFactory
	{
		public static List<RecordSrmaDto> BuildListRecordSrmaDto()
		{
			return new List<RecordSrmaDto>
			{
				new RecordSrmaDto("record-srma-name", "record-srma-details", "record-srma-reason", 1, 1),
				new RecordSrmaDto("record-srma-name", "record-srma-details", "record-srma-reason", 2, 2),
				new RecordSrmaDto("record-srma-name", "record-srma-details", "record-srma-reason", 3, 3)
			};
		}

		public static RecordSrmaDto BuildRecordSrmaDto()
		{
			return new RecordSrmaDto("record-srma-name", "record-srma-details", "record-srma-reason", 1, 1);
		}

		public static CreateRecordSrmaDto BuildCreateRecordSrmaDto()
		{
			return new CreateRecordSrmaDto("record-srma-name", "record-srma-details", "record-srma-reason", 1);
		}
	}
}