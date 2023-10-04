using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.TeamCasework
{
	public class ConcernsCaseworkTeamResponse
	{
		public string OwnerId { get; set; }

		public string[] TeamMembers { get; set; }
	}
}
