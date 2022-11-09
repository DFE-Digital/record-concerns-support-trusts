using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions
{
	/// <summary>
	/// This request is created within the API as the resource is accessed via restful URI.
	/// Arguably should be moved to an internal location within the API, and probably will be at some point when time permits.
	/// </summary>
	public class GetDecisionRequest
	{
		[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
		public int ConcernsCaseUrn { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "The DecisionId must be greater than zero")]
		public int DecisionId { get; set; }

		public GetDecisionRequest(int concernsCaseUrn, int decisionId)
		{
			_ = concernsCaseUrn <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : concernsCaseUrn;
			_ = decisionId <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : decisionId;

			ConcernsCaseUrn = concernsCaseUrn;
			DecisionId = decisionId;
		}
	}
}
