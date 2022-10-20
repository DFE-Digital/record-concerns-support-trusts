using ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter;
using ConcernsCaseWork.Redis.NtiWarningLetter;
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiWL
{
	[Parallelizable(ParallelScope.All)]
	public class ClosePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			Mock<INtiWarningLetterStatusesCachedService> mockNtiWarningLetterStatusesCachedService = new Mock<INtiWarningLetterStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWarningLetterModelService, mockNtiWarningLetterStatusesCachedService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			Mock<INtiWarningLetterStatusesCachedService> mockNtiWarningLetterStatusesCachedService = new Mock<INtiWarningLetterStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWarningLetterModelService, mockNtiWarningLetterStatusesCachedService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1); 
			routeData.Add("ntiWLId", 1); 

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-WL::ClosePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			Mock<INtiWarningLetterStatusesCachedService> mockNtiWarningLetterStatusesCachedService = new Mock<INtiWarningLetterStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWarningLetterModelService, mockNtiWarningLetterStatusesCachedService, mockLogger);

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
		public async Task WhenOnPostAsync_ValidatesStatus_ThrowsException_ReturnsPage()
		{
			// arrange
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			Mock<INtiWarningLetterStatusesCachedService> mockNtiWarningLetterStatusesCachedService = new Mock<INtiWarningLetterStatusesCachedService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWarningLetterModelService, mockNtiWarningLetterStatusesCachedService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiWLId", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "status", new StringValues("Invalid value") }
				});

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
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService,
			Mock<INtiWarningLetterStatusesCachedService> mockNtiWarningLetterStatusesCachedService,
			Mock<ILogger<ClosePageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ClosePageModel(mockNtiWarningLetterModelService.Object, mockNtiWarningLetterStatusesCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}


}