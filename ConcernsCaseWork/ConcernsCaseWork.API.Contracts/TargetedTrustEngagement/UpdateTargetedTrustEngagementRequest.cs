using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement
{
	public class UpdateTargetedTrustEngagementRequest
	{
		[Range(1, int.MaxValue, ErrorMessage = "The CaseUrn must be greater than zero")]
		public int CaseUrn { get; set; }

		public DateTimeOffset? EngagementStartDate { get; set; }


		public TargetedTrustEngagementActivity? ActivityId { get; set; }
		public TargetedTrustEngagementActivityType[] ActivityTypes { get; set; }


		[StringLength(TargetedTrustEngagementConstants.MaxSupportingNotesLength, ErrorMessage = "Notes must be 2000 characters or less", MinimumLength = 0)]
		public string Notes { get; set; }

		public string CreatedBy { get; set; }

		public bool IsValid()
		{
			return (Notes?.Length ?? 0) <= TargetedTrustEngagementConstants.MaxSupportingNotesLength;
		}
	}
}
