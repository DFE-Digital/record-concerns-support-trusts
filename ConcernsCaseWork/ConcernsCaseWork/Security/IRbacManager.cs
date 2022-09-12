using Service.Redis.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	public interface IRbacManager
	{
		Task<IList<string>> GetSystemUsers(params string[] excludes);
		//Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoles();
		//Task<RoleClaimWrapper> GetUserRoleClaimWrapper(string user);
	}
}