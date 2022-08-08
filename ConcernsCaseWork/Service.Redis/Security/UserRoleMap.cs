using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Redis.Security
{
	/// <summary>
	/// NOTE: Replace / review this class when AD is integrated.
	/// </summary>
	public static class UserRoleMap
	{
		public const string AdminUserName = "concerns.casework";
		private const string UserE2ECypressUserName = "e2e.cypress.test";
		private static readonly IList<string> ToExcludeUsers = new List<string> { AdminUserName, UserE2ECypressUserName };
		
		private readonly static IDictionary<string, List<RoleEnum>> DefaultUserRoles = new Dictionary<string, List<RoleEnum>>
		{
			{ AdminUserName, new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ UserE2ECypressUserName, new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "ben.memmott", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "richard.machen", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "elijah.aremu", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "paul.simmons", new List<RoleEnum> { RoleEnum.User } },
			{ "menol.razeek", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "emma.whitcroft", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "chris.dexter", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "anthon.thomas", new List<RoleEnum> { RoleEnum.User, RoleEnum.Admin, RoleEnum.Leader } },
			{ "chanel.diep", new List<RoleEnum> { RoleEnum.User, RoleEnum.Leader } },
			{ "israt.choudhury", new List<RoleEnum> { RoleEnum.User } },
			{ "magdalena.bober", new List<RoleEnum> { RoleEnum.User } },
			{ "jane.dickinson", new List<RoleEnum> { RoleEnum.User } },
			{ "elaine.martel", new List<RoleEnum> { RoleEnum.User } },
			{ "case.worker1", new List<RoleEnum> { RoleEnum.User } },
			{ "case.worker2", new List<RoleEnum> { RoleEnum.User } },
			{ "james.cheetham", new List<RoleEnum> { RoleEnum.User } },
			{ "christian.gleadall", new List<RoleEnum> { RoleEnum.User } },
			{ "philip.pybus", new List<RoleEnum> { RoleEnum.User } },
			{ "emma.wadsworth", new List<RoleEnum> { RoleEnum.User } }
		};
		
		public static List<RoleEnum> DefaultUserRole()
		{
			return new List<RoleEnum> { RoleEnum.User };
		}
		
		public static IDictionary<string, List<RoleEnum>> InitUserRoles()
		{
			return DefaultUserRoles;
		}

		public static IList<string> GetDefaultUsersExcludedList(IEnumerable<string> users)
		{
			return users.Where(user => !ToExcludeUsers.Contains(user)).ToList();
		}

		public static IList<string> GetDefaultUsersExcludeE2E(IEnumerable<string> users)
		{
			return users.Where(user => !user.Equals(UserE2ECypressUserName, StringComparison.OrdinalIgnoreCase)).ToList();
		}
	}
}