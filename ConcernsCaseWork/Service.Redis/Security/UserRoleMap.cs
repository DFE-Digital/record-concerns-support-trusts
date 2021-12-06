using System;
using System.Collections.Generic;

namespace Service.Redis.Security
{
	/// <summary>
	/// NOTE: Replace / review this class when AD is integrated.
	/// </summary>
	public static class UserRoleMap
	{
		private readonly static IDictionary<string, List<RoleEnum>> DefaultUserRoles = new Dictionary<string, List<RoleEnum>>
		{
			{ "concerns.casework", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "ben.memmott", new List<RoleEnum> { RoleEnum.User } },
			{ "richard.machen", new List<RoleEnum> { RoleEnum.User } },
			{ "steve.oconnor", new List<RoleEnum> { RoleEnum.User } },
			{ "elijah.aremu", new List<RoleEnum> { RoleEnum.User } },
			{ "paulo.lancao", new List<RoleEnum> { RoleEnum.User } },
			{ "richard.joseph", new List<RoleEnum> { RoleEnum.User } },
			{ "william.cook", new List<RoleEnum> { RoleEnum.User } },
			{ "stephanie.maskery", new List<RoleEnum> { RoleEnum.User } }
		};

		public const string AdminUserName = "concerns.casework";
		
		public static List<RoleEnum> DefaultUserRole()
		{
			return new List<RoleEnum> { RoleEnum.User };
		}
		
		public static IDictionary<string, List<RoleEnum>> InitUserRoles(string[] users = null)
		{
			if (users is null || users.Length == 0) return DefaultUserRoles;

			var usersRoles = new Dictionary<string, List<RoleEnum>>();
			foreach (var user in users)
			{
				if (user.Equals(AdminUserName, StringComparison.OrdinalIgnoreCase))
				{
					usersRoles.Add(user, new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader });
					continue;
				}
				
				usersRoles.Add(user, DefaultUserRole());
			}
			
			return usersRoles;
		}
	}
}