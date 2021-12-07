using AutoFixture;
using Service.Redis.Security;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class RoleFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static List<RoleEnum> BuildListRoleEnum()
		{
			return new List<RoleEnum> { RoleEnum.Admin, RoleEnum.Leader, RoleEnum.User };
		}

		public static List<RoleEnum> BuildPartialListRoleEnum()
		{
			return new List<RoleEnum> { RoleEnum.Leader, RoleEnum.User };
		}
		
		public static List<RoleEnum> BuildListUserRoleEnum()
		{
			return new List<RoleEnum> { RoleEnum.User };
		}
		
		public static IDictionary<string, RoleClaimWrapper> BuildDicUsersRoles()
		{
			return new Dictionary<string, RoleClaimWrapper>
			{
				{ Fixture.Create<string>(), Fixture.Create<RoleClaimWrapper>() }
			};
		}
	}
}