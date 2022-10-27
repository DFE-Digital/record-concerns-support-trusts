using ConcernsCaseWork.Extensions;
using NUnit.Framework;
using System;

namespace ConcernsCaseWork.Tests.Extensions
{
	[Parallelizable(ParallelScope.All)]
	public class DateExtensionTests
	{
		[TestCase(7,7,2021, "7 July 2021")]
		[TestCase(16, 11,2021, "16 November 2021")]
		[TestCase(1, 8,2022, "1 August 2022")]
		public void WhenToUserFriendlyDate_ReturnsExpected(int day, int month, int year, string expected)
		{
			// arrange
			var date = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
			
			// act
			var result = date.ToUserFriendlyDate();
			
			// assert
			Assert.That(result, Is.EqualTo(expected));
		}
		
		[TestCase(7,7,2021, "07-07-2021")]
		[TestCase(16, 11,2021, "16-11-2021")]
		[TestCase(1, 8,2022, "01-08-2022")]
		public void WhenToDayMonthYear_DateTimeOffset_ReturnsExpected(int day, int month, int year, string expected)
		{
			// arrange
			var date = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
			
			// act
			var result = date.ToDayMonthYear();
			
			// assert
			Assert.That(result, Is.EqualTo(expected));
		}
				
		[TestCase(7,7,2021, "07-07-2021")]
		[TestCase(16, 11,2021, "16-11-2021")]
		[TestCase(1, 8,2022, "01-08-2022")]
		public void WhenToDayMonthYear_DateTime_ReturnsExpected(int day, int month, int year, string expected)
		{
			// arrange
			var date = new DateTime(year, month, day, 0, 0, 0);
			
			// act
			var result = date.ToDayMonthYear();
			
			// assert
			Assert.That(result, Is.EqualTo(expected));
		}
		
		[TestCase(7,7,2021, "07-07-2021")]
		[TestCase(16, 11,2021, "16-11-2021")]
		[TestCase(1, 8,2022, "01-08-2022")]
		public void WhenToDayMonthYear_NullableDateTime_ReturnsExpected(int day, int month, int year, string expected)
		{
			// arrange
			DateTime? date = new DateTime(year, month, day, 0, 0, 0);
			
			// act
			var result = date.ToDayMonthYear();
			
			// assert
			Assert.That(result, Is.EqualTo(expected));
		}
		
		[Test]
		public void WhenToDayMonthYear_WithNull_ReturnsNull()
		{
			// arrange
			DateTime? date = null;
			
			// act
			var result = date.ToDayMonthYear();
			
			// assert
			Assert.That(result, Is.EqualTo(null));
		}
	}
}