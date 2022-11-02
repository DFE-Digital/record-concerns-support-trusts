using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
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
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsExceptionAndCallsLogger()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ViewClosedPageModel>>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockActionsModelService = new Mock<IActionsModelService>();
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockStatusCachedService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasCalled("ViewClosedPageModel::OnGetAsync::Exception - CaseUrn is null or invalid to parse");

			Assert.That(pageModel.TempData["Error.Message"],
				Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockActionsModelService.Verify(c => c.GetClosedActionsSummary(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.Throws<Exception>();

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockStatusCachedService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasCalled("ViewClosedPageModel::OnGetAsync::Exception - Exception of type 'System.Exception' was thrown.");

			Assert.That(pageModel.TempData["Error.Message"],
				Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var caseModel = CaseFactory.BuildCaseModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedActions = ActionsSummaryFactory.BuildListOfActionSummaries();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			mockActionsModelService.Setup(a => a.GetClosedActionsSummary(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(closedActions);
			mockStatusCachedService.Setup(a => a.GetStatusByName(StatusEnum.Close.ToString()))
				.ReturnsAsync(new StatusDto("Closed", DateTimeOffset.Now, DateTimeOffset.Now, caseModel.StatusId));

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockStatusCachedService.Object, mockLogger.Object);

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
			Assert.That(pageModel.CaseModel.StatusName, Is.EqualTo(caseModel.StatusName));
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
				var expectedRecordRatingModel = expectedRecordsModel.ElementAt(index).RatingModel;
				var actualRecordRatingModel = recordsModel.ElementAt(index).RatingModel;
				Assert.NotNull(expectedRecordRatingModel);
				Assert.NotNull(actualRecordRatingModel);
				Assert.That(expectedRecordRatingModel.Checked, Is.EqualTo(actualRecordRatingModel.Checked));
				Assert.That(expectedRecordRatingModel.Name, Is.EqualTo(actualRecordRatingModel.Name));
				Assert.That(expectedRecordRatingModel.Id, Is.EqualTo(actualRecordRatingModel.Id));
				Assert.That(expectedRecordRatingModel.RagRating, Is.EqualTo(actualRecordRatingModel.RagRating));
				Assert.That(expectedRecordRatingModel.RagRatingCss, Is.EqualTo(actualRecordRatingModel.RagRatingCss));

				var expectedRecordTypeModel = expectedRecordsModel.ElementAt(index).TypeModel;
				var actualRecordTypeModel = recordsModel.ElementAt(index).TypeModel;
				Assert.NotNull(expectedRecordTypeModel);
				Assert.NotNull(actualRecordTypeModel);
				Assert.That(expectedRecordTypeModel.Type, Is.EqualTo(actualRecordTypeModel.Type));
				Assert.That(expectedRecordTypeModel.SubType, Is.EqualTo(actualRecordTypeModel.SubType));
				Assert.That(expectedRecordTypeModel.TypeDisplay, Is.EqualTo(actualRecordTypeModel.TypeDisplay));
				Assert.That(expectedRecordTypeModel.TypesDictionary, Is.EqualTo(actualRecordTypeModel.TypesDictionary));

				var expectedRecordStatusModel = expectedRecordsModel.ElementAt(index).StatusModel;
				var actualRecordStatusModel = recordsModel.ElementAt(index).StatusModel;
				Assert.NotNull(expectedRecordStatusModel);
				Assert.NotNull(actualRecordTypeModel);
				Assert.That(expectedRecordStatusModel.Name, Is.EqualTo(actualRecordStatusModel.Name));
				Assert.That(expectedRecordStatusModel.Id, Is.EqualTo(actualRecordStatusModel.Id));
			}
			
			Assert.That(pageModel.CaseActions, Is.EquivalentTo(closedActions.ToList()));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockActionsModelService.Verify(c => c.GetClosedActionsSummary(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
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
			var mockStatusCachedService = new Mock<IStatusCachedService>();

			var caseModel = CaseFactory.BuildCaseModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closedActions = ActionsSummaryFactory.BuildListOfActionSummaries();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			mockActionsModelService.Setup(a => a.GetClosedActionsSummary(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(closedActions);
			mockStatusCachedService.Setup(a => a.GetStatusByName(StatusEnum.Close.ToString()))
				.ReturnsAsync(new StatusDto("Open", DateTimeOffset.Now, DateTimeOffset.Now, 99999));

			var pageModel = SetupViewClosedPageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockRecordModelService.Object, mockActionsModelService.Object,
				mockStatusCachedService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			var result = await pageModel.OnGetAsync();

			// assert
			mockLogger.VerifyLogErrorWasNotCalled();
			Assert.That(((RedirectResult)result).Url, Is.EqualTo($"/case/{caseModel.Urn}/management"));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockTrustModelService.Verify(c => c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			mockRecordModelService.Verify(c => c.GetRecordsModelByCaseUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockActionsModelService.Verify(c => c.GetClosedActionsSummary(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}

		private static ViewClosedPageModel SetupViewClosedPageModel(
			ICaseModelService mockCaseModelService,
			ITrustModelService mockTrustModelService,
			IRecordModelService mockRecordModelService,
			IActionsModelService mockActionsModelService,
			IStatusCachedService mockStatusCachedService,
			ILogger<ViewClosedPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ViewClosedPageModel(mockCaseModelService, mockTrustModelService, mockRecordModelService, mockActionsModelService, mockStatusCachedService, mockLogger)
			{
				PageContext = pageContext, TempData = tempData, Url = new UrlHelper(actionContext), MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}