using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models.Validatable;

[Parallelizable(ParallelScope.All)]
public class StringValidatableTests
{	
	[Test]
	public void WhenStringContentsLength_ExceedsMaxLength_Validate_ReturnsValidationResult()
	{
		// arrange
		var sut = new ValidateableString()
		{
			DisplayName = "Supporting Notes",
			MaxLength = 10,
			StringContents = "max character has been exceeded"
		};
		var context = new ValidationContext(sut);
			
		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(1);
		result.Single().ErrorMessage.Should().Be("Supporting Notes must be 10 characters or less");
	}

	[Test]
	public void WhenStringContentsLength_DoesNot_ExceedsMaxLength_Validate_ReturnsValidationResult()
	{
		// arrange
		var sut = new ValidateableString()
		{
			DisplayName = "Supporting Notes",
			MaxLength = 100,
			StringContents = "max character has been exceeded"
		};
		var context = new ValidationContext(sut);

		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(0);
	}

	[TestCase("")]
	[TestCase(null)]
	public void WhenRequired_NoValue_ReturnsValidationError(string? value)
	{
		// arrange
		var sut = new ValidateableString()
		{
			DisplayName = "Supporting Notes",
			MaxLength = 100,
			StringContents = value,
			Required = true
		};
		var context = new ValidationContext(sut);

		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(1);
		result.Single().ErrorMessage.Should().Be("Supporting Notes is required");
	}

	[Test]
	public void WhenRequired_ValueProvided_ReturnsNoValidationErrors()
	{
		// arrange
		var sut = new ValidateableString()
		{
			DisplayName = "Supporting Notes",
			MaxLength = 100,
			StringContents = "This is a value",
			Required = true
		};
		var context = new ValidationContext(sut);

		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(0);
	}
}