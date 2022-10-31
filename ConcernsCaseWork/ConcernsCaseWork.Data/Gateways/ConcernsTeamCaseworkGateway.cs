using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using static Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsTeamCaseworkGateway : IConcernsTeamCaseworkGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;

        public ConcernsTeamCaseworkGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext ?? throw new ArgumentNullException(nameof(concernsDbContext));
        }

        public async Task AddCaseworkTeam(ConcernsCaseworkTeam team, CancellationToken cancellationToken)
        {
            _ = team ?? throw new ArgumentNullException(nameof(team));
            _ = team.TeamMembers ?? throw new ArgumentNullException(nameof(team.TeamMembers));

            _concernsDbContext.ConcernsTeamCaseworkTeam.Add(team);
            await _concernsDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<ConcernsCaseworkTeam> GetByOwnerId(string ownerId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentNullException(nameof(ownerId));
            }

            return await _concernsDbContext.ConcernsTeamCaseworkTeam
                .Include(t => t.TeamMembers)
                .FirstOrDefaultAsync(x => x.Id == ownerId, cancellationToken);
        }

        public async Task UpdateCaseworkTeam(ConcernsCaseworkTeam team, CancellationToken cancellationToken)
        {
            _ = team ?? throw new ArgumentNullException(nameof(team));
            if (string.IsNullOrWhiteSpace(team.Id))
            {
                throw new ArgumentNullException(nameof(team.Id));
            }
            _ = team.TeamMembers ?? throw new ArgumentNullException(nameof(team.TeamMembers));

            _concernsDbContext.ConcernsTeamCaseworkTeam.Update(team);
            await _concernsDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<string[]> GetTeamOwners(CancellationToken cancellationToken)
        {
            return await _concernsDbContext.ConcernsTeamCaseworkTeam
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken);
        }
    }
}
