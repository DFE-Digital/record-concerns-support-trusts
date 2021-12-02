using Service.Redis.Base;
using Service.Redis.Security;
using System.Threading.Tasks;

namespace Service.Redis.Users
{
	public sealed class UserRoleCachedService : CachedService, IUserRoleCachedService
	{
		private readonly IActiveDirectoryService _activeDirectoryService;

		private const string UserRoleClaimKey = "Concerns.UserRoleClaim";
		
		public UserRoleCachedService(IActiveDirectoryService activeDirectoryService, ICacheProvider cacheProvider) : base(cacheProvider)
		{
			_activeDirectoryService = activeDirectoryService;
		}
		
		/// <summary>
		/// TODO: Logic to be review when AD integration is done
		/// </summary>
		public async Task<Claims> GetUserClaimsAsync(UserCredentials userCredentials)
		{
			var userRoleClaimState = await GetData<UserRoleClaimState>(UserRoleClaimKey);
			if (userRoleClaimState != null 
			    && userRoleClaimState.ClaimRoles.TryGetValue(userCredentials.UserName, out var roleClaimWrapper) 
			    && roleClaimWrapper.Claims != null) return roleClaimWrapper.Claims;

			var userClaims = await _activeDirectoryService.GetUserAsync(userCredentials);
			
			userRoleClaimState ??= new UserRoleClaimState();

			if (userRoleClaimState.ClaimRoles.TryGetValue(userCredentials.UserName, out roleClaimWrapper))
			{
				roleClaimWrapper.Claims = userClaims;
			}
			else
			{
				var defaultUserRoles = UserRoleMap.InitUserRoles();
				defaultUserRoles.TryGetValue(userCredentials.UserName, out var roles);
				roleClaimWrapper = new RoleClaimWrapper { Claims = userClaims, Roles = roles ?? UserRoleMap.DefaultUserRole() };
				userRoleClaimState.ClaimRoles.Add(userCredentials.UserName, roleClaimWrapper);
			}
			
			await StoreData(UserRoleClaimKey, userRoleClaimState);
			
			return userClaims;
		}
		
		
	}
}