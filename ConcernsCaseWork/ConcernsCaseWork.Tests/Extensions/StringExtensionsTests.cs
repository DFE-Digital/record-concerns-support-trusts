using ConcernsCaseWork.Extensions;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Extensions
{
	[Parallelizable(ParallelScope.All)]
	public class StringExtensionsTests
	{
		[TestCase("", false)]
		[TestCase("xpto", false)]
		[TestCase("true", true)]
		[TestCase("True", true)]
		[TestCase("t", true)]
		[TestCase("yes", true)]
		[TestCase("Yes", true)]
		[TestCase("y", true)]
		[TestCase("1", true)]
		[TestCase("false", false)]
		[TestCase("False", false)]
		[TestCase("f", false)]
		[TestCase("no", false)]
		[TestCase("No", false)]
		[TestCase("n", false)]
		[TestCase("0", false)]
		public void WhenToBoolean_ReturnsExpected(string actual, bool expected)
		{
			// assert
			Assert.That(actual.ToBoolean(), Is.EqualTo(expected));
		}

		[TestCase("Acer Trust", "Acer Trust")]
		[TestCase("ACER TrUst", "Acer Trust")]
		[TestCase("acer TRUST", "Acer Trust")]
		[TestCase("ACER TRUST", "Acer Trust")]
		[TestCase("acer trust", "Acer Trust")]
		public void WhenToTitle_ReturnsExpected(string actual, string expected)
		{
			// assert
			Assert.That(actual.ToTitle(), Is.EqualTo(expected));
		}

		[TestCase("100", 100)]
		[TestCase("invalid_string", 0)]
		[TestCase("", 0)]
		[TestCase(null, 0)]
		public void WhenParseToInt_ReturnsExpected(string input, int expected)
		{
			// assert
			Assert.That(input.ParseToInt(), Is.EqualTo(expected));
		}
	}
}