namespace ConcernsCaseWork.API.ResponseModels
{
    public class ConcernsRecordResponse
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public int RatingId { get; set; }
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public int CaseUrn { get; set; }
        public int? MeansOfReferralId { get; set; }
    }
}