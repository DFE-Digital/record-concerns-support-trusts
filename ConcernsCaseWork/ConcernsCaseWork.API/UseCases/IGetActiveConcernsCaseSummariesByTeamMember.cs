using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetActiveConcernsCaseSummariesByTeamMember
{
	Task<IList<ActiveCaseSummaryResponse>> Execute(string ownerId, CancellationToken cancellationToken);
}