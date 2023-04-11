using ConcernsCaseWork.Models;
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
			Assert.That(results, Has.Exactly(1).InstanceOf<ValidationResult>());
			Assert.That(results.First().ErrorMessage, Is.EqualTo("Reason: Please enter a value"));
		}

		[Test]
		public void Validate_RequiredIsFalseAndSelectedIdIsNull_ReturnsNoValidationErrors()
		{
			// Arrange
			var component = new RadioButtonsUiComponent("rootId", "name", "heading") { Required = false };

			// Act
			var results = component.Validate(new ValidationContext(component));

			// Assert
			Assert.That(results, Is.Empty);
		}

		[Test]
		public void Validate_RequiredIsTrueAndSelectedIdIsNotNull_ReturnsNoValidationErrors()
		{
			// Arrange
			var component = new RadioButtonsUiComponent("rootId", "name", "heading") { Required = true, SelectedId = 1 };

			// Act
			var results = component.Validate(new ValidationContext(component));

			// Assert
			Assert.That(results, Is.Empty);
		}
	}
}
