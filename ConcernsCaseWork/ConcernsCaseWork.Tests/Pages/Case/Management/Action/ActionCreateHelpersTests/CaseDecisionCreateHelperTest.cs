using AutoFixture;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using NUnit.Framework;
using Service.TRAMS.Cases;
using System;

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
			var builder = new TestBuilder();
			var sut = builder.BuildSut();

			var result = sut.CanHandle(action);

			Assert.That(result, Is.EqualTo(true));
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

