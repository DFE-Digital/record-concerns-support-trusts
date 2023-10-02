using ConcernsCaseWork.API.ResponseModels.Concerns.TeamCasework;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.TeamCasework
{
	public interface IGetConcernsCaseworkTeam
	{
		public Task<ConcernsCaseworkTeamResponse> Execute(string ownerId, CancellationToken cancellationToken);
	}

	public class GetConcernsCaseworkTeam : IGetConcernsCaseworkTeam
	{
		private readonly IConcernsTeamCaseworkGateway _gateway;

		public GetConcernsCaseworkTeam(IConcernsTeamCaseworkGateway gateway)
		{
			_gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
		}

		public async Task<ConcernsCaseworkTeamResponse> Execute(string ownerId, CancellationToken cancellationToken)
		{
			var record = await _gateway.GetByOwnerId(ownerId, cancellationToken);

			if (record is null)
				return null;
			return new ConcernsCaseworkTeamResponse { OwnerId = ownerId, TeamMembers = record.TeamMembers.Select(x => x.TeamMember).ToArray() };
		}
	}
}
