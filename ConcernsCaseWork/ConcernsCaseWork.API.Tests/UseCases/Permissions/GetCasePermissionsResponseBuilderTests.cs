using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.UseCases.Permissions;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ConcernsCaseWork.API.Tests.UseCases.Permissions
{
	public class GetCasePermissionsResponseBuilderTests
	{
		[Fact]
		public void When_CaseOwner_Then_HasEditPermission() 
		{
			var builder = new GetCasePermissionsResponseBuilder();

			var input = new GetCasePermissionsResponseBuilderParams()
			{
				CaseOwner = "Jim.Jones",
				Username = "Jim.Jones",
				IsAdmin = false
			};

			var result = builder.Build(input);

			result.Permissions.Should().BeEquivalentTo(new List<CasePermission>() { CasePermission.Edit });
		}

		[Fact]
		public void When_NotCaseOwner_Then_HasNoPermission()
		{
			var builder = new GetCasePermissionsResponseBuilder();

			var input = new GetCasePermissionsResponseBuilderParams()
			{
				CaseOwner = "Jim.Jones",
				Username = "Jayne.Jones",
				IsAdmin = false
			};

			var result = builder.Build(input);

			result.Permissions.Should().BeEmpty();
		}

		[Fact]
		public void When_NotCaseOwnerAndAdmin_Then_HasEditPermission()
		{
			var builder = new GetCasePermissionsResponseBuilder();

			var input = new GetCasePermissionsResponseBuilderParams()
			{
				CaseOwner = "Jim.Jones",
				Username = "Jayne.Jones",
				IsAdmin = true
			};

			var result = builder.Build(input);

			result.Permissions.Should().BeEquivalentTo(new List<CasePermission>() { CasePermission.Edit });
		}
	}
}
