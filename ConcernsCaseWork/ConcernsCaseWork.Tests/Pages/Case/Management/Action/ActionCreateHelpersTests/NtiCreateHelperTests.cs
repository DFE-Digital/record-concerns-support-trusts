using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.NtiWarningLetter;
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
	public class NtiCreateHelperTests
	{

		[Test]
		public async Task NtiCreateHelperTests_CanHandle_RespondsCorrectly()
		{
			// arrange
			var mockNtiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			var sut = new NtiCreateHelper(mockNtiUnderConsiderationService.Object, mockNtiWarningLetterService.Object);

			// act
			var canHandleNTIUnderConsideration = sut.CanHandle(CaseActionEnum.NtiUnderConsideration);
			var canHandleNTIWarningLetter = sut.CanHandle(CaseActionEnum.NtiWarningLetter);
			var canHandleFinancialPlan = sut.CanHandle(CaseActionEnum.FinancialPlan);

			// assert
			Assert.That(canHandleNTIUnderConsideration, Is.True);
			Assert.That(canHandleNTIWarningLetter, Is.True);
			Assert.That(canHandleFinancialPlan, Is.False);
		}

		[Test]
		public async Task NtiCreateHelperTests_NewCaseActionAllowed_ClearToAdd()
		{
			// arrange
			var mockNtiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			var sut = new NtiCreateHelper(mockNtiUnderConsiderationService.Object, mockNtiWarningLetterService.Object);

			var underConsiderationActions = new NtiUnderConsiderationModel[] {
				new NtiUnderConsiderationModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiUnderConsiderationModel { Id = 124, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiUnderConsiderationModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			var warningLetterActions = new NtiWarningLetterModel[] {
				new NtiWarningLetterModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiWarningLetterModel { Id = 124, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiWarningLetterModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			mockNtiUnderConsiderationService.Setup(svc => svc.GetNtiUnderConsiderationsForCase(888)).ReturnsAsync(underConsiderationActions);
			mockNtiWarningLetterService.Setup(svc => svc.GetNtiWarningLettersForCase(888)).ReturnsAsync(warningLetterActions);

			// act
			var actual = sut.NewCaseActionAllowed(888, string.Empty).Result;

			// assert
			Assert.That(actual, Is.True);
		}

		[Test]
		public async Task NtiCreateHelperTests_NewCaseActionAllowed_Open_UnderConsideration_NotClearToAdd()
		{
			// arrange
			var mockNtiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			var sut = new NtiCreateHelper(mockNtiUnderConsiderationService.Object, mockNtiWarningLetterService.Object);

			var underConsiderationActions = new NtiUnderConsiderationModel[] {
				new NtiUnderConsiderationModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiUnderConsiderationModel { Id = 124, CaseUrn = 888 },
				new NtiUnderConsiderationModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			var warningLetterActions = new NtiWarningLetterModel[] {
				new NtiWarningLetterModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiWarningLetterModel { Id = 124, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiWarningLetterModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			mockNtiUnderConsiderationService.Setup(svc => svc.GetNtiUnderConsiderationsForCase(888)).ReturnsAsync(underConsiderationActions);
			mockNtiWarningLetterService.Setup(svc => svc.GetNtiWarningLettersForCase(888)).ReturnsAsync(warningLetterActions);

			// act, assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.NewCaseActionAllowed(888, string.Empty));
		}

		[Test]
		public async Task NtiCreateHelperTests_NewCaseActionAllowed_Open_WarningLetter_NotClearToAdd()
		{
			// arrange
			var mockNtiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			var sut = new NtiCreateHelper(mockNtiUnderConsiderationService.Object, mockNtiWarningLetterService.Object);

			var underConsiderationActions = new NtiUnderConsiderationModel[] {
				new NtiUnderConsiderationModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiUnderConsiderationModel { Id = 124, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiUnderConsiderationModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			var warningLetterActions = new NtiWarningLetterModel[] {
				new NtiWarningLetterModel { Id = 123, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiWarningLetterModel { Id = 124, CaseUrn = 888 },
				new NtiWarningLetterModel { Id = 125, CaseUrn = 888, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			mockNtiUnderConsiderationService.Setup(svc => svc.GetNtiUnderConsiderationsForCase(888)).ReturnsAsync(underConsiderationActions);
			mockNtiWarningLetterService.Setup(svc => svc.GetNtiWarningLettersForCase(888)).ReturnsAsync(warningLetterActions);

			// act, assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.NewCaseActionAllowed(888, string.Empty));
		}
	}

}