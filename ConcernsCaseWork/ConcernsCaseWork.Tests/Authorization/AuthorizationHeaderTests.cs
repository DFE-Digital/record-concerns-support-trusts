using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.UserContext;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Principal;

namespace ConcernsCaseWork.Tests.Authorization;

public class AuthorizationHeaderTests
{
	[Test]
	public void Given_Valid_Claim_Principal_When_GetName_Then_Returns_UserName()
	{
		var mockPrincipal = new Mock<IPrincipal>();
		string expectedName = Guid.NewGuid().ToString();
		mockPrincipal.SetupGet(x => x.Identity.Name).Returns(expectedName);

		var sut = new ClaimsPrincipalHelper();
		var actualName = sut.GetPrincipalName(mockPrincipal.Object);

		Assert.AreEqual(expectedName, actualName);
	}
}