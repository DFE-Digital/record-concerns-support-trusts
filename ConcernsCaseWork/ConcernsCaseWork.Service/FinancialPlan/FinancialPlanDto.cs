using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.FinancialPlan
{
	public sealed class FinancialPlanDto
	{
		[JsonProperty("id")]
		public long Id { get; }

		[JsonProperty("caseUrn")]
		public long CaseUrn { get; set; }

		[JsonProperty("createdAt")]
		public DateTime CreatedAt { get; }

		[JsonProperty("closedAt")]
		public DateTime? ClosedAt { get; }

		[JsonProperty("createdBy")]
		public string CreatedBy { get; }

		[JsonProperty("statusId")]
		public long? StatusId { get; }

		[JsonProperty("datePlanRequested")]
		public DateTime? DatePlanRequested { get; }

		[JsonProperty("dateViablePlanReceived")]
		public DateTime? DateViablePlanReceived { get; }

		[JsonProperty("notes")]
		public string Notes { get; }

		[JsonConstructor]
		public FinancialPlanDto(long id, long caseUrn, DateTime createdAt, DateTime? closedAt, string createdBy, long? statusId, DateTime? datePlanRequested, DateTime? dateViablePlanReceived, string notes) =>
			(Id, CaseUrn, CreatedAt, ClosedAt, CreatedBy, StatusId, DatePlanRequested, DateViablePlanReceived, Notes) =
			(id, caseUrn, createdAt, closedAt, createdBy, statusId, datePlanRequested, dateViablePlanReceived, notes);
	}
}
