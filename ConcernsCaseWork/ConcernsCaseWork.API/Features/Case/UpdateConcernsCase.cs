using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;
using System;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface IUpdateConcernsCase
	{
		ConcernsCaseResponse Execute(int urn, ConcernCaseRequest request);
	}

	public class UpdateConcernsCase : IUpdateConcernsCase
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly IConcernsCaseRiskToTrustRatingHistoryGateway _concernsCaseRiskToTrustRagRationalHistoryGateway;

		public UpdateConcernsCase(
			IConcernsCaseGateway concernsCaseGateway, IConcernsCaseRiskToTrustRatingHistoryGateway concernsCaseRiskToTrustRagRationalHistoryGateway)
		{
			_concernsCaseGateway = concernsCaseGateway;
			_concernsCaseRiskToTrustRagRationalHistoryGateway = concernsCaseRiskToTrustRagRationalHistoryGateway;
		}

		public ConcernsCaseResponse Execute(int urn, ConcernCaseRequest request)
		{
			var currentConcernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(urn);

			if (!request.RatingRationalCommentary.Equals(currentConcernsCase.RatingRationalCommentary, StringComparison.OrdinalIgnoreCase))
			{
				request.RatingRational = true;
			}

			request.RatingRationalCommentary = request.RatingRationalCommentary ?? "The RAG rationale commentary is not available yet";

			if (currentConcernsCase.RatingId != request.RatingId)
			{
				_concernsCaseRiskToTrustRagRationalHistoryGateway.CreateHistory(
					currentConcernsCase.Id,
					currentConcernsCase.RatingId,
					currentConcernsCase.RatingRationalCommentary
				);
			}

			var concernsCaseToUpdate = ConcernsCaseFactory.Update(currentConcernsCase, request);
			var updatedConcernsCase = _concernsCaseGateway.Update(concernsCaseToUpdate);
			return ConcernsCaseResponseFactory.Create(updatedConcernsCase);
		}
	}
}