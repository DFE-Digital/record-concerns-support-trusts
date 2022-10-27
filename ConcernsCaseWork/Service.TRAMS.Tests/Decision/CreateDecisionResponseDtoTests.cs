using AutoFixture;
using AutoFixture.Idioms;
using NUnit.Framework;
using Service.TRAMS.Decision;

namespace Service.TRAMS.Tests.Decision
{
	public class CreateDecisionResponseDtoTests
	{
		[Test]
		public void Can_Construct_CreateDecisionResponseDto()
		{
			var sut = new CreateDecisionResponseDto();
			Assert.That(sut, Is.Not.Null);
		}

		[Test]
		public void Constructors_Guard_Against_Null_Arguments()
		{
			var fixture = new Fixture();
			var assertion = fixture.Create<GuardClauseAssertion>();
			assertion.Verify(typeof(CreateDecisionResponseDto).GetConstructors());
		}

		[Test]
		public void Writable_Properties_Work_As_Expected()
		{
			// Arrange
			var fixture = new Fixture();
			var assertion = fixture.Create<WritablePropertyAssertion>();

			// Act & Assert
			assertion.Verify(typeof(CreateDecisionResponseDto));
		}
	}
}
