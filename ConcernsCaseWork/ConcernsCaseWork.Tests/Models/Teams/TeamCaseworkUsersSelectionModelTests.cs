using AutoFixture.Idioms;
using AutoFixture;
using NUnit.Framework;
using Service.TRAMS.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.Teams;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ConcernsCaseWork.Tests.Models.Teams
{
	public class TeamCaseworkUsersSelectionModelTests
	{
		[Test]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(TeamCaseworkUsersSelectionModel).GetConstructors());
		}

		[Test]
		public void Properties_Are_Initialized_By_Constructor()
		{
			// Arrange
			var fixture = new Fixture();
			var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();

			// Act & Assert
			assertion.Verify(typeof(TeamCaseworkUsersSelectionModel));
		}
	}
}
