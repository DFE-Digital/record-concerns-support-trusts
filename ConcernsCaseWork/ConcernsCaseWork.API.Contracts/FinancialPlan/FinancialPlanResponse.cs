namespace ConcernsCaseWork.API.Contracts.FinancialPlan
{
	public class FinancialPlanResponse
    {
        public long Id { get; set; }
        public int CaseUrn { get; set; }
        public string Name { get; set; }
        public long? StatusId { get; set; }
        public DateTime? DatePlanRequested { get; set; }
        public DateTime? DateViablePlanReceived { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string Notes { get; set; }
		public DateTime? DeletedAt { get; set; }

		public FinancialPlanStatusResponse Status { get; set; }
    }

	public class FinancialPlanStatusResponse
	{
		public long Id { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public bool IsClosedStatus { get; set; }
	}
}