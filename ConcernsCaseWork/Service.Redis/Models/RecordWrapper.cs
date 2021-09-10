using Service.TRAMS.RecordRatingHistory;
using Service.TRAMS.Records;
using System;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class RecordWrapper
	{
		public RecordDto RecordDto { get; set; }
		public IList<RecordRatingHistoryDto> RecordsRatingHistory { get; set; } = new List<RecordRatingHistoryDto>();
	}
}