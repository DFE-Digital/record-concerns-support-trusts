using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Exceptions;
using ConcernsCaseWork.Data.Models.Decisions;

namespace ConcernsCaseWork.Data.Models
{
    public class ConcernsCaseRiskToTrustRatingHistory
    {
		public int Id { get; set; }

		public int CaseId { get; set; }

		public int RatingId { get; set; }

		public string RationalCommentary { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
