using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCasework.Service.Teams;

namespace ConcernsCaseWork.Service.Tests.Teams
{
	public class ConcernsCaseworkTeamDtoTests
	{
		[Test]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(ConcernsCaseworkTeamDto).GetConstructors());
		}

		[Test]
		public void Properties_Are_Initialized_By_Constructor()
		{
			// Arrange
			var fixture = new Fixture();
			var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();

			// Act & Assert
			assertion.Verify(typeof(ConcernsCaseworkTeamDto));
		}
	}
}
