namespace ConcernsCaseWork.Data.Models
{
    public class ConcernsRecord
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public int CaseId { get; set; }
        public int TypeId { get; set; }
        public int RatingId { get; set; }
        public int StatusId { get; set; }
        public int? MeansOfReferralId { get; set; }
        public virtual ConcernsCase ConcernsCase { get; set; }
        public virtual ConcernsType ConcernsType { get; set; }
        public virtual ConcernsRating ConcernsRating { get; set; }
        public virtual ConcernsMeansOfReferral ConcernsMeansOfReferral { get; set; }
        public virtual ConcernsStatus Status { get; set; }
    }
}