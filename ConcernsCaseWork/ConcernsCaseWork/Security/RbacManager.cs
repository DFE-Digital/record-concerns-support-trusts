using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using Service.Redis.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	public sealed class RbacManager : IRbacManager
	{
		private readonly IUserRoleCachedService _userRoleCachedService;
		private readonly ILogger<RbacManager> _logger;

		private readonly string[] _defaultUsers;

		public RbacManager(IConfiguration configuration, IUserRoleCachedService userRoleCachedService, ILogger<RbacManager> logger)
		{
			_userRoleCachedService = userRoleCachedService;
			_logger = logger;
			
			// set hardcoded users from configuration
			// update / remove when AD integration
			_defaultUsers = configuration["app:username"].Split(',');
		}

		public async Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoles()
		{
			_logger.LogInformation("RbacManager::GetUsersRoles");
			
			var usersRoleClaim = await _userRoleCachedService.GetUsersRoleClaim(_defaultUsers);
			
			return usersRoleClaim;
		}

		public async Task<IList<RoleEnum>> GetUserRoles(string user)
		{
			_logger.LogInformation("RbacManager::GetUserRoles {User}", user);

			var userRoles = await _userRoleCachedService.GetRoleClaimWrapper(_defaultUsers, user);
			
			return userRoles.Roles;
		}

		public async Task UpdateUserRoles(string user, IList<RoleEnum> roles)
		{
			_logger.LogInformation("RbacManager::UpdateUserRoles {User}", user);
			
			await _userRoleCachedService.UpdateUserRoles(user, roles);
		}
	}
}