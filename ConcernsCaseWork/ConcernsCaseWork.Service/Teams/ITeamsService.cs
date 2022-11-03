namespace ConcernsCaseWork.Service.Teams
{
	public interface ITeamsService
	{
		public Task<ConcernsCaseworkTeamDto> GetTeam(string ownerId);
		public Task PutTeam(ConcernsCaseworkTeamDto team);
		public Task<string[]> GetTeamOwners();
	}
}