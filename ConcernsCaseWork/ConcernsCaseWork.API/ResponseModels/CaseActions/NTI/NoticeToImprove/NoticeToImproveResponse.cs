namespace ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove
{
    public class NoticeToImproveResponse
    {
		public long Id { get; set; }
		public int CaseUrn { get; set; }
        public int? StatusId { get; set; }
        public DateTime? DateStarted { get; set; }
		public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
		public string CreatedBy { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedStatusId { get; set; }
        public string SumissionDecisionId { get; set; }
        public DateTime? DateNTILifted { get; set; }
        public DateTime? DateNTIClosed { get; set; }

        public ICollection<int> NoticeToImproveReasonsMapping { get; set; }
		public ICollection<int> NoticeToImproveConditionsMapping { get; set; }
	}
}
