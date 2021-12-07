using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using Service.Redis.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	/// <summary>
	/// NOTE: Waiting for AD integration to adjust service accordingly
	/// </summary>
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

		public IList<string> GetDefaultUsers()
		{
			return _defaultUsers;
		}
		
		public async Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoles()
		{
			_logger.LogInformation("RbacManager::GetUsersRoles");
			
			var usersRoleClaim = await _userRoleCachedService.GetUsersRoleClaim(_defaultUsers);
			
			return new SortedDictionary<string, RoleClaimWrapper>(usersRoleClaim);
		}

		public async Task<RoleClaimWrapper> GetUserRoleClaimWrapper(string user)
		{
			_logger.LogInformation("RbacManager::GetUserRoleClaimWrapper {User}", user);

			var roleClaimWrapper = await _userRoleCachedService.GetRoleClaimWrapper(_defaultUsers, user);
			
			return roleClaimWrapper;
		}

		public async Task UpdateUserRoles(string user, IList<RoleEnum> roles, IList<string> users)
		{
			_logger.LogInformation("RbacManager::UpdateUserRoles {User}", user);
			
			await _userRoleCachedService.UpdateUserRoles(user, roles, users);
		}
	}
}