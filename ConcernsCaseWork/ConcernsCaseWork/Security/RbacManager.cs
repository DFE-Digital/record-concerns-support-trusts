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

		public Task GetUserRoles(string user)
		{
			_logger.LogInformation("RbacManager::GetUserRoles {User}", user);
			
			throw new System.NotImplementedException();
		}
	}
}