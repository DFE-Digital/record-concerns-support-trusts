namespace ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework
{
    public class ConcernsCaseworkTeamUpdateRequest
    {
        public string OwnerId { get; set; }

        public string[] TeamMembers { get; set; }
    }
}
