using System;

namespace ConcernsCaseWork.Redis.Models
{
	[Serializable]
	public sealed class CreateFinancialPlanModel
	{
		public long CaseUrn { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? DatePlanRequested { get; set; }
		public DateTime? DateViablePlanReceived { get; set; }
		public long? StatusId { get; set; }
		public string Notes { get; set; }
		public string CreatedBy { get; set; }
	}
}
