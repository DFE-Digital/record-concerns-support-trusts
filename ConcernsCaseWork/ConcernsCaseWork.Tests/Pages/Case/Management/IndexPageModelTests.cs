﻿/*using AutoFixture;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.Fixtures)]
	[TestFixture]
	public class IndexPageModelTests
	{
		private Mock<ICaseModelService> _mockCaseModelService = null;
		private Mock<ITrustModelService> _mockTrustModelService = null;
		private Mock<IRecordModelService> _mockRecordModelService = null;
		private Mock<IRatingModelService> _mockRatingModelService = null;
		private Mock<IStatusCachedService> _mockStatusCachedService = null;
		private Mock<ILogger<IndexPageModel>> _mockLogger = null;
		private Mock<INtiUnderConsiderationStatusesCachedService> _mockNtiStatusesCachedService = null;
		private Mock<IActionsModelService> _actionsModelService = null;
		private Mock<ICaseSummaryService> _caseSummaryService = null;
		private Mock<ICasePermissionsService> _casePermissionsService = null;

		private readonly static Fixture _fixture = new();

		[SetUp]
		public void SetUp()
		{
			_mockCaseModelService = new Mock<ICaseModelService>();
			_mockTrustModelService = new Mock<ITrustModelService>();
			_mockRecordModelService = new Mock<IRecordModelService>();
			_mockRatingModelService = new Mock<IRatingModelService>();
			_mockStatusCachedService = new Mock<IStatusCachedService>();
			_mockLogger = new Mock<ILogger<IndexPageModel>>();
			_mockNtiStatusesCachedService = new Mock<INtiUnderConsiderationStatusesCachedService>();
			_actionsModelService = new Mock<IActionsModelService>();
			_caseSummaryService = new Mock<ICaseSummaryService>();

			_casePermissionsService = new Mock<ICasePermissionsService>();
			_casePermissionsService.Setup(m => m.GetCasePermissions(It.IsAny<long>())).ReturnsAsync(new GetCasePermissionsResponse());
		}

		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var pageModel = SetupIndexPageModel();

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));

			_mockCaseModelService.Verify(c =>
				c.GetCaseByUrn(It.IsAny<long>()), Times.Never);
			_mockTrustModelService.Verify(c =>
				c.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public async Task WhenOnGetAsync_ModelDataIsBuilt()
		{
			// arrange
			var urn = 3;

			SetupDefaultModels();

			var openActions = _fixture
				.Build<ActionSummaryModel>()
				.Without(a => a.ClosedDate)
				.CreateMany()
				.ToList();

			var closedActions = _fixture.CreateMany<ActionSummaryModel>().ToList();

			var actionBreakdown = new ActionSummaryBreakdownModel();
			actionBreakdown.OpenActions = openActions;
			actionBreakdown.ClosedActions = closedActions;

			_actionsModelService.Setup(m => m.GetActionsSummary(It.IsAny<long>())).ReturnsAsync(actionBreakdown);

			var caseModel = _fixture.Create<CaseModel>();
			_mockCaseModelService.Setup(m => m.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseModel);

			var rating = _fixture.Create<RatingModel>();
			_mockRatingModelService.Setup(m => m.GetRatingModelById(It.IsAny<long>())).ReturnsAsync(rating);

			var records = _fixture.CreateMany<RecordModel>().ToList();
			_mockRecordModelService.Setup(m => m.GetRecordsModelByCaseUrn(It.IsAny<long>())).ReturnsAsync(records);

			var trustDetails = _fixture.Create<TrustDetailsModel>();
			_mockTrustModelService.Setup(m => m.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(trustDetails);

			var activeCases = _fixture.CreateMany<ActiveCaseSummaryModel>().ToList();
			_caseSummaryService.Setup(m => m.GetActiveCaseSummariesByTrust(It.IsAny<string>())).ReturnsAsync(activeCases);
			
			var closedCases = _fixture.CreateMany<ClosedCaseSummaryModel>().ToList();
			_caseSummaryService.Setup(m => m.GetClosedCaseSummariesByTrust(It.IsAny<string>())).ReturnsAsync(closedCases);

			var pageModel = SetupIndexPageModel();
			pageModel.RouteData.Values.Add("urn", urn);

			// act
			var page = await pageModel.OnGetAsync();

			// assert
			PageLoadedWithoutError(pageModel);

			pageModel.OpenCaseActions.Should().BeEquivalentTo(openActions);
			pageModel.ClosedCaseActions.Should().BeEquivalentTo(closedActions);
			pageModel.CaseModel.RatingModel.Should().BeEquivalentTo(rating);
			pageModel.CaseModel.Urn.Should().Be(caseModel.Urn);
			pageModel.CaseModel.RecordsModel.Should().BeEquivalentTo(records);
			pageModel.TrustDetailsModel.Should().BeEquivalentTo(trustDetails);
			pageModel.ActiveCases.Should().BeEquivalentTo(activeCases);
			pageModel.ClosedCases.Should().BeEquivalentTo(closedCases);
		}

		[Test]
		public async Task WhenOnGetAsync_WhenCaseIsClosed_RedirectsToClosedCasePage()
		{
			var closedStatusId = 3;
			SetupDefaultModels();

			var caseModel = CaseFactory.BuildCaseModel(statusId: closedStatusId);
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), closedStatusId);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(StatusEnum.Close.ToString()))
				.ReturnsAsync(closedStatusModel);

			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			var result = await pageModel.OnGetAsync();

			// assert
			Assert.That(result, Is.TypeOf<RedirectResult>());
			Assert.That(((RedirectResult)result).Url, Is.EqualTo($"/case/1/closed"));
		}

		[Test]
		public async Task WhenUserHasEditCasePrivileges_ShowEditActions_Return_False()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel();
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsModel);

			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.False(showEditActions);
		}

		[Test]
		public async Task WhenUserHasEditCasePrivileges_ShowEditActions_Return_True()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel("Tester");
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closeStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closeStatusModel);

			var permissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };
			_casePermissionsService.Setup(m => m.GetCasePermissions(It.IsAny<long>())).ReturnsAsync(permissionsResponse);

			var pageModel = SetupIndexPageModel(isAuthenticated: true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.True(showEditActions);
		}

		[Test]
		public async Task WhenCaseIsClosed_ShowEditActions_Return_False()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel("Tester", 3);
			var trustCasesModel = CaseFactory.BuildListTrustCasesModel();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			var closeStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closeStatusModel);

			var pageModel = SetupIndexPageModel(isAuthenticated: true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.False(showEditActions);
		}

		private IndexPageModel SetupIndexPageModel(
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(_mockCaseModelService.Object,
				_mockTrustModelService.Object,
				_mockRecordModelService.Object,
				_mockRatingModelService.Object,
				_mockStatusCachedService.Object,
				_mockNtiStatusesCachedService.Object,
				_mockLogger.Object,
				_actionsModelService.Object,
				_caseSummaryService.Object,
				_casePermissionsService.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private void SetupDefaultModels()
		{
			var urn = 3;
			var caseModel = CaseFactory.BuildCaseModel("Tester", urn);
			var closedStatusModel = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 1);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			_mockStatusCachedService.Setup(s => s.GetStatusByName(It.IsAny<string>()))
				.ReturnsAsync(closedStatusModel);
			_mockNtiStatusesCachedService.Setup(s => s.GetAllStatuses())
				.ReturnsAsync(new List<NtiUnderConsiderationStatusDto>());
			_actionsModelService.Setup(m => m.GetActionsSummary(It.IsAny<long>()))
				.ReturnsAsync(new ActionSummaryBreakdownModel());
		}

		private void PageLoadedWithoutError(IndexPageModel pageModel)
		{
			pageModel.TempData["Error.Message"].Should().BeNull();
		}
	}
}*/