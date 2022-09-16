using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.Nti
{
	public class NtiDto
	{
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public DateTime? DateStarted { get; set; }
		public string Notes { get; set; }
		public int? StatusId { get; set; }

		[JsonProperty("NoticeToImproveReasonsMapping")]
		public ICollection<int> ReasonsMapping { get; set; }

		[JsonProperty("NoticeToImproveConditionsMapping")]
		public ICollection<int> ConditionsMapping { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
		public int? ClosedStatusId { get; set; }
		public string SumissionDecisionId { get; set; }
		public DateTime? DateNTILifted { get; set; }
		public DateTime? DateNTIClosed { get; set; }
	}
}
