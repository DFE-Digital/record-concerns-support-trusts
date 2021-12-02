using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Users
{
	public sealed class UserRoleCachedService : CachedService, IUserRoleCachedService
	{
		private readonly IActiveDirectoryService _activeDirectoryService;
		private readonly ILogger<UserRoleCachedService> _logger;

		private const string UserRoleClaimKey = "Concerns.UserRoleClaim";
		
		public UserRoleCachedService(ICacheProvider cacheProvider, IActiveDirectoryService activeDirectoryService, ILogger<UserRoleCachedService> logger) : base(cacheProvider)
		{
			_activeDirectoryService = activeDirectoryService;
			_logger = logger;
		}
		
		public async Task ClearData()
		{
			await ClearData(UserRoleClaimKey);
		}
		
		/// <summary>
		/// TODO: Logic to be review when AD integration is done
		/// </summary>
		public async Task<Claims> GetUserClaims(UserCredentials userCredentials)
		{
			_logger.LogInformation("UserRoleCachedService::GetUserClaims {UserName} {Email}", userCredentials.UserName, userCredentials.Email);
			
			var userRoleClaimState = await GetData<UserRoleClaimState>(UserRoleClaimKey);
			if (userRoleClaimState != null 
			    && userRoleClaimState.UserRoleClaim.TryGetValue(userCredentials.UserName, out var roleClaimWrapper)
			    && roleClaimWrapper.Claims != null) return roleClaimWrapper.Claims;

			var userClaims = await _activeDirectoryService.GetUserAsync(userCredentials);
			
			userRoleClaimState ??= new UserRoleClaimState();

			if (userRoleClaimState.UserRoleClaim.TryGetValue(userCredentials.UserName, out roleClaimWrapper))
			{
				roleClaimWrapper.Claims = userClaims;
			}
			else
			{
				var defaultUserRoles = UserRoleMap.InitUserRoles();
				defaultUserRoles.TryGetValue(userCredentials.UserName, out var roles);
				roleClaimWrapper = new RoleClaimWrapper { Claims = userClaims, Roles = roles ?? UserRoleMap.DefaultUserRole() };
				userRoleClaimState.UserRoleClaim.Add(userCredentials.UserName, roleClaimWrapper);
			}
			
			await StoreData(UserRoleClaimKey, userRoleClaimState);
			
			return userClaims;
		}
		
		public async Task<IDictionary<string, RoleClaimWrapper>> GetUsersRoleClaim(string[] users) 
		{
			_logger.LogInformation("UserRoleCachedService::GetUsersRoleClaim");
			
			// Check if cache contains user roles
			var userRoleClaimState = await GetData<UserRoleClaimState>(UserRoleClaimKey);
			if (userRoleClaimState != null && userRoleClaimState.UserRoleClaim.Any())
				return userRoleClaimState.UserRoleClaim;
			
			// Init user roles
			var userRoles = UserRoleMap.InitUserRoles(users);
			
			// Store cache
			userRoleClaimState ??= new UserRoleClaimState();
			
			Parallel.ForEach(userRoles, userRole =>
			{
				(string key, List<RoleEnum> value) = userRole;
				userRoleClaimState.UserRoleClaim.Add(key, new RoleClaimWrapper { Roles = value });
			});
			
			await StoreData(UserRoleClaimKey, userRoleClaimState);
			
			return userRoleClaimState.UserRoleClaim;
		}

		public async Task<RoleClaimWrapper> GetRoleClaimWrapper(string[] users, string user)
		{
			_logger.LogInformation("UserRoleCachedService::GetUserRoleClaim {UserName}", user);
			
			var userRoleClaimState = await GetData<UserRoleClaimState>(UserRoleClaimKey) ?? new UserRoleClaimState { UserRoleClaim = await GetUsersRoleClaim(users) };
			if (userRoleClaimState.UserRoleClaim.TryGetValue(user, out var roleClaimWrapper)) 
			{
				if (roleClaimWrapper.Roles.Any())
					return roleClaimWrapper;
				
				roleClaimWrapper.Roles = UserRoleMap.DefaultUserRole();
			}
			else 
			{
				roleClaimWrapper = new RoleClaimWrapper { Roles = UserRoleMap.DefaultUserRole() };
				
				userRoleClaimState.UserRoleClaim.Add(user, roleClaimWrapper);
			}
			
			await StoreData(user, userRoleClaimState);
			
			return roleClaimWrapper;
		}

		public async Task UpdateUserRoles(string user, IList<RoleEnum> roles)
		{
			_logger.LogInformation("UserRoleCachedService::UpdateUserRoles {UserName}", user);
			
			var userRoleClaimState = await GetData<UserRoleClaimState>(UserRoleClaimKey);
			if (userRoleClaimState != null && userRoleClaimState.UserRoleClaim.TryGetValue(user, out var roleClaimWrapper))
			{
				roleClaimWrapper.Roles = roles;
				
				await StoreData(user, userRoleClaimState);
			}
		}
	}
}