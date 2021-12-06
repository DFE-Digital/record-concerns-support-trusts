using Service.Redis.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	public interface IRbacManager
	{
		IList<string> GetDefaultUsers();
		Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoles();
		Task<RoleClaimWrapper> GetUserRoleClaimWrapper(string user);
		Task UpdateUserRoles(string user, IList<RoleEnum> roles, IList<string> users);
	}
}