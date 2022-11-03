using ConcernsCaseWork.Models.CaseActions;
using NUnit.Framework;
using System;

namespace ConcernsCaseWork.Tests.Models.CaseActions
{
	[Parallelizable(ParallelScope.All)]
	public class NTIWarningLetterTests
	{
		[Test]
		[TestCase("05/10/2022", false)]
		[TestCase(null, true)]
		public void WhenBuildNTIWarningLetter_IsClosed_ReturnsValidLogic(DateTime? closedAt, bool expectedResulted)
		{
			// arrange
			var ntiWarningLetter = new NtiWarningLetterModel();
			ntiWarningLetter.ClosedAt = closedAt;

			// act
			var result = ntiWarningLetter.CanBeEdited();
			
			// assert
			Assert.That(result, Is.EqualTo(expectedResulted));
		}
	}
}