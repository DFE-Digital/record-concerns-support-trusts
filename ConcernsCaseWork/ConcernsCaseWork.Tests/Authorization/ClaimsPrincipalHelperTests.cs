using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.Authorization;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Principal;

namespace ConcernsCaseWork.Tests.Authorization
{
	public class ClaimsPrincipalHelperTests
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

		[Test]
		public void Given_Null_Claim_Principal_When_GetName_Then_Returns_UserName()
		{
			var mockPrincipal = new Mock<IPrincipal>();
			mockPrincipal.SetupGet(x => x.Identity.Name).Returns(default(string));

			var sut = new ClaimsPrincipalHelper();

			Assert.Throws<ArgumentNullException>(() => sut.GetPrincipalName(null));
			Assert.Throws<ArgumentNullException>(() => sut.GetPrincipalName(mockPrincipal.Object));
		}

		[Test]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			// Arrange
			var fixture = new Fixture();
			fixture.Register(Mock.Of<IPrincipal>);
			var assertion = fixture.Create<GuardClauseAssertion>();
   
			// Act & Assert
			assertion.Verify(typeof(ClaimsPrincipalHelper).GetConstructors());
		}

		[Test]
		public void Methods_GuardAgainstNullArgs()
		{
			// Arrange
			var fixture = new Fixture();
			fixture.Register(Mock.Of<IPrincipal>);
			var assertion = fixture.Create<GuardClauseAssertion>();

			// Act & Assert
			assertion.Verify(typeof(ClaimsPrincipalHelper).GetMethods());
		}
		
		[Theory]
		[TestCase(true)]
		[TestCase(false)]
		public void Given_CaseWorker_Role_When_IsCaseWorker_Then_Returns_Expectation(bool expectation)
		{
			var mockPrincipal = new Mock<IPrincipal>();
			mockPrincipal.Setup(x => x.IsInRole(ClaimsPrincipalHelper.CaseWorkerRole)).Returns(expectation);

			var sut = new ClaimsPrincipalHelper();
			var actual = sut.IsCaseworker(mockPrincipal.Object);

			Assert.AreEqual(expectation, actual);
		}

		[Theory]
		[TestCase(true)]
		[TestCase(false)]
		public void Given_IsTeamLeader_Role_When_IsCaseWorker_Then_Returns_Expectation(bool expectation)
		{
			var mockPrincipal = new Mock<IPrincipal>();
			mockPrincipal.Setup(x => x.IsInRole(ClaimsPrincipalHelper.TeamLeaderRole)).Returns(expectation);

			var sut = new ClaimsPrincipalHelper();
			var actual = sut.IsTeamLeader(mockPrincipal.Object);

			Assert.AreEqual(expectation, actual);
		}

		[Theory]
		[TestCase(true)]
		[TestCase(false)]
		public void Given_IsAdmin_Role_When_IsCaseWorker_Then_Returns_Expectation(bool expectation)
		{
			var mockPrincipal = new Mock<IPrincipal>();
			mockPrincipal.Setup(x => x.IsInRole(ClaimsPrincipalHelper.AdminRole)).Returns(expectation);

			var sut = new ClaimsPrincipalHelper();
			var actual = sut.IsAdmin(mockPrincipal.Object);

			Assert.AreEqual(expectation, actual);
		}
	}
}
