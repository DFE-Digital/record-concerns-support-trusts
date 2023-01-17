using ConcernsCaseWork.UserContext;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Tests.Context
{
	public class UserInfoTests
	{
		[Fact]
		public void ToHeadersKVP_When_NameSet_Returns_Name_Header()
		{
			var sut = new UserInfo();
			sut.Name = Guid.NewGuid().ToString();
			sut.Roles = new[]
			{
				UserInfo.CaseWorkerRoleClaim,
				UserInfo.TeamLeaderRoleClaim,
				UserInfo.AdminRoleClaim,
				"x-filtered-out-role"
			};

			var results = sut.ToHeadersKVP();
			var nameResult = results.FirstOrDefault(x => x.Key == "x-userContext-name");
			nameResult.Value.Should().Be(sut.Name);
		}

		[Fact]
		public void ToHeadersKVP_When_Name_And_Roles_Set_Returns_Role_Headers()
		{
			var sut = new UserInfo();
			sut.Name = Guid.NewGuid().ToString();
			sut.Roles = new[]
			{
				UserInfo.CaseWorkerRoleClaim,
				UserInfo.TeamLeaderRoleClaim,
				UserInfo.AdminRoleClaim
			};

			var results = sut.ToHeadersKVP();
			var roleResults = results.Where(x => x.Key.StartsWith("x-userContext-role-")).ToArray();
			roleResults.Length.Should().Be(3);

			roleResults[0].Key.Should().Be("x-userContext-role-0");
			roleResults[0].Value.Should().Be(UserInfo.CaseWorkerRoleClaim);
			roleResults[1].Key.Should().Be("x-userContext-role-1");
			roleResults[1].Value.Should().Be(UserInfo.TeamLeaderRoleClaim);
			roleResults[2].Key.Should().Be("x-userContext-role-2");
			roleResults[2].Value.Should().Be(UserInfo.AdminRoleClaim);
		}

		[Fact]
		public void ParseRoleClaims_When_Claims_Filters_To_Only_Concerns_Roles()
		{
			var sut = new UserInfo();
			sut.Name = Guid.NewGuid().ToString();
			var claims = new[]
			{
				UserInfo.CaseWorkerRoleClaim,
				UserInfo.TeamLeaderRoleClaim,
				UserInfo.AdminRoleClaim,
				"x-filtered-out-role"
			};

			var results = UserInfo.ParseRoleClaims(claims);
			results.Length.Should().Be(3);

			results[0].Should().Be(UserInfo.CaseWorkerRoleClaim);
			results[1].Should().Be(UserInfo.TeamLeaderRoleClaim);
			results[2].Should().Be(UserInfo.AdminRoleClaim);
		}

		[Fact]
		public void FromHeaders_Returns_Populated_UserInfo()
		{
			var inputs = new KeyValuePair<string,string>[]
			{
				new("x-userContext-name" ,"John"),
				new("x-userContext-role-0", UserInfo.CaseWorkerRoleClaim),
				new("x-userContext-role-1", UserInfo.TeamLeaderRoleClaim),
				new("x-userContext-role-2", UserInfo.AdminRoleClaim)
			};

			var sut = UserInfo.FromHeaders(inputs);

			sut.Name.Should().Be("John");
			sut.Roles.Should().Contain(UserInfo.CaseWorkerRoleClaim);
			sut.Roles.Should().Contain(UserInfo.TeamLeaderRoleClaim);
			sut.Roles.Should().Contain(UserInfo.AdminRoleClaim);
		}
	}
}
