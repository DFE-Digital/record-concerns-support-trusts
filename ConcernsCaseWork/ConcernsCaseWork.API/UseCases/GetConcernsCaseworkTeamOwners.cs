using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases
{
    public class GetConcernsCaseworkTeamOwners : IGetConcernsCaseworkTeamOwners
    {
        private readonly IConcernsTeamCaseworkGateway _gateway;

        public GetConcernsCaseworkTeamOwners(IConcernsTeamCaseworkGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }
        public async Task<string[]> Execute(CancellationToken cancellationToken)
        {
            return await _gateway.GetTeamOwners(cancellationToken);
        }
    }
}
