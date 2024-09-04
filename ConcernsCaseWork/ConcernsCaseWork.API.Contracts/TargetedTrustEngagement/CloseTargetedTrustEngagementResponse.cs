namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;

public class CloseTargetedTrustEngagementResponse
{
	public CloseTargetedTrustEngagementResponse()
	{

	}
	public CloseTargetedTrustEngagementResponse(int caseUrn, int targetedTrustEngagementId)
	{
		CaseUrn = caseUrn >= 0 ? caseUrn : throw new ArgumentOutOfRangeException(nameof(caseUrn), "value must be greater than zero");
		TargetedTrustEngagementId = targetedTrustEngagementId >= 0 ? targetedTrustEngagementId : throw new ArgumentOutOfRangeException(nameof(targetedTrustEngagementId), "value must be greater than zero");
	}

	public int CaseUrn { get; set; }
	public int TargetedTrustEngagementId { get; set; }
}