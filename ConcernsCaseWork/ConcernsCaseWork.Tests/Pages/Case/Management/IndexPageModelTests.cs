using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Services.Actions;
using ConcernsCaseWork.Services.Cases;
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
		private Mock<ILogger<IndexPageModel>> _mockLogger = null;
		private Mock<IActionsModelService> _actionsModelService = null;
		private Mock<ICaseSummaryService> _caseSummaryService = null;
		private Mock<ICasePermissionsService> _casePermissionsService = null;
		private Mock<IUserStateCachedService> _mockUserStateCacheService = null;
		private Mock<ICaseValidatorService> _mockCloseCaseValidationService = null;

		private readonly static Fixture _fixture = new();

		[SetUp]
		public void SetUp()
		{
			_mockCaseModelService = new Mock<ICaseModelService>();
			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(CaseFactory.BuildCaseModel("Tester", (int)CaseStatus.Live));

			_mockTrustModelService = new Mock<ITrustModelService>();
			var trustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			_mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustDetailsModel);

			_mockRecordModelService = new Mock<IRecordModelService>();
			var recordsModel = RecordFactory.BuildListRecordModel();
			_mockRecordModelService.Setup(r => r.GetRecordsModelByCaseUrn(It.IsAny<long>()))
				.ReturnsAsync(recordsModel);

			_actionsModelService = new Mock<IActionsModelService>();
			_actionsModelService.Setup(m => m.GetActionsSummary(It.IsAny<long>()))
				.ReturnsAsync(new ActionSummaryBreakdownModel());

			_caseSummaryService = new Mock<ICaseSummaryService>();
			_caseSummaryService.Setup(m => m.GetActiveCaseSummariesByTrust(It.IsAny<string>(), 1))
					.ReturnsAsync(new CaseSummaryGroupModel<ActiveCaseSummaryModel>() { Pagination = new PaginationModel() });
			_caseSummaryService.Setup(m => m.GetClosedCaseSummariesByTrust(It.IsAny<string>(), 1))
					.ReturnsAsync(new CaseSummaryGroupModel<ClosedCaseSummaryModel>() { Pagination = new PaginationModel() });

			_mockUserStateCacheService = new Mock<IUserStateCachedService>();
			_mockUserStateCacheService.Setup(m => m.GetData(It.IsAny<string>())).ReturnsAsync(new UserState("Tester"));

			_casePermissionsService = new Mock<ICasePermissionsService>();
			_casePermissionsService.Setup(m => m.GetCasePermissions(It.IsAny<long>())).ReturnsAsync(new GetCasePermissionsResponse());

			_mockCloseCaseValidationService = new Mock<ICaseValidatorService>();
			_mockCloseCaseValidationService.Setup(m => m.ValidateClose(1)).ReturnsAsync(new List<CaseValidationErrorModel>());

			_mockLogger = new Mock<ILogger<IndexPageModel>>();
		}

		[Test]
		public async Task Get_When_CaseIsClosed_RedirectsToClosedCasePage()
		{
			var closedStatusId = (int)CaseStatus.Close;

			var caseModel = CaseFactory.BuildCaseModel(statusId: closedStatusId);

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);

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
		public async Task Get_WhenUserHasEditCasePrivileges_ShowEditActions_Return_False()
		{
			// arrange
			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.That(showEditActions, Is.False);
		}

		[Test]
		public async Task Get_WhenUserHasEditCasePrivileges_ShowEditActions_Return_True()
		{
			// arrange
			var permissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };
			_casePermissionsService.Setup(m => m.GetCasePermissions(It.IsAny<long>())).ReturnsAsync(permissionsResponse);

			var pageModel = SetupIndexPageModel(isAuthenticated: true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.That(showEditActions, Is.True);
		}

		[Test]
		public async Task Get_WhenCaseIsClosed_ShowEditActions_Return_False()
		{
			// arrange
			var caseModel = CaseFactory.BuildCaseModel("Tester", 3);
			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupIndexPageModel(isAuthenticated: true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var showEditActions = pageModel.IsEditableCase;

			// assert
			Assert.That(showEditActions, Is.False);
		}

		[Test]
		public async Task Post_WhenCaseHasNoOpenConcernsAndActions_Returns_NoValidationsErrors()
		{
			var pageModel = SetupIndexPageModel(isAuthenticated: true);
			pageModel.CaseUrn = 1;

			var result = await pageModel.OnPostCloseCaseAsync();

			var errors = pageModel.ModelState.GetValidationMessages();

			errors.Should().HaveCount(0);

			result.Should().BeAssignableTo<RedirectResult>();

			var redirect = result as RedirectResult;

			redirect.Url.Should().Be("/case/1/management/closure");
		}

		[Test]
		public async Task Post_WhenCaseHasOpenConcernsAndActions_Returns_ValidationsErrors()
		{
			_mockCloseCaseValidationService.Setup(m => m.ValidateClose(1)).ReturnsAsync(
				new List<CaseValidationErrorModel>() 
				{ 
					new CaseValidationErrorModel() { Type = CaseValidationError.Concern, Error = "Resolve concerns" },
					new CaseValidationErrorModel() { Type = CaseValidationError.CaseAction, Error = "Resolve SRMA" }
				});

			_mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(CaseFactory.BuildCaseModel("Tester", 1));

			var pageModel = SetupIndexPageModel(isAuthenticated: true);
			pageModel.CaseUrn = 1;

			await pageModel.OnPostCloseCaseAsync();

			var errors = pageModel.ModelState.GetValidationMessages().ToList();

			errors.Should().HaveCount(2);

			var concernError = errors[0];
			concernError.Key.Should().Be("Concerns");
			concernError.Value.Should().Be("Resolve concerns");

			var actionError = errors[1];
			actionError.Key.Should().Be("CaseActions");
			actionError.Value.Should().Be("Resolve SRMA");
		}

		private IndexPageModel SetupIndexPageModel(
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(_mockCaseModelService.Object,
				_mockTrustModelService.Object,
				_mockRecordModelService.Object,
				_mockLogger.Object,
				_actionsModelService.Object,
				_caseSummaryService.Object,
				_casePermissionsService.Object,
				_mockUserStateCacheService.Object,
				new ClaimsPrincipalHelper(),
				_mockCloseCaseValidationService.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}