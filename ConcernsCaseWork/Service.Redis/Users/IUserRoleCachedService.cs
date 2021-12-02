using Service.Redis.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Users
{
	public interface IUserRoleCachedService
	{
		Task<Claims> GetUserClaims(UserCredentials userCredentials);
		Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoleClaim(string[] users);
		Task<RoleClaimWrapper> GetUserRoleClaim(string user);
		Task UpdateUserRoles(string user, IList<RoleEnum> roles);
		Task ClearData();
	}
}