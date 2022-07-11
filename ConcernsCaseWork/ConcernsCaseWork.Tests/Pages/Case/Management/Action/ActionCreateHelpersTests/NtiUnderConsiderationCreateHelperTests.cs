using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Service.Redis.FinancialPlan;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.FinancialPlan
{
	[Parallelizable(ParallelScope.All)]
	public class NtiUnderConsiderationCreateHelperTests
	{

		[Test]
		public async Task NtiUnderConsiderationCreateHelper_CanHandle_RespondsCorrectly()
		{
			// arrange
			var mockNtiService = new Mock<INtiModelService>();
			var sut = new NtiUnderConsiderationCreateHelper(mockNtiService.Object);

			// act
			var expectedTrue = sut.CanHandle(CaseActionEnum.NtiUnderConsideration);
			var expectedFalse = sut.CanHandle(CaseActionEnum.FinancialPlan);

			// assert
			Assert.That(expectedTrue, Is.True);
			Assert.That(expectedFalse, Is.False);
		}

		[Test]
		public async Task NtiUnderConsiderationCreateHelper_NewCaseActionAllowed_ClearToAdd()
		{
			// arrange
			var mockNtiService = new Mock<INtiModelService>();
			var sut = new NtiUnderConsiderationCreateHelper(mockNtiService.Object);

			var caseActions = new NtiModel[] {
				new NtiModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiModel { Id = 124, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			mockNtiService.Setup(svc => svc.GetNtiUnderConsiderationsForCase(888)).Returns(Task.FromResult(caseActions));

			// act
			var actual = sut.NewCaseActionAllowed(888, string.Empty).Result;

			// assert
			Assert.That(actual, Is.True);
		}

		[Test]
		public async Task NtiUnderConsiderationCreateHelper_NewCaseActionAllowed_NotClearToAdd()
		{
			// arrange
			var mockNtiService = new Mock<INtiModelService>();
			var sut = new NtiUnderConsiderationCreateHelper(mockNtiService.Object);

			var caseActions = new NtiModel[] {
				new NtiModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiModel { Id = 124, CaseUrn = 888 },
				new NtiModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			mockNtiService.Setup(svc => svc.GetNtiUnderConsiderationsForCase(888)).Returns(Task.FromResult(caseActions));

			// act, assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.NewCaseActionAllowed(888, string.Empty));
		}
	}

}