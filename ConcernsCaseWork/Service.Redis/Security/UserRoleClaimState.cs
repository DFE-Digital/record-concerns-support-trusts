using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Service.Redis.Security
{
	[Serializable]
	public sealed class UserRoleClaimState
	{
		public IDictionary<string, RoleClaimWrapper> UserRoleClaim { get; } = new ConcurrentDictionary<string, RoleClaimWrapper>();
	}
}