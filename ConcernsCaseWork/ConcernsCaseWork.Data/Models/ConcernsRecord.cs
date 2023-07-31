using System.Diagnostics;

namespace ConcernsCaseWork.Data.Models
{
    public class ConcernsRecord: IAuditable
    {
		public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime? ClosedAt { get; set; }
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

		public DateTime? DeletedAt { get; set; }

		//Todo: BB 25/07/2023 Make constructor private once concernsrecord refactor is complete
		public ConcernsRecord()
		{
			
		}

		protected ConcernsRecord(int caseUrn, int typeId, int ratingId, int meansOfReferralId, int statusId)
		{
			this.CaseId = caseUrn;
			this.TypeId = typeId;
			this.RatingId = ratingId;
			this.MeansOfReferralId = meansOfReferralId; 
			this.StatusId = statusId;
		}

		public static ConcernsRecord Create(int caseUrn, int typeId, int ratingId, int meansOfReferralId, int statusId)
		{
			return new ConcernsRecord(caseUrn,typeId, ratingId, meansOfReferralId, statusId);
		}

		public void ChangeNameDescriptionAndReason(string name, string description, string reason)
		{
			this.Name = name;
			this.Description = description;
			this.Reason = reason;
		}

		public void SetAuditInformation(DateTime createdAt, DateTime updatedAt, DateTime reviewAt)
		{
			this.CreatedAt = createdAt;
			this.UpdatedAt = updatedAt;
			this.ReviewAt = reviewAt;
		}
	}
}