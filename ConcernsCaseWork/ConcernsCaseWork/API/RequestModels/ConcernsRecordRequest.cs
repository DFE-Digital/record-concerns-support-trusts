namespace Concerns.Data.RequestModels
{
    public class ConcernsRecordRequest
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public int CaseUrn { get; set; }
        public int TypeUrn { get; set; }
        public int RatingUrn { get; set; }
        public int StatusUrn { get; set; }
        public int? MeansOfReferralUrn { get; set; }
    }
}