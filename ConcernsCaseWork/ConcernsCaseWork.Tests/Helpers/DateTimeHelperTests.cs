using ConcernsCaseWork.Helpers;
using NUnit.Framework;
using System;
using System.Globalization;

namespace ConcernsCaseWork.Tests.Helpers
{
	[Parallelizable(ParallelScope.All)]
	public class DateTimeHelperTests
	{
		[TestCase("07/04/2022", true)]
		[TestCase("29/02/2022", false)]
		public void WhenTryParseExact_ReturnsExpected(string dateString, bool expectedResult)
		{
			// act
			var result = DateTimeHelper.TryParseExact(dateString, out DateTime dt);

			//assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[TestCase("07/04/2022", 2022, 4, 7)]
		[TestCase("15/12/2021", 2022, 12, 15)]
		public void WhenParseExact_ReturnsExpected(string dateString, int expectedYear, int expectedMonth, int expectedDay)
		{
			// arrange
			var expectedResult = new DateTime(expectedYear, expectedMonth,expectedDay);
			
			// act
			var result = DateTimeHelper.ParseExact(dateString);

			//assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
