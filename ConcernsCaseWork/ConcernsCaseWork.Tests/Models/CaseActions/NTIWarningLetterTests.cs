﻿using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models
{
	[Parallelizable(ParallelScope.All)]
	public class NTIWarningLetterTests
	{
		[Test]
		[TestCase("05/10/2022", false)]
		//[TestCase(new DateTime(2022, 10, 5), false)]
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