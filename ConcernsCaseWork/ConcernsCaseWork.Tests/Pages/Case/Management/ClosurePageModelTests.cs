using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.All)]
	public class ClosurePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_When_No_OpenConcerns_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNTIUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNTIWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var mockNTIModelService = new Mock<INtiModelService>();
			var mockCaseActionValidator = new Mock<ICaseActionValidator>();

			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var closedRecordModel = RecordFactory.BuildRecordModel(3);
			var recordsList = new List<RecordModel>() { closedRecordModel };
			var liveStatusDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);
			var closeStatusDto = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			var expectedCaseModel = CaseFactory.BuildCaseModel();
			var expectedTrustDetailsModel = TrustFactory.BuildTrustDetailsModel();

			DateTime closedAt = DateTime.Now;

			var openSRMAModels = SrmaFactory.BuildListSrmaModel(SRMAStatus.PreparingForDeployment, SRMAReasonOffered.SchoolsFinancialSupportAndOversight, closedAt);
			var financialPlans = FinancialPlanFactory.BuildListFinancialPlanModel(closedAt);
			var ntiUnderConsiderationModels = NTIUnderConsiderationFactory.BuildClosedListNTIUnderConsiderationModel();
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(2, closedAt);
			var ntiModels = NTIFactory.BuildClosedListNTIModel();

			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsList);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Live.ToString()))
				.ReturnsAsync(liveStatusDto);

			mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Close.ToString()))
				.ReturnsAsync(closeStatusDto);

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(expectedCaseModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(expectedTrustDetailsModel);

			mockSRMAModelService.Setup(s => s.GetSRMAsForCase(It.IsAny<long>()))
				.ReturnsAsync(openSRMAModels);

			mockFinancialPlanModelService.Setup(f => f.GetFinancialPlansModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(financialPlans);

			mockNTIUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiUnderConsiderationModels);

			mockNTIWarningLetterModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);

			mockNTIModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModels);

			mockCaseActionValidator.Setup(v => v.Validate(It.IsAny<IEnumerable<CaseActionModel>>()))
				.Returns(new List<string>());

			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockStatusCachedService.Object, mockSRMAModelService.Object, mockFinancialPlanModelService.Object, mockNTIUnderConsiderationModelService.Object, mockNTIWarningLetterModelService.Object, mockNTIModelService.Object, mockCaseActionValidator.Object, mockLogger.Object, true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			var caseModel = pageModel.CaseModel;
			var trustDetailsModel = pageModel.TrustDetailsModel;

			// assert
			Assert.IsAssignableFrom<CaseModel>(caseModel);

			Assert.That(caseModel.Description, Is.EqualTo(expectedCaseModel.Description));
			Assert.That(caseModel.Issue, Is.EqualTo(expectedCaseModel.Issue));
			Assert.That(caseModel.StatusId, Is.EqualTo(expectedCaseModel.StatusId));
			Assert.That(caseModel.Urn, Is.EqualTo(expectedCaseModel.Urn));
			Assert.That(caseModel.ClosedAt, Is.EqualTo(expectedCaseModel.ClosedAt));
			Assert.That(caseModel.CreatedAt, Is.EqualTo(expectedCaseModel.CreatedAt));
			Assert.That(caseModel.CreatedBy, Is.EqualTo(expectedCaseModel.CreatedBy));
			Assert.That(caseModel.CrmEnquiry, Is.EqualTo(expectedCaseModel.CrmEnquiry));
			Assert.That(caseModel.CurrentStatus, Is.EqualTo(expectedCaseModel.CurrentStatus));
			Assert.That(caseModel.DeEscalation, Is.EqualTo(expectedCaseModel.DeEscalation));
			Assert.That(caseModel.NextSteps, Is.EqualTo(expectedCaseModel.NextSteps));
			Assert.That(caseModel.CaseAim, Is.EqualTo(expectedCaseModel.CaseAim));
			Assert.That(caseModel.DeEscalationPoint, Is.EqualTo(expectedCaseModel.DeEscalationPoint));
			Assert.That(caseModel.ReviewAt, Is.EqualTo(expectedCaseModel.ReviewAt));
			Assert.That(caseModel.StatusName, Is.EqualTo(expectedCaseModel.StatusName));
			Assert.That(caseModel.UpdatedAt, Is.EqualTo(expectedCaseModel.UpdatedAt));
			Assert.That(caseModel.DirectionOfTravel, Is.EqualTo(expectedCaseModel.DirectionOfTravel));
			Assert.That(caseModel.ReasonAtReview, Is.EqualTo(expectedCaseModel.ReasonAtReview));
			Assert.That(caseModel.TrustUkPrn, Is.EqualTo(expectedCaseModel.TrustUkPrn));
			Assert.That(caseModel.PreviousUrl, Is.EqualTo(expectedCaseModel.PreviousUrl));

			Assert.That(trustDetailsModel, Is.Not.Null);
			Assert.That(trustDetailsModel.Establishments, Is.Not.Null);
			Assert.That(trustDetailsModel.Establishments.Count, Is.EqualTo(1));
			Assert.That(trustDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustDetailsModel.IfdData, Is.Not.Null);
			Assert.That(trustDetailsModel.TotalPupils, Is.EqualTo(0.0d));
			Assert.That(trustDetailsModel.PupilCapacityPercentage, Is.EqualTo(0.0d));
			Assert.That(trustDetailsModel.TotalPupilCapacity, Is.EqualTo(0.0d));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}

		[Test]
		public async Task WhenOnGetAsync_When_OpenConcerns_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNTIUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNTIWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var mockNTIModelService = new Mock<INtiModelService>();
			var mockCaseActionValidator = new Mock<ICaseActionValidator>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var openRecordModel = RecordFactory.BuildRecordModel();
			var recordsList = new List<RecordModel>() { openRecordModel };
			var liveStatusDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);

			DateTime closedAt = DateTime.Now;

			var financialPlans = FinancialPlanFactory.BuildListFinancialPlanModel(closedAt);
			var ntiUnderConsiderationModels = NTIUnderConsiderationFactory.BuildListNTIUnderConsiderationModel();
			var ntiWarningLetterModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(2, closedAt);
			var ntiModels = NTIFactory.BuildClosedListNTIModel();

			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsList);

			mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(liveStatusDto);

			mockFinancialPlanModelService.Setup(f => f.GetFinancialPlansModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(financialPlans);

			mockNTIUnderConsiderationModelService.Setup(uc => uc.GetNtiUnderConsiderationsForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiUnderConsiderationModels);

			mockNTIWarningLetterModelService.Setup(wl => wl.GetNtiWarningLettersForCase(It.IsAny<long>()))
				.ReturnsAsync(ntiWarningLetterModels);

			mockNTIModelService.Setup(nti => nti.GetNtisForCaseAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModels);

			mockCaseActionValidator.Setup(v => v.Validate(It.IsAny<IEnumerable<CaseActionModel>>()))
				.Returns(new List<string>());

			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockStatusCachedService.Object, mockSRMAModelService.Object, mockFinancialPlanModelService.Object, mockNTIUnderConsiderationModelService.Object, mockNTIWarningLetterModelService.Object, mockNTIModelService.Object, mockCaseActionValidator.Object, mockLogger.Object, true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var caseModel = pageModel.CaseModel;
			var trustDetailsModel = pageModel.TrustDetailsModel;

			// assert
			Assert.IsNull(caseModel);
			Assert.IsNull(trustDetailsModel);

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			Assert.That(pageModel.TempData["OpenActions.Message"], Is.Not.Null);
			Assert.IsTrue((pageModel.TempData["OpenActions.Message"] as List<string>).Contains("Resolve Concerns"));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingRoutes_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNTIUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNTIWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var mockNTIModelService = new Mock<INtiModelService>();
			var mockCaseActionValidator = new Mock<ICaseActionValidator>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockStatusCachedService.Object, mockSRMAModelService.Object, mockFinancialPlanModelService.Object, mockNTIUnderConsiderationModelService.Object, mockNTIWarningLetterModelService.Object, mockNTIModelService.Object, mockCaseActionValidator.Object, mockLogger.Object, true);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostCloseCase_RedirectToHomePage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNTIUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNTIWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var mockNTIModelService = new Mock<INtiModelService>();
			var mockCaseActionValidator = new Mock<ICaseActionValidator>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var closedRecordModel = RecordFactory.BuildRecordModel(3);
			var recordsList = new List<RecordModel>() { closedRecordModel };
			var liveStatusDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);

			mockCaseModelService.Setup(c => c.PatchClosure(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockStatusCachedService.Object, mockSRMAModelService.Object, mockFinancialPlanModelService.Object, mockNTIUnderConsiderationModelService.Object, mockNTIWarningLetterModelService.Object, mockNTIModelService.Object, mockCaseActionValidator.Object, mockLogger.Object, true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("case-outcomes") }
				});
			
			// act
			var actionResult = await pageModel.OnPostCloseCase();
			var redirectResult = actionResult as RedirectResult;

			// assert
			Assert.That(actionResult, Is.AssignableFrom<RedirectResult>());
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("/"));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}

		[Test]
		public async Task WhenOnPostCloseCase_MissingRoutes_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNTIUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNTIWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var mockNTIModelService = new Mock<INtiModelService>();
			var mockCaseActionValidator = new Mock<ICaseActionValidator>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockStatusCachedService.Object, mockSRMAModelService.Object, mockFinancialPlanModelService.Object, mockNTIUnderConsiderationModelService.Object, mockNTIWarningLetterModelService.Object, mockNTIModelService.Object, mockCaseActionValidator.Object, mockLogger.Object, true);

			// act
			var actionResult = await pageModel.OnPostCloseCase();
			var redirectResult = actionResult as RedirectResult;
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(actionResult, Is.AssignableFrom<RedirectResult>());
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("closure"));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostCloseCase_MissingFormValues_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();
			var mockSRMAModelService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNTIUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockNTIWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var mockNTIModelService = new Mock<INtiModelService>();
			var mockCaseActionValidator = new Mock<ICaseActionValidator>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockStatusCachedService.Object, mockSRMAModelService.Object, mockFinancialPlanModelService.Object, mockNTIUnderConsiderationModelService.Object, mockNTIWarningLetterModelService.Object, mockNTIModelService.Object, mockCaseActionValidator.Object, mockLogger.Object, true);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("") }
				});
			
			// act
			var actionResult = await pageModel.OnPostCloseCase();
			var redirectResult = actionResult as RedirectResult;

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(actionResult, Is.AssignableFrom<RedirectResult>());
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("closure"));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		private static ClosurePageModel SetupClosurePageModel(
			ICaseModelService mockCaseModelService, ITrustModelService mockTrustModelService, IRecordModelService mockRecordModelService, IStatusCachedService mockStatusCachedService, ISRMAService mockSRMAModelService, IFinancialPlanModelService mockFinancialPlanModelService, INtiUnderConsiderationModelService mockNTIUnderConsiderationService, INtiWarningLetterModelService mockNTIWarningLetterModelService, INtiModelService mockNTIModelService, ICaseActionValidator mockCaseActionValidator, ILogger<ClosurePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClosurePageModel(mockCaseModelService, mockTrustModelService, mockRecordModelService, mockStatusCachedService, mockSRMAModelService, mockFinancialPlanModelService, mockNTIUnderConsiderationService, mockNTIWarningLetterModelService, mockNTIModelService, mockCaseActionValidator, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}