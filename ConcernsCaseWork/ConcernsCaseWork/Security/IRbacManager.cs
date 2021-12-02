using Service.Redis.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	public interface IRbacManager
	{
		Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoles();
		Task GetUserRoles(string user);
	}
}