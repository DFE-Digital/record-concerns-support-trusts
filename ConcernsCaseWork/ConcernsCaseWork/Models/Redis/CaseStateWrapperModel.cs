using System;

namespace ConcernsCaseWork.Models.Redis
{
	[Serializable]
	public class CaseStateWrapperModel
	{
		public CaseModel CaseModel { get; set; }
		public RecordModel RecordModel { get; set; }
		public RecordRatingHistoryModel RecordRatingHistoryModel { get; set; }
	}
}