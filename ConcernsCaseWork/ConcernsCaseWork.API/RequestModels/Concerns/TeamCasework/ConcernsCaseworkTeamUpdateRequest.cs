using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework
{
    public class ConcernsCaseworkTeamUpdateRequest
    {
	    [MaxLength(300)]
        public string OwnerId { get; set; }

        public string[] TeamMembers { get; set; }
    }
}
