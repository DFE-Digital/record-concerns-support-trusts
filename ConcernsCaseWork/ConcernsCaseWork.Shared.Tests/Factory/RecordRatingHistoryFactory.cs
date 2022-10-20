using ConcernsCaseWork.Service.RecordRatingHistory;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RecordRatingHistoryFactory
	{
		public static List<RecordRatingHistoryDto> BuildListRecordRatingHistoryDto()
		{
			return new List<RecordRatingHistoryDto>
			{
				new RecordRatingHistoryDto(DateTimeOffset.Now, 1, 1), 
				new RecordRatingHistoryDto(DateTimeOffset.Now, 2, 2), 
				new RecordRatingHistoryDto(DateTimeOffset.Now, 3, 3)
			};
		}
		
		public static RecordRatingHistoryDto BuildRecordRatingHistoryDto()
		{
			return new RecordRatingHistoryDto(DateTimeOffset.Now, 1, 1);
		}
	}
}