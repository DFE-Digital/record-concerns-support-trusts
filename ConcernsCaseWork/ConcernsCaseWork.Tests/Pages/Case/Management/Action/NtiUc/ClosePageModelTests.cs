using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
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
using Service.Redis.FinancialPlan;
using Service.Redis.Nti;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiUc
{
	[Parallelizable(ParallelScope.All)]
	public class ClosePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiModelService> mockNtiModelService = new Mock<INtiModelService>();
			Mock<INtiStatusesCachedService> mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			Mock<INtiModelService> mockNtiModelService = new Mock<INtiModelService>();
			Mock<INtiStatusesCachedService> mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::ClosePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiModelService> mockNtiModelService = new Mock<INtiModelService>();
			Mock<INtiStatusesCachedService> mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);

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
		public async Task WhenOnPostAsync_ValidatesNotesLength_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiModelService> mockNtiModelService = new Mock<INtiModelService>();
			Mock<INtiStatusesCachedService> mockNtiStatusesCachedService = new Mock<INtiStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiStatusesCachedService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		private static ClosePageModel SetupAddPageModel(
			Mock<INtiModelService> mockNtiModelService,
			Mock<INtiStatusesCachedService> mockNtiStatusesCachedService,
			Mock<ILogger<ClosePageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ClosePageModel(mockNtiModelService.Object, mockNtiStatusesCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}


}