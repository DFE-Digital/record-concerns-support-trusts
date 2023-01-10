using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels
{
    public class ConcernsRecordRequest
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        [StringLength(300)]
        public string Name { get; set; }
        
        [StringLength(300)]
        public string Description { get; set; }
        
        [StringLength(300)]
        public string Reason { get; set; }
        public int CaseUrn { get; set; }
        public int TypeId { get; set; }
        public int RatingId { get; set; }
        public int StatusId { get; set; }
        public int? MeansOfReferralId { get; set; }
    }
}