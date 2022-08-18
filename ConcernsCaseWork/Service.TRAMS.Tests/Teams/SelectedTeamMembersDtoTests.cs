using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.Models.Teams;
using NUnit.Framework;
using Service.TRAMS.Teams;

namespace Service.TRAMS.Tests.Teams
{
	public class SelectedTeamMembersDtoTests
	{
		[Test]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamCaseworkUsersSelectionDto).GetConstructors());
		}

		[Test]
		public void Properties_Are_Initialized_By_Constructor()
		{
			// Arrange
			var fixture = new Fixture();
			var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();

			// Act & Assert
			assertion.Verify(typeof(TeamCaseworkUsersSelectionDto));
		}
	}
}
