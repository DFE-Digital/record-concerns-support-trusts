using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models
{
	[TestFixture]
	public class OptionalDateTimeUiComponentTests
	{
		private OptionalDateTimeUiComponent _component;

		[SetUp]
		public void SetUp()
		{
			_component = new OptionalDateTimeUiComponent("elementId", "name", "heading");
		}

		[Test]
		public void Validate_ReturnsNoErrors_WhenDateIsNotRequiredAndIsNull()
		{
			// Arrange
			_component.Required = false;
			_component.Date = null;

			// Act
			var result = _component.Validate("Display Name");

			// Assert
			result.Should().BeEmpty();
		}

		[Test]
		public void Validate_ReturnsNoErrors_WhenDateIsNotRequiredAndIsEmpty()
		{
			// Arrange
			_component.Required = false;
			_component.Date = new OptionalDateModel();

			// Act
			var result = _component.Validate("Display Name");

			// Assert
			result.Should().BeEmpty();
		}

		[Test]
		public void Validate_ReturnsError_WhenDateIsRequiredAndIsNull()
		{
			// Arrange
			_component.Required = true;
			_component.Date = null;

			// Act
			var result = _component.Validate("Display Name");

			// Assert
			result.Should().HaveCount(1);
			result.First().ErrorMessage.Should().Be("Display Name: Please enter a date");
		}

		[Test]
		public void Validate_ReturnsError_WhenDateIsRequiredAndIsEmpty()
		{
			// Arrange
			_component.Required = true;
			_component.Date = new OptionalDateModel();

			// Act
			var result = _component.Validate("Display Name");

			// Assert
			result.Should().HaveCount(1);
			result.First().ErrorMessage.Should().Be("Display Name: Please enter a date");
		}

		[Test]
		public void Validate_CallsDateValidate_WhenDateIsNotNull()
		{
			// Arrange
			_component.Required = false;
			_component.DisplayName = "Date";

			_component.Date = new OptionalDateModel()
			{
				Day = "22"
			};

			// Act
			var result = _component.Validate("Display Name");

			// Assert
			result.Should().HaveCount(1);
			result.First().ErrorMessage.Should().Be("Date: Please enter a complete date DD MM YYYY");
		}
	}
}
