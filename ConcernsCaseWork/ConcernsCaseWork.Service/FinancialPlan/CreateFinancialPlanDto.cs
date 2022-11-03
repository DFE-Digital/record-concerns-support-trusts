using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.FinancialPlan
{
	public sealed class CreateFinancialPlanDto
	{
		[JsonProperty("caseUrn")]
		public long CaseUrn { get; set; }

		[JsonProperty("createdAt")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("createdBy")]
		public string CreatedBy { get; set; }

		[JsonProperty("statusId")]
		public long? StatusId { get; }

		[JsonProperty("datePlanRequested")]
		public DateTime? DatePlanRequested { get; }

		[JsonProperty("dateViablePlanReceived")]
		public DateTime? DateViablePlanReceived { get; }

		[JsonProperty("notes")]
		public string Notes { get; }

		[JsonConstructor]
		public CreateFinancialPlanDto(long caseUrn, DateTime createdAt, string createdBy, long? statusId, DateTime? datePlanRequested, DateTime? dateViablePlanReceived, string notes) =>
			(CaseUrn, CreatedAt, CreatedBy, StatusId, DatePlanRequested, DateViablePlanReceived, Notes) =
			(caseUrn, createdAt, createdBy, statusId, datePlanRequested, dateViablePlanReceived, notes);
	}
}
