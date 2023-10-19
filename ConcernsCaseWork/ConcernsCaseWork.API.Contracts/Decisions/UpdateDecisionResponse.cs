﻿namespace ConcernsCaseWork.API.Contracts.Decisions
{
	public class UpdateDecisionResponse
	{
		public UpdateDecisionResponse()
		{

		}
		public UpdateDecisionResponse(int concernsCaseUrn, int decisionId)
		{
			ConcernsCaseUrn = concernsCaseUrn >= 0 ? concernsCaseUrn : throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn), "value must be greater than zero");
			DecisionId = decisionId >= 0 ? decisionId : throw new ArgumentOutOfRangeException(nameof(decisionId), "value must be greater than zero");
		}

		public int ConcernsCaseUrn { get; set; }
		public int DecisionId { get; set; }
	}
}
