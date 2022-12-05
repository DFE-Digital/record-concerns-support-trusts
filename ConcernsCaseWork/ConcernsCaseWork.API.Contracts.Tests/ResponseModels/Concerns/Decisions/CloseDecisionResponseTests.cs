using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using FluentAssertions.Execution;

namespace ConcernsCaseWork.API.Contracts.Tests.ResponseModels.Concerns.Decisions;

public class CloseDecisionResponseTests
{
	[Fact]
	public void All_Constructors_Guard_Against_Null()
	{
		// Arrange
		var fixture = new Fixture();
		using var scope = new AssertionScope();
		
		// Act & Assert
		var assertion = fixture.Create<GuardClauseAssertion>();

		assertion.Verify(typeof(CloseDecisionResponse).GetConstructors());
	}
	
	[Fact]
	public void Properties_Are_Initialized_By_Constructor()
	{
		// Arrange
		var fixture = new Fixture();
		using var scope = new AssertionScope();
		// Act & Assert
		var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();
		assertion.Verify(typeof(CloseDecisionResponse).GetConstructors());
	}

	[Fact]
	public void Property_Setters_Work_As_Expected()
	{
		// Arrange
		var fixture = new Fixture();
		using var scope = new AssertionScope();
		// Act & Assert
		var assertion = fixture.Create<WritablePropertyAssertion>();
		assertion.Verify(typeof(CloseDecisionResponse).GetConstructors());
	}
}