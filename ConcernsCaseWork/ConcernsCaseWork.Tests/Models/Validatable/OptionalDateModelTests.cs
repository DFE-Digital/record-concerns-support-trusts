using AutoFixture;
using ConcernsCaseWork.Models.Validatable;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Tests.Models.Validatable
{
	[Parallelizable(ParallelScope.All)]
	public class OptionalDateModelTests
	{
		private readonly IFixture _fixture = new Fixture();
		private ValidationContext _validationContext;

		[SetUp]
		public void Setup()
		{
			_validationContext = _fixture.Create<ValidationContext>();
			_validationContext.DisplayName = "Test Date";
		}

		[Test]
		public void When_DateIsValid_Returns_NoValidationErrors()
		{
			var model = new OptionalDateModel()
			{
				Day = "22",
				Month = "06",
				Year = "2022"
			};

			var result = model.Validate(_validationContext);

			result.Should().BeEmpty();
		}

		[Test]
		public void When_DateIsEmpty_Returns_NoValidationErrors()
		{
			var model = new OptionalDateModel();

			var result = model.Validate(_validationContext);

			result.Should().BeEmpty();
		}

		[Test]
		public void When_NotAllFieldsFilled_Returns_CompleteDateValidationError()
		{
			var model = new OptionalDateModel()
			{
				Day = "22"
			};

			var result = model.Validate(_validationContext);

			var expected = new List<ValidationResult>()
			{
				new ValidationResult("Test Date: Please enter a complete date dd MM YYYY")
			};

			result.Should().BeEquivalentTo(expected);
		}

		[Test]
		public void When_WhenDateIsInvalid_Returns_DateIsInvalidError()
		{
			var model = new OptionalDateModel()
			{
				Day = "22",
				Month = "22",
				Year = "2022"
			};

			var result = model.Validate(_validationContext);

			var expected = new List<ValidationResult>()
			{
				new ValidationResult("Test Date: 22-22-2022 is an invalid date")
			};

			result.Should().BeEquivalentTo(expected);
		}

		[Test]
		public void IsEmpty_When_DateIsEmpty_Returns_True()
		{
			var model = new OptionalDateModel();

			model.IsEmpty().Should().BeTrue();
		}

		[Test]
		public void IsEmpty_When_DateIsNotEmpty_Returns_False()
		{
			var model = new OptionalDateModel()
			{
				Day = "22",
				Month = "22",
				Year = "2022"
			};

			model.IsEmpty().Should().BeFalse();
		}
	}
}
