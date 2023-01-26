using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

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

	public async Task<IList<ActiveCaseSummaryResponse>> Execute(string ownerId, CancellationToken cancellationToken = default)
	{
		var team = await _teamCaseworkGateway.GetByOwnerId(ownerId, cancellationToken);

		if (team == null)
		{
			return new List<ActiveCaseSummaryResponse>();
		}

		var teamMembers = team.TeamMembers.Select(x => x.TeamMember).ToArray();

		if (!teamMembers.Any())
		{
			return new List<ActiveCaseSummaryResponse>();
		}
		
		var caseSummaries = await _caseSummaryGateway.GetActiveCaseSummariesByTeamMember(teamMembers);
		
		return caseSummaries.Select(CaseSummaryResponseFactory.Create).ToList();
	}
}