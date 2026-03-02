using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Data.Gateways
{
	public interface IConcernsCaseRiskToTrustRatingHistoryGateway
	{
		void CreateHistory(int caseId, int RatingId, string rationalCommentary);
	}

    public class ConcernsCaseRiskToTrustRatingHistoryGateway(ConcernsDbContext concernsDbContext, ILogger<ConcernsCaseRiskToTrustRatingHistoryGateway> logger) : IConcernsCaseRiskToTrustRatingHistoryGateway
    {
        private readonly ConcernsDbContext _concernsDbContext = concernsDbContext;
        private readonly ILogger<ConcernsCaseRiskToTrustRatingHistoryGateway> _logger = logger;

		public void CreateHistory(int caseId, int RatingId, string rationalCommentary)
        {
            var entity = new ConcernsCaseRiskToTrustRatingHistory
			{
                CaseId = caseId,
				RatingId = RatingId,
				RationalCommentary = rationalCommentary,
				CreatedAt = DateTime.Now
			};

            try
            {
				_concernsDbContext.ConcernsCaseRiskToTrustRatingHistory.Add(entity);

				_concernsDbContext.SaveChanges();
			}
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create Trust RAG history for Case Id {CaseId}. Exception: {ExceptionMessage}", entity.CaseId, ex.Message);

                throw new InvalidOperationException($"Failed to create Trust RAG history for Case Id {entity.CaseId}. See inner exception for details.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An application exception has occurred whilst creating Trust RAG rational history for Case Id {CaseId}. Exception: {ExceptionMessage}", entity.CaseId, ex.Message);

                throw new InvalidOperationException($"An error occurred while creating Trust RAG rational history for Case Id {entity.CaseId}. See inner exception for details.", ex);
            }
        }
    }
}
