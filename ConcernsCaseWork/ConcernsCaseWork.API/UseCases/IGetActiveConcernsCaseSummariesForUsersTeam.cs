using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetActiveConcernsCaseSummariesForUsersTeam
{
	Task<IList<ActiveCaseSummaryResponse>> Execute(string ownerId, CancellationToken cancellationToken);
}