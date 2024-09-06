using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;

public class CloseTargetedTrustEngagementRequest
{
	public DateTimeOffset? EngagementEndDate { get; set; }

	public TargetedTrustEngagementOutcome OutcomeId { get; set; }

	[StringLength(TargetedTrustEngagementConstants.MaxSupportingNotesLength, ErrorMessage = "Notes must be 2000 characters or less", MinimumLength = 0)]
	public string Notes { get; set; }

	public bool IsValid()
	{
		return (Notes?.Length ?? 0) <= TargetedTrustEngagementConstants.MaxSupportingNotesLength;
	}
}