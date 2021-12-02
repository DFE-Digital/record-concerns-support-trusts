using System;
using System.Collections.Generic;

namespace Service.Redis.Security
{
	[Serializable]
	public sealed class RoleClaimWrapper
	{
		public Claims Claims { get; set; }
		public List<RoleEnum> Roles { get; set; } = new List<RoleEnum>();
	}
}