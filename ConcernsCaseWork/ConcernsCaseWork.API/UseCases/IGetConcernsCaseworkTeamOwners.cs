namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsCaseworkTeamOwners
    {
        public Task<string[]> Execute(CancellationToken cancellationToken);
    }
}
