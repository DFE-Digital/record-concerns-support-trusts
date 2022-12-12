using ConcernsCaseWork.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System;

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
		[TestCase("15/12/2021", 2021, 12, 15)]
		public void WhenParseExact_ReturnsExpected(string dateString, int expectedYear, int expectedMonth, int expectedDay)
		{
			// arrange
			var expectedResult = new DateTime(expectedYear, expectedMonth,expectedDay);
			
			// act
			var result = DateTimeHelper.ParseExact(dateString);

			//assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[TestCase("16 December 2022", 2022, 12, 16)]
		[TestCase("22 May 2022", 2022, 5, 22)]
		public void WhenParseToDisplayDate_ReturnsExpected(string dateString, int expectedYear, int expectedMonth, int expectedDay)
		{
			// arrange
			var date = new DateTime(expectedYear, expectedMonth, expectedDay);

			// act
			var result = DateTimeHelper.ParseToDisplayDate(date);

			//assert 
			result.Should().Be(dateString);
		}
	}
}
