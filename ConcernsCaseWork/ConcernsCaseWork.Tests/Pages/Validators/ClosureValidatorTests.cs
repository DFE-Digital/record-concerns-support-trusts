using ConcernsCaseWork.Pages.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Pages.Validators
{
	[Parallelizable(ParallelScope.All)]
	public class ClosureValidatorTests
	{
		[Test]
		public void WhenClosureType_When_Monitoring_True_IsValid()
		{
			// arrange
			var timeNow = DateTimeOffset.Now.Add(TimeSpan.FromDays(+1));
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("case-outcomes") },
					{ "monitoring", new StringValues("true") },
					{ "dtr-day", new StringValues(timeNow.Day.ToString()) },
					{ "dtr-month", new StringValues(timeNow.Month.ToString()) },
					{ "dtr-year", new StringValues(timeNow.Year.ToString()) }
				});
			
			// act
			var isValid = ClosureValidator.IsValid(formCollection);

			// assert
			Assert.True(isValid);
		}
		
		[Test]
		public void WhenClosureType_When_Monitoring_False_IsValid()
		{
			// arrange
			var timeNow = DateTimeOffset.Now.Add(TimeSpan.FromDays(+1));
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("case-outcomes") },
					{ "monitoring", new StringValues("false") },
					{ "dtr-day", new StringValues(timeNow.Day.ToString()) },
					{ "dtr-month", new StringValues(timeNow.Month.ToString()) },
					{ "dtr-year", new StringValues(timeNow.Year.ToString()) }
				});
			
			// act
			var isValid = ClosureValidator.IsValid(formCollection);

			// assert
			Assert.True(isValid);
		}
		
		[Test]
		public void WhenClosureType_When_Monitoring_True_IsInValid()
		{
			// arrange
			var timeNow = DateTimeOffset.Now.Add(TimeSpan.FromDays(+1));
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("case-outcomes") },
					{ "monitoring", new StringValues("true") },
					{ "dtr-day", new StringValues("") },
					{ "dtr-month", new StringValues("") },
					{ "dtr-year", new StringValues(timeNow.Year.ToString()) }
				});
			
			// act
			var isValid = ClosureValidator.IsValid(formCollection);

			// assert
			Assert.False(isValid);
		}
		
		[Test]
		public void WhenClosureType_When_Missing_Required_IsInValid()
		{
			// arrange
			var timeNow = DateTimeOffset.Now.Add(TimeSpan.FromDays(+1));
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("") },
					{ "monitoring", new StringValues("true") },
					{ "dtr-day", new StringValues("") },
					{ "dtr-month", new StringValues("") },
					{ "dtr-year", new StringValues(timeNow.Year.ToString()) }
				});
			
			// act
			var isValid = ClosureValidator.IsValid(formCollection);

			// assert
			Assert.False(isValid);
		}
	}
}