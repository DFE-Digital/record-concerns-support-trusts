using ConcernsCaseWork.Models;
using FluentAssertions;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models
{
	internal class CheckboxUiComponentTests
	{
		private CheckboxUiComponent _validator;

		[SetUp]
		public void Setup()
		{
			_validator = new CheckboxUiComponent("test", "name", "heading");
		}

		[Test]
		public void Validate_WhenRequiredAndNotChecked_ReturnsError()
		{
			// Arrange
			_validator.Required = true;
			_validator.Checked = false;
			_validator.DisplayName = "DisplayName";

			// Act
			var result = _validator.Validate(new ValidationContext(_validator));

			// Assert
			result.Should().HaveCount(1);
			result.First().ErrorMessage.Should().Be("DisplayName: Please select");
			result.First().MemberNames.Should().Contain("DisplayName");
		}

		[Test]
		public void Validate_WhenRequiredAndNotCheckedAndCustomError_ReturnsError()
		{
			// Arrange
			_validator.Required = true;
			_validator.Checked = false;
			_validator.DisplayName = "DisplayName";
			_validator.ErrorTextForRequiredField = "This is a custom error";

			// Act
			var result = _validator.Validate(new ValidationContext(_validator));

			// Assert
			result.Should().HaveCount(1);
			result.First().ErrorMessage.Should().Be("This is a custom error");
			result.First().MemberNames.Should().Contain("DisplayName");
		}

		[TestCase(true, true)]
		[TestCase(false, false)]
		[TestCase(false, true)]
		public void Validate_Scenarios_ReturnsNoError(bool required, bool @checked)
		{
			// Arrange
			_validator.Required = required;
			_validator.Checked = @checked;

			// Act
			var result = _validator.Validate(new ValidationContext(_validator));

			// Assert
			result.Should().BeEmpty();
		}
	}
}
