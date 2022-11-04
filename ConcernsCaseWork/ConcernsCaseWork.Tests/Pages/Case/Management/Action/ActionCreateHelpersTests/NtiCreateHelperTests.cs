using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.CaseActionCreateHelpers;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.ActionCreateHelpersTests
{
	[Parallelizable(ParallelScope.All)]
	public class NtiCreateHelperTests
	{
		[Test]
		public async Task NtiCreateHelperTests_CanHandle_RespondsCorrectly()
		{
			// arrange
			NtiCreateHelper sut = CreateNtiCreateHelper();

			// act
			var canHandleNTIUnderConsideration = sut.CanHandle(CaseActionEnum.NtiUnderConsideration);
			var canHandleNTIWarningLetter = sut.CanHandle(CaseActionEnum.NtiWarningLetter);
			var canHandleNti = sut.CanHandle(CaseActionEnum.Nti);

			var canHandleFinancialPlan = sut.CanHandle(CaseActionEnum.FinancialPlan);

			// assert
			Assert.That(canHandleNTIUnderConsideration, Is.True);
			Assert.That(canHandleNTIWarningLetter, Is.True);
			Assert.That(canHandleNti, Is.True);
			Assert.That(canHandleFinancialPlan, Is.False);
		}


		[Test]
		public async Task NtiCreateHelperTests_NewCaseActionAllowed_ClearToAdd()
		{
			// arrange
			var caseUrn = 889L;
			var mockNtiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			var mockNtiService = new Mock<INtiModelService>();
			var sut = CreateNtiCreateHelper(mockNtiUnderConsiderationService, mockNtiWarningLetterService);

			var underConsiderationActions = new NtiUnderConsiderationModel[] {
				new NtiUnderConsiderationModel { Id = 123, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiUnderConsiderationModel { Id = 124, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiUnderConsiderationModel { Id = 125, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			var warningLetterActions = new NtiWarningLetterModel[] {
				new NtiWarningLetterModel { Id = 123, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiWarningLetterModel { Id = 124, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiWarningLetterModel { Id = 125, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			var ntis = new NtiModel[] {
				new NtiModel { Id = 1, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-3)}
			};

			mockNtiUnderConsiderationService.Setup(svc => svc.GetNtiUnderConsiderationsForCase(caseUrn)).ReturnsAsync(underConsiderationActions);
			mockNtiWarningLetterService.Setup(svc => svc.GetNtiWarningLettersForCase(caseUrn)).ReturnsAsync(warningLetterActions);
			mockNtiService.Setup(svc => svc.GetNtisForCaseAsync(caseUrn)).ReturnsAsync(ntis);

			// act
			var actual = sut.NewCaseActionAllowed(caseUrn, string.Empty).Result;

			// assert
			Assert.That(actual, Is.True);
		}

		[Test]
		public async Task NtiCreateHelperTests_NewCaseActionAllowed_Open_UnderConsideration_NotClearToAdd()
		{
			// arrange
			var mockNtiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			var sut = CreateNtiCreateHelper(mockNtiUnderConsiderationService, mockNtiWarningLetterService);

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
			var sut = CreateNtiCreateHelper(mockNtiUnderConsiderationService, mockNtiWarningLetterService);

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

		[Test]
		public async Task NtiCreateHelperTests_NewCaseActionAllowed_Open_NoticeToImprove_NotClearToAdd()
		{
			// arrange
			var caseUrn = 888L;
			var mockNtiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			var mockNtiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			var mockNtiService = new Mock<INtiModelService>();
			var sut = CreateNtiCreateHelper(mockNtiUnderConsiderationService, mockNtiWarningLetterService, mockNtiService);

			var underConsiderationActions = new NtiUnderConsiderationModel[] {
				new NtiUnderConsiderationModel { Id = 123, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiUnderConsiderationModel { Id = 124, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-8) },
				new NtiUnderConsiderationModel { Id = 125, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			var warningLetterActions = new NtiWarningLetterModel[] {
				new NtiWarningLetterModel { Id = 123, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-10) },
				new NtiWarningLetterModel { Id = 124, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-2) },
				new NtiWarningLetterModel { Id = 125, CaseUrn = caseUrn, ClosedAt = DateTime.Now.AddDays(-5) },
			}.AsEnumerable();

			var ntis = new NtiModel[] {
				new NtiModel { Id = 771, CaseUrn = caseUrn, ClosedAt= DateTime.Now.AddDays(-10) },
				new NtiModel { Id = 772, CaseUrn = caseUrn, ClosedAt = null }
			};

			mockNtiUnderConsiderationService.Setup(svc => svc.GetNtiUnderConsiderationsForCase(caseUrn)).ReturnsAsync(underConsiderationActions);
			mockNtiWarningLetterService.Setup(svc => svc.GetNtiWarningLettersForCase(caseUrn)).ReturnsAsync(warningLetterActions);
			mockNtiService.Setup(svc => svc.GetNtisForCaseAsync(caseUrn)).ReturnsAsync(ntis);

			// act, assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.NewCaseActionAllowed(caseUrn, string.Empty));
		}

		private static NtiCreateHelper CreateNtiCreateHelper(Mock<INtiUnderConsiderationModelService> mockNtiUnderConsiderationService = null,
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterService = null,
			Mock<INtiModelService> mockNtiModelService = null)
		{
			mockNtiUnderConsiderationService ??= new Mock<INtiUnderConsiderationModelService>();
			mockNtiWarningLetterService ??= new Mock<INtiWarningLetterModelService>();
			mockNtiModelService ??= new Mock<INtiModelService>();

			return new NtiCreateHelper(mockNtiUnderConsiderationService.Object, mockNtiWarningLetterService.Object, mockNtiModelService.Object);
		}
	}
}