using ConcernsCaseWork.Utils.Extensions;

namespace ConcernsCaseWork.Utils.Tests.Extensions
{
	public class EnumExtensionsTests
	{
		[Test]
		public void GetDescription_When_ValidValue_Returns_Description()
		{
			var testEnum = TestEnum.Red;

			var result = testEnum.Description();

			result.Should().Be("Ruby Red");
		}

		[Test]
		public void GetDescription_When_ValidValue_WithoutDescription_Returns_Description()
		{
			var testEnum = TestEnum.Green;

			var result = testEnum.Description();

			result.Should().Be("Green");
		}
	}

	public enum TestEnum
	{
		[System.ComponentModel.Description("Ruby Red")]
		Red = 1,
		Green = 2,
		Blue = 3,
		Yellow = 4
	}
}
