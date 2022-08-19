using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.Nti
{
	public class NtiDto
	{
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public DateTime? DateLetterSent { get; set; }
		public string Notes { get; set; }
		public int? StatusId { get; set; }
		public ICollection<int> ReasonsMapping { get; set; }
		public ICollection<int> ConditionsMapping { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
		public int? ClosedStatusId { get; set; }
	}
}
