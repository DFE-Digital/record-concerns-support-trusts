using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using NUnit.Framework;
using System;

namespace ConcernsCaseWork.Tests.Models.CaseActions;

[Parallelizable(ParallelScope.All)]
public class CaseActionModelTests
{
	private readonly IFixture _fixture;

	public CaseActionModelTests()
	{
		_fixture = new Fixture();
	}
	
	[Test]
	public void WhenClosedAtIsNull_IsClosedIsFalse()
	{
		// arrange
		var caseActionModel = new CaseActionModel { ClosedAt = null };

		// act
		var result = caseActionModel.IsClosed;
			
		// assert
		Assert.That(result, Is.False);
	}
	
	[Test]
	public void WhenClosedAtIsNotNull_IsClosedIsTrue()
	{
		// arrange
		var closedAt = _fixture.Build<DateTime?>().Create();
		var caseActionModel = new CaseActionModel { ClosedAt = closedAt };

		// act
		var result = caseActionModel.IsClosed;
			
		// assert
		Assert.That(result, Is.True);
	}
	
	[Test]
	public void WhenClosedAtIsNull_IsOpenIsTrue()
	{
		// arrange
		var caseActionModel = new CaseActionModel { ClosedAt = null };

		// act
		var result = caseActionModel.IsOpen;
			
		// assert
		Assert.That(result, Is.True);
	}
	
	[Test]
	public void WhenClosedAtIsNotNull_IsOpenIsFalse()
	{
		// arrange
		var closedAt = _fixture.Build<DateTime?>().Create();
		var caseActionModel = new CaseActionModel { ClosedAt = closedAt };

		// act
		var result = caseActionModel.IsOpen;
			
		// assert
		Assert.That(result, Is.False);
	}
}