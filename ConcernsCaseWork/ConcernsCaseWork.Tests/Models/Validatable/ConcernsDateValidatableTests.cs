using AutoFixture;
using ConcernsCaseWork.Models.Validatable;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models.Validatable;

[Parallelizable(ParallelScope.All)]
public class ConcernsDateValidatableTests
{
	private readonly IFixture _fixture = new Fixture();
	
	[Test]
	public void WhenDayMonthAndYearNotSet_Validate_ReturnsValidationResult()
	{
		// arrange
		var sut = new ConcernsDateValidatable();
		var context = new ValidationContext(sut);
			
		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(1);
		result.Single().ErrorMessage.Should().Be("Enter a valid date.");
		result.Single().MemberNames.Should().BeEmpty();
	}
	
	[Test]
	[TestCase("")]
	[TestCase("0")]
	[TestCase("32")]
	[TestCase("dffg")]
	public void WhenDayIsNotAValidDay_Validate_ReturnsValidationResult(string day)
	{
		// arrange
		var testDate = _fixture.Create<DateTime>();
		var sut = new ConcernsDateValidatable
		{
			Day = day, 
			Month = testDate.Month.ToString(), 
			Year = testDate.Year.ToString()
		};
		var context = new ValidationContext(sut);
			
		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(1);
		result.Single().ErrorMessage.Should().Be("Enter a valid date.");
		result.Single().MemberNames.Should().BeEmpty();
	}
		
	[Test]
	[TestCase("")]
	[TestCase("0")]
	[TestCase("13")]
	[TestCase("dffg")]
	public void WhenMonthIsNotAValidMonth_Validate_ReturnsValidationResult(string month)
	{
		// arrange
		var testDate = _fixture.Create<DateTime>();
		var sut = new ConcernsDateValidatable
		{
			Day = testDate.Day.ToString(), 
			Month = month, 
			Year = testDate.Year.ToString()
		};
		var context = new ValidationContext(sut);
			
		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(1);
		result.Single().ErrorMessage.Should().Be("Enter a valid date.");
		result.Single().MemberNames.Should().BeEmpty();
	}
			
	[Test]
	[TestCase("")]
	[TestCase("0")]
	[TestCase("13")]
	[TestCase("dffg")]
	public void WhenYearIsNotAValidYear_Validate_ReturnsValidationResult(string year)
	{
		// arrange
		var testDate = _fixture.Create<DateTime>();
		var sut = new ConcernsDateValidatable
		{
			Day = testDate.Day.ToString(), 
			Month = testDate.Month.ToString(), 
			Year = year
		};
		var context = new ValidationContext(sut);
			
		// act
		var result = sut.Validate(context);

		// assert
		result.Should().HaveCount(1);
		result.Single().ErrorMessage.Should().Be("Enter a valid date.");
		result.Single().MemberNames.Should().BeEmpty();
	}
	
	[Test]
	[TestCase("")]
	[TestCase("0")]
	[TestCase("13")]
	[TestCase("45678")]
	[TestCase("dffg")]
	public void WhenDateIsValid_Validate_ReturnsEmptyValidationResult(string year)
	{
		// arrange
		var testDate = _fixture.Create<DateTime>();
		var sut = new ConcernsDateValidatable
		{
			Day = testDate.Day.ToString(), 
			Month = testDate.Month.ToString(), 
			Year = testDate.Year.ToString()
		};
		var context = new ValidationContext(sut);
			
		// act
		var result = sut.Validate(context);

		// assert
		result.Should().BeEmpty();
	}
}