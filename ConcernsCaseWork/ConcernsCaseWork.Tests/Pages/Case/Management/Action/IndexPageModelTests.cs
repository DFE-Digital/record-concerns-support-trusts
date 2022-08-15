using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Pages.Case.Management.Action;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
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
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));


			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::IndexPageModel::OnPostAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_MissingFormData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "action", new StringValues("") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_ActionIs_SRMA_ReturnsToAddSRMAPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);


			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			var expectedRedirectLink = "srma/add";

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "action", new StringValues(CaseActionEnum.Srma.ToString()) }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
			var page = pageResponse as RedirectToPageResult;

			Assert.IsEmpty(pageModel.TempData);
			Assert.That(page, Is.Not.Null);
			Assert.That(page.PageName, Is.EqualTo(expectedRedirectLink));
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_ActionIs_Unknown_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "action", new StringValues("unknown") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_ActionIs_SRMA_SRMA_Already_Exists_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockCaseModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "action", new StringValues(CaseActionEnum.Srma.ToString()) }
				});

			var srmaModelList = SrmaFactory.BuildListSrmaModel(SRMAStatus.TrustConsidering);

			mockSrmaService.Setup(s => s.GetSRMAsForCase(It.IsAny<long>())).ReturnsAsync(srmaModelList);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["CaseAction.Error"], Is.EqualTo("There is already an open SRMA action linked to this case. Please resolve that before opening another one."));
		}

		private static IndexPageModel SetupIndexPageModel(
			ICaseModelService mockCaseModelService,
			ISRMAService mockSrmaService,
			IFinancialPlanModelService mockFinancialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			ILogger<IndexPageModel> mockLogger,
			INtiWarningLetterModelService ntiWarningLetterModelService = null,
			INtiModelService ntiModelService = null,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockCaseModelService, mockSrmaService, mockFinancialPlanModelService, ntiUnderConsiderationModelService
				, ntiWarningLetterModelService ?? CreateMock<INtiWarningLetterModelService>()
				, ntiModelService ?? CreateMock<INtiModelService>()
				, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private static T CreateMock<T>() where T : class
		{
			var moq = new Mock<T>();
			return moq.Object;
		}
	}


}