﻿using ConcernsCaseWork.Pages.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Pages.Validators
{
	[Parallelizable(ParallelScope.All)]
	public class ConcernTypeValidatorTests
	{
		[Test]
		public void WhenConcernType_WhenType_Contains_TypeUrn_IsValid()
		{
			// arrange
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("123:type") },
					{ "sub-type", new StringValues("subtype") },
					{ "rating", new StringValues("rating") },
					{ "trust-ukprn", new StringValues("trustukprn") },
					{ "means-of-referral-id", new StringValues("1") }
				});
			
			// act
			var isValid = ConcernTypeValidator.IsValid(formCollection);

			// assert
			Assert.That(isValid, Is.True);
		}
		
		[Test]
		public void WhenConcernType_WhenSubType_Contains_TypeUrn_IsValid()
		{
			// arrange
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("type") },
					{ "sub-type", new StringValues("123:subtype") },
					{ "rating", new StringValues("rating") },
					{ "trust-ukprn", new StringValues("trustukprn") },
					{ "means-of-referral-id", new StringValues("2") }
				});
			
			// act
			var isValid = ConcernTypeValidator.IsValid(formCollection);

			// assert
			Assert.That(isValid, Is.True);
		}
		
		[Test]
		public void WhenConcernType_IsInValid()
		{
			// arrange
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("") },
					{ "sub-type", new StringValues("") },
					{ "rating", new StringValues("") },
					{ "trust-ukprn", new StringValues("") },
					{ "means-of-referral-id", new StringValues("2") }
				});
			
			// act
			var isValid = ConcernTypeValidator.IsValid(formCollection);

			// assert
			Assert.That(isValid, Is.False);
		}
		
		[Test]
		public void WhenEditConcernType_WhenType_Contains_TypeUrn_IsValid()
		{
			// arrange
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("123:type") },
					{ "sub-type", new StringValues("subtype") }
				});
			
			// act
			var isValid = ConcernTypeValidator.IsEditValid(formCollection);

			// assert
			Assert.That(isValid, Is.True);
		}
		
		[Test]
		public void WhenEditConcernType_WhenSubType_Contains_TypeUrn_IsValid()
		{
			// arrange
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("type") },
					{ "sub-type", new StringValues("123:subtype") }
				});
			
			// act
			var isValid = ConcernTypeValidator.IsEditValid(formCollection);

			// assert
			Assert.That(isValid, Is.True);
		}
		
		[Test]
		public void WhenEditConcernType_IsInValid()
		{
			// arrange
			var formCollection = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("") },
					{ "sub-type", new StringValues("") }
				});
			
			// act
			var isValid = ConcernTypeValidator.IsEditValid(formCollection);

			// assert
			Assert.That(isValid, Is.False);
		}
	}
}