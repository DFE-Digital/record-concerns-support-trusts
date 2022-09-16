using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsTeamCaseworkGateway
    {
        Task<ConcernsCaseworkTeam> GetByOwnerId(string ownerId, CancellationToken cancellationToken);
        Task UpdateCaseworkTeam(ConcernsCaseworkTeam team, CancellationToken cancellationToken);
        Task AddCaseworkTeam(ConcernsCaseworkTeam team, CancellationToken cancellationToken);
        Task<string[]> GetTeamOwners(CancellationToken cancellationToken);
    }
}
