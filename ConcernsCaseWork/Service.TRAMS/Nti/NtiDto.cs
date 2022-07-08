using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.TRAMS.Nti
{
	public class NtiDto
	{
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public ICollection<NtiReasonDto> Reasons { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string Notes { get; set; }
		public DateTime? ClosedAt { get; set; }
		public int? ClosedStatusId { get; set; }
		public ICollection<int> UnderConsiderationReasonsMapping { get; set; }
	}
}