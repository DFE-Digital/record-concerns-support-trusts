using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public interface IGetActiveConcernsCaseSummariesForUsersTeam
{
	Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesForUsersTeamParameters parameters, CancellationToken cancellationToken);
}