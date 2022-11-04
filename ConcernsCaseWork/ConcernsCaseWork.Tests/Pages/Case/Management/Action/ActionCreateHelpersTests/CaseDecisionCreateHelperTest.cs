using AutoFixture;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Service.Cases;
using Moq;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.ActionCreateHelpersTests
{
	public class CaseDecisionCreateHelperTest
	{

		[Test]
		public void CaseDecisionCreateHelper_Is_CaseActionCreateHelper()
		{
			var builder = new TestBuilder();
			var sut = builder.BuildSut();

			Assert.IsInstanceOf<CaseActionCreateHelper>(sut);
		}

		[Test]
		public void CaseDecisionCreateHelper_CanHandle_ResponseCorrectly([Values]CaseActionEnum action)
		{
			var expectedResult = action == CaseActionEnum.Decision;

			var builder = new TestBuilder();
			var sut = builder.BuildSut();

			var result = sut.CanHandle(action);

			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public void CaseDecisionCreateHelper_NewCaseActionAllowed_Returns_True()
		{
			var builder = new TestBuilder();
			var sut = builder.BuildSut();
			var expectedResult = true;

			var result = sut.NewCaseActionAllowed(It.IsAny<long>(), It.IsAny<string>()).Result;

			Assert.That(result, Is.EqualTo(expectedResult));
		}

		private class TestBuilder
		{
			public TestBuilder()
			{
				this.Fixture = new Fixture();
			}

			public CaseDecisionCreateHelper BuildSut()
			{
				var result = this.Fixture.Create<CaseDecisionCreateHelper>();

				return result;
			}

			public Fixture Fixture { get; set; }
		}
	}
}
