using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.ActionCreateHelpersTests
{
	[Parallelizable(ParallelScope.All)]
	public class FinancialPlanCreateHelperTests
	{
		[Test]
		public async Task FinancialPlanCreateHelper_CanHandle_RespondsCorrectly()
		{
			// arrange
			var mockFinancialPlanService = new Mock<IFinancialPlanModelService>();
			var sut = new FinancialPlanCreateHelper(mockFinancialPlanService.Object);

			// act
			var expectedTrue = sut.CanHandle(CaseActionEnum.FinancialPlan);
			var expectedFalse = sut.CanHandle(CaseActionEnum.Nti);

			// assert
			Assert.That(expectedTrue, Is.True);
			Assert.That(expectedFalse, Is.False);
		}

		[Test]
		public async Task FinancialPlanCreateHelper_NewCaseActionAllowed_ClearToAdd()
		{
			// arrange
			var mockFinancialPlanService = new Mock<IFinancialPlanModelService>();
			var sut = new FinancialPlanCreateHelper(mockFinancialPlanService.Object);

			IList<FinancialPlanModel> caseActions = new List<FinancialPlanModel> {
				new FinancialPlanModel (123, 888, DateTime.Now.AddDays(-15), null, null, string.Empty, null, DateTime.Now.AddDays(-10)),
				new FinancialPlanModel (123, 888, DateTime.Now.AddDays(-25), null, null, string.Empty, null, DateTime.Now.AddDays(-10)),
				new FinancialPlanModel (123, 888, DateTime.Now.AddDays(-11), null, null, string.Empty, null, DateTime.Now.AddDays(-10))
			};

			mockFinancialPlanService.Setup(svc => svc.GetFinancialPlansModelByCaseUrn(888, string.Empty)).Returns(Task.FromResult(caseActions));

			// act
			var actual = sut.NewCaseActionAllowed(888, string.Empty).Result;

			// assert
			Assert.That(actual, Is.True);
		}

		[Test]
		public async Task SrmaCreateHelper_NewCaseActionAllowed_NotClearToAdd()
		{
			// arrange
			var mockFinancialPlanService = new Mock<IFinancialPlanModelService>();
			var sut = new FinancialPlanCreateHelper(mockFinancialPlanService.Object);

			IList<FinancialPlanModel> caseActions = new List<FinancialPlanModel> {
				new FinancialPlanModel (123, 888, DateTime.Now.AddDays(-15), null, null, string.Empty, null, DateTime.Now.AddDays(-10)),
				new FinancialPlanModel (123, 888, DateTime.Now.AddDays(-25), null, null, string.Empty, null, closedAt: null),
				new FinancialPlanModel (123, 888, DateTime.Now.AddDays(-11), null, null, string.Empty, null, DateTime.Now.AddDays(-10))
			};

			mockFinancialPlanService.Setup(svc => svc.GetFinancialPlansModelByCaseUrn(888, string.Empty)).Returns(Task.FromResult(caseActions));

			// act, assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.NewCaseActionAllowed(888, string.Empty));
		}
	}
}