using ConcernsCaseWork.Pages.Case.Management.Action.SRMA;
using ConcernsCaseWork.Services.Cases;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

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
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::SRMA::AddPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_Missing_Status_FormData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", new StringValues("") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_Invalid_SRMA_Status_FormData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", new StringValues("random") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_Invalid_Date_FormData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", new StringValues("PreparingForDeployment") },
					{ "dtr-day", new StringValues("00") },
					{ "dtr-month", new StringValues("00") },
					{ "dtr-year", new StringValues("0000") },
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["SRMA.Message"], Is.EqualTo("SRMA offered date is not valid 00-00-0000"));
		}

		[Test]
		public async Task WhenOnPostAsync_Invalid_NotesLength_FormData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			var exceededNotesLength = "Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data Test Data";

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", new StringValues("PreparingForDeployment") },
					{ "dtr-day", new StringValues("13") },
					{ "dtr-month", new StringValues("04") },
					{ "dtr-year", new StringValues("2022") },
					{ "srma-notes", new StringValues(exceededNotesLength) },
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_IsValid_SRMA_Is_Created_ReturnsToManagementPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);
			var caseUrn = 1;

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", new StringValues("PreparingForDeployment") },
					{ "dtr-day", new StringValues("13") },
					{ "dtr-month", new StringValues("04") },
					{ "dtr-year", new StringValues("2022") },
					{ "srma-notes", new StringValues("Test Data") },
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.IsEmpty(pageModel.TempData);
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo($"/case/{caseUrn}/management"));
		}

		private static AddPageModel SetupAddPageModel(
			ISRMAService mockSrmaService,
			ILogger<AddPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockSrmaService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}


}