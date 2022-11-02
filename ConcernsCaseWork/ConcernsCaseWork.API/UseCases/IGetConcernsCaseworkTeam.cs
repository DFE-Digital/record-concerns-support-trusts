using ConcernsCaseWork.API.ResponseModels.Concerns.TeamCasework;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsCaseworkTeam
    {
        public Task<ConcernsCaseworkTeamResponse> Execute(string ownerId, CancellationToken cancellationToken);
    }
}
