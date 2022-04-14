using ConcernsCaseWork.Helpers;
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


		//[TestCase("07/04/2022")]
		//public void WhenParseExact_ReturnsExpected(string dateString)
		//{
		//	// act
		//	DateTime expectedResult = DateTime.Parse(dateString);
		//	var result = DateTimeHelper.ParseExact(dateString);

		//	//assert
		//	Assert.That(result, Is.EqualTo(expectedResult));
		//}
	}
}
