using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using Service.Redis.Teams;
using Service.Redis.Users;
using System.Collections.Generic;
using System.Linq;
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
		private readonly ITeamsCachedService _teamsService;

		private readonly string[] _defaultUsers;

		public RbacManager(IConfiguration configuration, IUserRoleCachedService userRoleCachedService, ILogger<RbacManager> logger, ITeamsCachedService teamsService)
		{
			_userRoleCachedService = Guard.Against.Null(userRoleCachedService);
			_logger = Guard.Against.Null(logger);
			_teamsService = Guard.Against.Null(teamsService);

			// set hardcoded users from configuration
			// update / remove when AD integration
			// _defaultUsers = configuration["app:username"].Split(',');
		}

		public async Task<IList<string>> GetSystemUsers(params string[] excludes)
		{
			_logger.LogMethodEntered();
			var users = await _teamsService.GetTeamOwners();
			return users.Except(excludes).ToArray();
		}
		
		//public async Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoles()
		//{
		//	_logger.LogInformation("RbacManager::GetUsersRoles");
			
		//	var usersRoleClaim = await _userRoleCachedService.GetUsersRoleClaim(_defaultUsers);
			
		//	return new SortedDictionary<string, RoleClaimWrapper>(usersRoleClaim);
		//}

		//public async Task<RoleClaimWrapper> GetUserRoleClaimWrapper(string user)
		//{
		//	_logger.LogInformation("RbacManager::GetUserRoleClaimWrapper {User}", user);

		//	var roleClaimWrapper = await _userRoleCachedService.GetRoleClaimWrapper(_defaultUsers, user);
			
		//	return roleClaimWrapper;
		//}
	}
}