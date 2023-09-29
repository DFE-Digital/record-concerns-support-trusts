using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;
namespace ConcernsCaseWork.API.Features.Case;

public interface IGetActiveConcernsCaseSummariesForUsersTeam
{
	Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesForUsersTeamParameters parameters, CancellationToken cancellationToken);
}

public class GetActiveConcernsCaseSummariesForUsersTeam : IGetActiveConcernsCaseSummariesForUsersTeam
{
	private readonly ICaseSummaryGateway _caseSummaryGateway;
	private readonly IConcernsTeamCaseworkGateway _teamCaseworkGateway;

	public GetActiveConcernsCaseSummariesForUsersTeam(
		ICaseSummaryGateway caseSummaryGateway,
		IConcernsTeamCaseworkGateway teamCaseworkGateway)
	{
		_caseSummaryGateway = caseSummaryGateway;
		_teamCaseworkGateway = teamCaseworkGateway;
	}

	public async Task<(IList<ActiveCaseSummaryResponse>, int)> Execute(GetCaseSummariesForUsersTeamParameters parameters, CancellationToken cancellationToken = default)
	{
		var team = await _teamCaseworkGateway.GetByOwnerId(parameters.UserID, cancellationToken);

		if (team == null)
			return (new List<ActiveCaseSummaryResponse>(), 0);

		parameters.teamMemberIds = team.TeamMembers.Select(x => x.TeamMember).ToArray();

		if (!parameters.teamMemberIds.Any())
			return (new List<ActiveCaseSummaryResponse>(), 0);

		(IList<ActiveCaseSummaryVm> caseSummaries, int recordCount) = await _caseSummaryGateway.GetActiveCaseSummariesByTeamMembers(parameters);

		return (caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList(), recordCount);
	}
}