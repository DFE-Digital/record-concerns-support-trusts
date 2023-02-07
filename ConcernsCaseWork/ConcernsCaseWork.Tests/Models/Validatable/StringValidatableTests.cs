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
	private readonly IFixture _fixture = new Fixture();
	
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
		result.Single().ErrorMessage.Should().Be("Supporting Notes: Exceeds maximum allowed length (10 characters).");
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


}