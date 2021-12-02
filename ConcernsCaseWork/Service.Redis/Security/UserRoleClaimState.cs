using System;
using System.Collections.Generic;

namespace Service.Redis.Security
{
	[Serializable]
	public sealed class UserRoleClaimState
	{
		public IDictionary<string, RoleClaimWrapper> ClaimRoles { get; } = new Dictionary<string, RoleClaimWrapper>();
	}
}