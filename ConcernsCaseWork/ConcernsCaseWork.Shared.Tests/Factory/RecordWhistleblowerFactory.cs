using Service.TRAMS.RecordWhistleblower;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordWhistleblowerFactory
	{
		public static List<RecordWhistleblowerDto> BuildListRecordWhistleblowerDto()
		{
			return new List<RecordWhistleblowerDto>
			{
				new RecordWhistleblowerDto("recordWhistleblower-name", "recordWhistleblower-details",
					"recordWhistleblower-reason", 1, 1),
				new RecordWhistleblowerDto("recordWhistleblower-name", "recordWhistleblower-details",
					"recordWhistleblower-reason", 2, 2)
			};
		}

		public static RecordWhistleblowerDto BuildRecordWhistleblowerDto()
		{
			return new RecordWhistleblowerDto("recordWhistleblower-name", "recordWhistleblower-details",
				"recordWhistleblower-reason", 1, 1);
		}

		public static CreateRecordWhistleblowerDto BuildCreateRecordWhistleblowerDto()
		{
			return new CreateRecordWhistleblowerDto("recordWhistleblower-name", "recordWhistleblower-details",
				"recordWhistleblower-reason", 1);
		}
	}
}