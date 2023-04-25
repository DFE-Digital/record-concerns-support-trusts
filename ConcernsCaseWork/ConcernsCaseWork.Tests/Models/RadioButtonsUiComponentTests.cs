using ConcernsCaseWork.Models;
using FluentAssertions;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models
{
	public class RadioButtonsUiComponentTests
	{
		[Test]
		public void Validate_RequiredIsTrueAndSelectedIdIsNull_ReturnsValidationError()
		{
			// Arrange
			var component = new RadioButtonsUiComponent("rootId", "name", "Reason") { Required = true, DisplayName = "Reason" };

			// Act
			var results = component.Validate(new ValidationContext(component));

			// Assert
			results.Should().HaveCount(1);
			results.First().ErrorMessage.Should().Be("Select Reason");
		}

        [Test]
        public void Validate_RequiredIsTrueAndSelectedIdIsNullAndCustomError_ReturnsValidationError()
        {
            // Arrange
            var component = new RadioButtonsUiComponent("rootId", "name", "Reason") 
            { 
                Required = true, 
                DisplayName = "Reason",
                ErrorTextForRequiredField = "This is a custom error"
            };

            // Act
            var results = component.Validate(new ValidationContext(component));

            // Assert
            results.Should().HaveCount(1);
            results.First().ErrorMessage.Should().Be("This is a custom error");
        }

        [Test]
		public void Validate_RequiredIsFalseAndSelectedIdIsNull_ReturnsNoValidationErrors()
		{
			// Arrange
			var component = new RadioButtonsUiComponent("rootId", "name", "heading") { Required = false };

			// Act
			var results = component.Validate(new ValidationContext(component));

			// Assert
			results.Should().BeEmpty();
		}

		[Test]
		public void Validate_RequiredIsTrueAndSelectedIdIsNotNull_ReturnsNoValidationErrors()
		{
			// Arrange
			var component = new RadioButtonsUiComponent("rootId", "name", "heading") { Required = true, SelectedId = 1 };

			// Act
			var results = component.Validate(new ValidationContext(component));

			// Assert
			results.Should().BeEmpty();
		}
	}
}
