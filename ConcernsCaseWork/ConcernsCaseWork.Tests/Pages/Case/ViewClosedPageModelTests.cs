﻿using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Configuration;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using ConcernsCaseWork.Tests.Helpers;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class ViewClosedPageModelTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsExceptionAndCallsLogger()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ViewClosedPageModel>>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockActionsModelService = new Mock<IActionsModelService>();

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockLogger.Object);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasCalled("ViewClosedPageModel::OnGetAsync::Exception - CaseUrn is null or invalid to parse");

			Assert.That(pageModel.TempData["Error.Message"],
				Is.EqualTo(ErrorConstants.ErrorOnGetPage));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn( It.IsAny<long>()), Times.Never);
			mockActionsModelService.Verify(c => c.GetActionsSummary(It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetAsync_RouteCaseUrn_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ViewClosedPageModel>>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockActionsModelService = new Mock<IActionsModelService>();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.Throws<Exception>();

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasCalled("ViewClosedPageModel::OnGetAsync::Exception - Exception of type 'System.Exception' was thrown.");

			Assert.That(pageModel.TempData["Error.Message"],
				Is.EqualTo(ErrorConstants.ErrorOnGetPage));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn( It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetAsync_RouteCaseUrn_Returns_CaseModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ViewClosedPageModel>>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockActionsModelService = new Mock<IActionsModelService>();

			var caseModel = CaseFactory.BuildCaseModel();
			caseModel.StatusId = (int)CaseStatus.Close;

			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();

			var actionBreakdown = new ActionSummaryBreakdownModel();

			var openActions = _fixture
				.Build<ActionSummaryModel>()
				.Without(a => a.ClosedDate)
				.CreateMany().ToList();

			actionBreakdown.OpenActions = openActions;

			var closedActions = _fixture.CreateMany<ActionSummaryModel>().ToList();
			actionBreakdown.ClosedActions = closedActions;

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn( It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			mockActionsModelService.Setup(a => a.GetActionsSummary(It.IsAny<long>()))
				.ReturnsAsync(actionBreakdown);

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasNotCalled();
			Assert.IsNull(pageModel.TempData["Error.Message"]);
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.CaseModel.Description, Is.EqualTo(caseModel.Description));
			Assert.That(pageModel.CaseModel.Issue, Is.EqualTo(caseModel.Issue));
			Assert.That(pageModel.CaseModel.StatusId, Is.EqualTo(caseModel.StatusId));
			Assert.That(pageModel.CaseModel.Urn, Is.EqualTo(caseModel.Urn));
			Assert.That(pageModel.CaseModel.ClosedAt, Is.EqualTo(caseModel.ClosedAt));
			Assert.That(pageModel.CaseModel.CreatedAt, Is.EqualTo(caseModel.CreatedAt));
			Assert.That(pageModel.CaseModel.CreatedBy, Is.EqualTo(caseModel.CreatedBy));
			Assert.That(pageModel.CaseModel.CrmEnquiry, Is.EqualTo(caseModel.CrmEnquiry));
			Assert.That(pageModel.CaseModel.CurrentStatus, Is.EqualTo(caseModel.CurrentStatus));
			Assert.That(pageModel.CaseModel.DeEscalation, Is.EqualTo(caseModel.DeEscalation));
			Assert.That(pageModel.CaseModel.NextSteps, Is.EqualTo(caseModel.NextSteps));
			Assert.That(pageModel.CaseModel.CaseAim, Is.EqualTo(caseModel.CaseAim));
			Assert.That(pageModel.CaseModel.DeEscalationPoint, Is.EqualTo(caseModel.DeEscalationPoint));
			Assert.That(pageModel.CaseModel.ReviewAt, Is.EqualTo(caseModel.ReviewAt));
			Assert.That(pageModel.CaseModel.UpdatedAt, Is.EqualTo(caseModel.UpdatedAt));
			Assert.That(pageModel.CaseModel.DirectionOfTravel, Is.EqualTo(caseModel.DirectionOfTravel));
			Assert.That(pageModel.CaseModel.ReasonAtReview, Is.EqualTo(caseModel.ReasonAtReview));
			Assert.That(pageModel.CaseModel.TrustUkPrn, Is.EqualTo(caseModel.TrustUkPrn));

			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupName, Is.EqualTo(trustDetailsModel.GiasData.GroupName));
			Assert.That(pageModel.TrustDetailsModel.GiasData.GroupNameTitle, Is.EqualTo(trustDetailsModel.GiasData.GroupName.ToTitle()));

			var expectedRecordsModel = pageModel.CaseModel.RecordsModel;
			for (var index = 0; index < expectedRecordsModel.Count; ++index)
			{
				Assert.That(expectedRecordsModel.ElementAt(index).Id, Is.EqualTo(recordsModel.ElementAt(index).Id));
				Assert.That(expectedRecordsModel.ElementAt(index).CaseUrn, Is.EqualTo(recordsModel.ElementAt(index).CaseUrn));
				Assert.That(expectedRecordsModel.ElementAt(index).RatingId, Is.EqualTo(recordsModel.ElementAt(index).RatingId));
				Assert.That(expectedRecordsModel.ElementAt(index).StatusId, Is.EqualTo(recordsModel.ElementAt(index).StatusId));
				Assert.That(expectedRecordsModel.ElementAt(index).TypeId, Is.EqualTo(recordsModel.ElementAt(index).TypeId));
			}
			
			Assert.That(pageModel.CaseActions, Is.EquivalentTo(closedActions.ToList()));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn( It.IsAny<long>()), Times.Once);
			mockActionsModelService.Verify(c => c.GetActionsSummary(It.IsAny<long>()), Times.Once);
		}
		
		
		[Test]
		public async Task WhenOnGetAsync_WhenCaseIsOpen_RedirectsToCaseManagementPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ViewClosedPageModel>>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockActionsModelService = new Mock<IActionsModelService>();

			var caseModel = CaseFactory.BuildCaseModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedActions = ActionsSummaryFactory.BuildListOfActionSummaries().ToList();

			var actionBreakdown = new ActionSummaryBreakdownModel()
			{
				ClosedActions = closedActions
			};

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn( It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			mockActionsModelService.Setup(a => a.GetActionsSummary(It.IsAny<long>()))
				.ReturnsAsync(actionBreakdown);

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			var result = await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasNotCalled();
			Assert.That(((RedirectResult)result).Url, Is.EqualTo($"/case/{caseModel.Urn}/management"));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn( It.IsAny<long>()), Times.Never);
			mockActionsModelService.Verify(c => c.GetActionsSummary(It.IsAny<long>()), Times.Never);
		}

		private static ViewClosedPageModel SetupViewClosedPageModel(
			ICaseModelService mockCaseModelService,
			ITrustModelService mockTrustModelService,
			IRecordModelService mockRecordModelService,
			IActionsModelService mockActionsModelService,
			ILogger<ViewClosedPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			Mock <IOptions<SiteOptions>> options = new Mock<IOptions<SiteOptions>>();
			options.Setup(m => m.Value).Returns(_fixture.Create<SiteOptions>());

			return new ViewClosedPageModel(mockCaseModelService, mockTrustModelService, 
				mockRecordModelService, mockActionsModelService, mockLogger,MockTelemetry.CreateMockTelemetryClient(),options.Object)
			{
				PageContext = pageContext, TempData = tempData, Url = new UrlHelper(actionContext), MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}