using AutoFixture;
using AutoFixture.Idioms;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using FluentAssertions;
using FluentAssertions.Execution;

namespace ConcernsCaseWork.API.Contracts.Tests.RequestModels.Concerns.Decisions;

public class CloseDecisionRequestTests
{
	[Fact]
	public void All_Constructors_Guard_Against_Null()
	{
		// Arrange
		var fixture = new Fixture();
		using var scope = new AssertionScope();
		
		// Act & Assert
		var assertion = fixture.Create<GuardClauseAssertion>();

		assertion.Verify(typeof(CloseDecisionRequest).GetConstructors());
	}
	
	[Theory]
	[InlineData(2001)]
	[InlineData(9999999)]
	public void IsValid_When_NotesTooLong_Returns_False(int stringLength)
	{
		var fixture = new Fixture();
		var sut = fixture.Build<CloseDecisionRequest>()
			.With(x => x.SupportingNotes, "too long string".PadLeft(stringLength, 'a'))
			.Create();

		sut.IsValid().Should().BeFalse();
	}
		
	[Theory]
	[InlineData(2000)]
	[InlineData(1)]
	[InlineData(50)]
	[InlineData(0)]
	public void IsValid_When_NotesLongEnough_Returns_True(int stringLength)
	{
		var fixture = new Fixture();
		var sut = fixture.Build<CloseDecisionRequest>()
			.With(x => x.SupportingNotes, "long enough string".PadLeft(stringLength, 'a'))
			.Create();

		sut.IsValid().Should().BeTrue();
	}
		
	[Fact]
	public void IsValid_When_NotesNull_Returns_True()
	{
		var fixture = new Fixture();
		string? testString = null;
			
		var sut = fixture.Build<CloseDecisionRequest>()
			.With(x => x.SupportingNotes, testString)
			.Create();

		sut.IsValid().Should().BeTrue();
	}
		
	[Fact]
	public void Properties_Are_Initialized_By_Constructor()
	{
		// Arrange
		var fixture = new Fixture();
		using var scope = new AssertionScope();
		// Act & Assert
		var assertion = fixture.Create<ConstructorInitializedMemberAssertion>();
		assertion.Verify(typeof(CloseDecisionRequest).GetConstructors());
	}

	[Fact]
	public void Property_Setters_Work_As_Expected()
	{
		// Arrange
		var fixture = new Fixture();
		using var scope = new AssertionScope();
		// Act & Assert
		var assertion = fixture.Create<WritablePropertyAssertion>();
		assertion.Verify(typeof(CloseDecisionRequest).GetConstructors());
	}
}