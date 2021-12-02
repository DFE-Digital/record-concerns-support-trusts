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
		private readonly IConfiguration _configuration;
		private readonly ILogger<RbacManager> _logger;

		public RbacManager(IConfiguration configuration, IUserRoleCachedService userRoleCachedService, ILogger<RbacManager> logger)
		{
			_userRoleCachedService = userRoleCachedService;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoles()
		{
			_logger.LogInformation("RbacManager::GetUsersRoles");
			
			var defaultUsers = _configuration["app:username"].Split(',');
			var defaultUserRoleClaim = new Dictionary<string, RoleClaimWrapper>();
			
			if (defaultUsers.Length == 0) return defaultUserRoleClaim;
			
			var usersRoleClaim = await _userRoleCachedService.GetUsersRoleClaim(defaultUsers);
			
			return usersRoleClaim;
		}

		public async Task<IList<RoleEnum>> GetUserRoles(string user)
		{
			_logger.LogInformation("RbacManager::GetUserRoles {User}", user);

			var userRoles = await _userRoleCachedService.GetUserRoleClaim(user);
			
			return userRoles.Roles;
		}

		public async Task UpdateUserRoles(string user, IList<RoleEnum> roles)
		{
			_logger.LogInformation("RbacManager::UpdateUserRoles {User}", user);
			
			await _userRoleCachedService.UpdateUserRoles(user, roles);
		}
	}
}