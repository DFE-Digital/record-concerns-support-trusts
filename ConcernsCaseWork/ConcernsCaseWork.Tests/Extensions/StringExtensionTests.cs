using ConcernsCaseWork.Extensions;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Extensions
{
	[Parallelizable(ParallelScope.All)]
	public class StringExtensionTests
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

		[TestCase("http://www.holleyparkacademy.co.uk", "http://www.holleyparkacademy.co.uk")]
		[TestCase("www.holleyparkacademy.co.uk", "http://www.holleyparkacademy.co.uk")]
		public void WhenToUri_Returns_Valid_Url(string input, string expected)
		{
			// assert
			Assert.That(input.ToUrl(), Is.EqualTo(expected));
		}

		[TestCase("123:type", "subtype", "123", "type", "")]
		[TestCase("type", "123:subtype", "123", "type", "subtype")]
		[TestCase("type", "subtype", "", "type", "subtype")]
		public void WhenSplitType_Returns_IsValid(string type, string subType, string expectedTypeUrn, string expectedType, string expectedSubType)
		{
			// act
			(string actualTypeUrn, string actualType, string actualSubType) = type.SplitType(subType);
			
			// assert
			Assert.That(actualTypeUrn, Is.EqualTo(expectedTypeUrn));
			Assert.That(actualType, Is.EqualTo(expectedType));
			Assert.That(actualSubType, Is.EqualTo(expectedSubType));
		}
	}
}