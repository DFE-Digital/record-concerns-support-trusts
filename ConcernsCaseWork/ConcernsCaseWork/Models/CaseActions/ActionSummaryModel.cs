using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public record ActionSummaryModel
	{
		public string RelativeUrl { get; set; }
		public string StatusName { get; set; }
		public string Name { get; set; }
		public string OpenedDate { get; set; }
		public string ClosedDate { get; set; }

		// Properties required to do sorting
		// We can't sort on strings, because we lose the time
		// Once we can move the action summaries into a dedicated api these could be removed
		public DateTimeOffset RawOpenedDate { get; set; }
		public DateTimeOffset? RawClosedDate { get; set; }
	}
}