using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework
{
    public class ConcernsCaseworkTeamUpdateRequest
    {
	    [StringLength(300)]
        public string OwnerId { get; set; }

        public string[] TeamMembers { get; set; }
    }
}
