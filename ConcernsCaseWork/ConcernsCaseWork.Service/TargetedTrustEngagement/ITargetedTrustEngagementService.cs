using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;

namespace ConcernsCaseWork.Service.TargetedTrustEngagement
{
	public interface ITargetedTrustEngagementService
	{
		Task<CreateTargetedTrustEngagementResponse> PostTargetedTrustEngagement(CreateTargetedTrustEngagementRequest createTargetedTrustEngagementRequestDto);

		Task<List<TargetedTrustEngagementSummaryResponse>> GetTargetedTrustEngagementByCaseUrn(int urn);

		Task<GetTargetedTrustEngagementResponse> GetTargetedTrustEngagement(int urn, int targetedTrustEngagementId);

		Task<UpdateTargetedTrustEngagementResponse> PutTargetedTrustEngagement(int caseUrn, int targetedTrustEngagementId, UpdateTargetedTrustEngagementRequest updateTargetedTrustEngagementRequest);

		Task<CloseTargetedTrustEngagementResponse> CloseTargetedTrustEngagement(int caseUrn, int targetedTrustEngagementId, CloseTargetedTrustEngagementRequest closeTargetedTrustEngagementRequest);

		Task DeleteTargetedTrustEngagement(int caseUrn, int targetedTrustEngagementId);
	}
}
