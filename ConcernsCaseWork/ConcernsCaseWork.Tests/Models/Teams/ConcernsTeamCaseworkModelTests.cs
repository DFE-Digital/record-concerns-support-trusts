using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.Models.Teams;
using NUnit.Framework;

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
	}
}
