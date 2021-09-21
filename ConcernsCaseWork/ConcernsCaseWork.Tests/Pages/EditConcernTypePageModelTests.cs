using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Type;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class EditConcernTypePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();
			
			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);
			
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.TypesDictionary, Is.Empty);
		}
		
		[Test]
		public async Task WhenOnPostEditConcernType_MissingRouteData_ThrowsException_ReloadPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();
			
			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.TypesDictionary, Is.Empty);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostEditConcernType_RouteData_MissingRequestForm_ReloadPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();
			
			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("") },
					{ "subType", new StringValues("")
				}
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.TypesDictionary, Is.Empty);
		}
		
		[Test]
		public async Task WhenOnPostEditConcernType_RouteData_RequestForm_ReturnsToPreviousUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();

			mockCaseModelService.Setup(c => c.UpdateCase(It.IsAny<PatchCaseModel>()));
			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues("type") },
					{ "subType", new StringValues("subtype")
					}
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.Null);
			Assert.That(pageModel.TypesDictionary, Is.Null);
			Assert.That(page.Url, Is.EqualTo("https://returnto/thispage"));
		}
		
		private static EditConcernTypePageModel SetupEditConcernTypePageModel(
			ICaseModelService mockCaseModelService, ITypeModelService mockTypeModelService, ILogger<EditConcernTypePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditConcernTypePageModel(mockTypeModelService, mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}