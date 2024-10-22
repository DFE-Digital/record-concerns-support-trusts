using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Utils.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
	public class TargetedTrustEngagementCase : IAuditable
    {
	    public int Id { get; set; }
        public int CaseUrn { get; set; }
		public DateTimeOffset? EngagementStartDate { get; set; }
		public ICollection<TargetedTrustEngagementType> ActivityTypes { get; set; }
		// Entity framework will make this VARCHAR(MAX) over 4000, without specifying
		[Column(TypeName = "VARCHAR(2000)")]
		[StringLength(TargetedTrustEngagementConstants.MaxSupportingNotesLength)]
		public string Notes { get; set; }

		public DateTimeOffset? EngagementEndDate { get; set; }
		public int? EngagementOutcomeId { get; set; }
		public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string CreatedBy { get; set; }
		public DateTime? DeletedAt { get; set; }

		public string GetTitle()
		{
			switch (ActivityTypes?.Count ?? 0)
			{
				case 0:
					return "No Activity Types";

				default:
					return ActivityTypes.First().ActivityId.Description();
			}
		}
	}
}