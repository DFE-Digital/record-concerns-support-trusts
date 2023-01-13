using ConcernsCaseWork.API.Contracts.Context;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Tests.Context
{
	public class UserContextTests
	{
		[Fact]
		public void ToHeadersKVP_When_NameSet_Returns_Name_Header()
		{
			var sut = new UserContext();
			sut.Name = Guid.NewGuid().ToString();
			sut.Roles = new[]
			{
				UserContext.CaseWorkerRoleClaim,
				UserContext.TeamLeaderRoleClaim,
				UserContext.AdminRoleClaim,
				"x-filtered-out-role"
			};

			var results = sut.ToHeadersKVP();
			var nameResult = results.FirstOrDefault(x => x.Key == "x-userContext-name");
			nameResult.Value.Should().Be(sut.Name);
		}

		[Fact]
		public void ToHeadersKVP_When_Name_And_Roles_Set_Returns_Role_Headers()
		{
			var sut = new UserContext();
			sut.Name = Guid.NewGuid().ToString();
			sut.Roles = new[]
			{
				UserContext.CaseWorkerRoleClaim,
				UserContext.TeamLeaderRoleClaim,
				UserContext.AdminRoleClaim
			};

			var results = sut.ToHeadersKVP();
			var roleResults = results.Where(x => x.Key.StartsWith("x-userContext-role-")).ToArray();
			roleResults.Length.Should().Be(3);

			roleResults[0].Key.Should().Be("x-userContext-role-0");
			roleResults[0].Value.Should().Be(UserContext.CaseWorkerRoleClaim);
			roleResults[1].Key.Should().Be("x-userContext-role-1");
			roleResults[1].Value.Should().Be(UserContext.TeamLeaderRoleClaim);
			roleResults[2].Key.Should().Be("x-userContext-role-2");
			roleResults[2].Value.Should().Be(UserContext.AdminRoleClaim);
		}

		[Fact]
		public void ParseRoleClaims_When_Claims_Filters_To_Only_Concerns_Roles()
		{
			var sut = new UserContext();
			sut.Name = Guid.NewGuid().ToString();
			var claims = new[]
			{
				UserContext.CaseWorkerRoleClaim,
				UserContext.TeamLeaderRoleClaim,
				UserContext.AdminRoleClaim,
				"x-filtered-out-role"
			};

			var results = UserContext.ParseRoleClaims(claims);
			results.Length.Should().Be(3);

			results[0].Should().Be(UserContext.CaseWorkerRoleClaim);
			results[1].Should().Be(UserContext.TeamLeaderRoleClaim);
			results[2].Should().Be(UserContext.AdminRoleClaim);
		}
	}
}
