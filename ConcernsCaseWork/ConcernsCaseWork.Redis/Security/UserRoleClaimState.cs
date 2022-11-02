using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConcernsCaseWork.Redis.Security
{
	[Serializable]
	public sealed class UserRoleClaimState
	{
		public IDictionary<string, RoleClaimWrapper> UserRoleClaim { get; set; } = new ConcurrentDictionary<string, RoleClaimWrapper>();
	}
}