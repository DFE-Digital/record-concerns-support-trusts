using System;
using System.Collections.Generic;

namespace Service.TRAMS.NtiUnderConsideration
{
	public class NtiUnderConsiderationDto
	{
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public ICollection<NtiUnderConsiderationReasonDto> Reasons { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string Notes { get; set; }
		public DateTime? ClosedAt { get; set; }
		public int? ClosedStatusId { get; set; }
		public string ClosedStatusName { get; set; }
		public ICollection<int> UnderConsiderationReasonsMapping { get; set; }
	}
}