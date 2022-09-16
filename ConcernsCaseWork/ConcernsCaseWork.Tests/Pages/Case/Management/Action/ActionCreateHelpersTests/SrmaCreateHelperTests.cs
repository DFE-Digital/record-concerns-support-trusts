using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCasework.Service.Cases;
using ConcernsCaseWork.Services.Cases;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.ActionCreateHelpersTests
{
	[Parallelizable(ParallelScope.All)]
	public class SrmaCreateHelperTests
	{

		[Test]
		public async Task SrmaCreateHelper_CanHandle_RespondsCorrectly()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var sut = new SrmaCreateHelper(mockSrmaService.Object);

			// act
			var expectedTrue = sut.CanHandle(CaseActionEnum.Srma);
			var expectedFalse = sut.CanHandle(CaseActionEnum.FinancialPlan);

			// assert
			Assert.That(expectedTrue, Is.True);
			Assert.That(expectedFalse, Is.False);
		}

		[Test]
		public async Task SrmaCreateHelper_NewCaseActionAllowed_ClearToAdd()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var sut = new SrmaCreateHelper(mockSrmaService.Object);

			var caseActions = new SRMAModel[] {
				new SRMAModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new SRMAModel { Id = 124, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-8) },
				new SRMAModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			mockSrmaService.Setup(svc => svc.GetSRMAsForCase(888)).Returns(Task.FromResult(caseActions));

			// act
			var actual = sut.NewCaseActionAllowed(888, string.Empty).Result;

			// assert
			Assert.That(actual, Is.True);
		}

		[Test]
		public async Task SrmaCreateHelper_NewCaseActionAllowed_NotClearToAdd()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var sut = new SrmaCreateHelper(mockSrmaService.Object);

			var caseActions = new SRMAModel[] {
				new SRMAModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new SRMAModel { Id = 124, CaseUrn = 888 },
				new SRMAModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			mockSrmaService.Setup(svc => svc.GetSRMAsForCase(888)).Returns(Task.FromResult(caseActions));

			// act, assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.NewCaseActionAllowed(888, string.Empty));
		}
	}

}