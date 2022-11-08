using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions
{
	public class GetDecisionsRequest
	{
		[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
		public int ConcernsCaseUrn { get; set; }

		public GetDecisionsRequest(int concernsCaseUrn)
		{
			ConcernsCaseUrn = concernsCaseUrn <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : concernsCaseUrn;
		}
	}
}
