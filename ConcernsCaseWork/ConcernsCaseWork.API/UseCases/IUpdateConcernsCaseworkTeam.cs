using ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework;
using ConcernsCaseWork.API.ResponseModels.Concerns.TeamCasework;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IUpdateConcernsCaseworkTeam
    {
        public Task<ConcernsCaseworkTeamResponse> Execute(ConcernsCaseworkTeamUpdateRequest updateRequest, CancellationToken cancellationToken);
    }
}
