using ConcernsCaseWork.Redis.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Users
{
	public interface IUserRoleCachedService
	{
		Task<Claims> GetUserClaims(UserCredentials userCredentials);
		Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoleClaim(string[] users);
		Task<RoleClaimWrapper> GetRoleClaimWrapper(string[] users, string user);
		Task UpdateUserRoles(string user, IList<RoleEnum> roles, IList<string> users);
		Task ClearData();
	}
}