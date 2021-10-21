using AutoMapper;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class UtilitiesTests
	{
		[Test]
		public void ParseInt_Convert_Valid_String_To_Int()
		{
			// arrange
			string input = "100";
			int expectedResult = 100;

			// act
			int result = Utilities.ParseInt(input);

			// assert
			Assert.That(expectedResult, Is.EqualTo(result));
		}

		[Test]
		public void ParseInt_Convert_Invalid_String_To_0()
		{
			// arrange
			string input = "invalid_string";
			int expectedResult = 0;

			// act
			int result = Utilities.ParseInt(input);

			// assert
			Assert.That(expectedResult, Is.EqualTo(result));
		}

		[Test]
		public void ParseInt_Convert_Empty_String_To_0()
		{
			// arrange
			string input = string.Empty;
			int expectedResult = 0;

			// act
			int result = Utilities.ParseInt(input);

			// assert
			Assert.That(expectedResult, Is.EqualTo(result));
		}

		[Test]
		public void ParseInt_Convert_Null_Value_To_0()
		{
			// arrange
			string input = null;
			int expectedResult = 0;

			// act
			int result = Utilities.ParseInt(input);

			// assert
			Assert.That(expectedResult, Is.EqualTo(result));
		}
	}
}