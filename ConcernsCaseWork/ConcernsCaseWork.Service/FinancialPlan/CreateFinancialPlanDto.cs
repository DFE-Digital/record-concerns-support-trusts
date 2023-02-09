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

		[JsonProperty("notes")]
		public string Notes { get; }
		
		[JsonProperty("updatedAt")]
		public DateTime UpdatedAt { get; set; }

		[JsonConstructor]
		public CreateFinancialPlanDto(long caseUrn, DateTime createdAt, string createdBy, long? statusId, DateTime? datePlanRequested, string notes, DateTime updatedAt) =>
			(CaseUrn, CreatedAt, CreatedBy, StatusId, DatePlanRequested, Notes, UpdatedAt) =
			(caseUrn, createdAt, createdBy, statusId, datePlanRequested, notes, updatedAt);
	}
}
