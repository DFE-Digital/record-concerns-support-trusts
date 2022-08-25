using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.Models.Teams;
using NUnit.Framework;
using System;

namespace ConcernsCaseWork.Tests.Models.Teams
{
	public class ConcernsTeamCaseworkModelTests
	{
		[Test]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(ConcernsTeamCaseworkModel).GetConstructors());
		}

		[Test]
		public void Properties_Are_Initialized_By_Constructor()
		{
			// Arrange
			var fixture = new Fixture();
			var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();

			// Act & Assert
			assertion.Verify(typeof(ConcernsTeamCaseworkModel));
		}

		[Test]
		public void Constructor_Allows_Empty_TeamsMebers()
		{
			// Arrange
			var sut = new ConcernsTeamCaseworkModel("john.smith", Array.Empty<string>());
			Assert.NotNull(sut);
			Assert.That(sut.TeamMembers.Length, Is.Zero);
		}
	}
}
