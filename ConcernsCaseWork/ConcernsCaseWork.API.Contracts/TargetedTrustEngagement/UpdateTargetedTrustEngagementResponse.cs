namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement
{
	public class UpdateTargetedTrustEngagementResponse
	{
		public UpdateTargetedTrustEngagementResponse()
		{
		}

		public UpdateTargetedTrustEngagementResponse(int concernsCaseUrn, int targetedTrustEngagementId)
		{
			ConcernsCaseUrn = concernsCaseUrn >= 0 ? concernsCaseUrn : throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn), "value must be greater than zero");
			TargetedTrustEngagementId = targetedTrustEngagementId >= 0 ? targetedTrustEngagementId : throw new ArgumentOutOfRangeException(nameof(targetedTrustEngagementId), "value must be greater than zero");
		}
		public int ConcernsCaseUrn { get; set; }
		public int TargetedTrustEngagementId { get; set; }
	}
}
