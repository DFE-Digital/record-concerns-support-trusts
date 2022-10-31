using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Redis.Security
{
	[Serializable]
	public sealed class RoleClaimWrapper
	{
		public Claims Claims { get; set; }
		public IList<string> Users { get; set; } = new List<string>();
		public IList<RoleEnum> Roles { get; set; } = new List<RoleEnum>();
	}
}