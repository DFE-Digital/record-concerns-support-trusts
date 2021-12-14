using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Management
{
	[Parallelizable(ParallelScope.All)]
	public class EditDirectionOfTravelPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditDirectionOfTravelPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			
			var pageModel = SetupEditDirectionOfTravelPageModel(mockCaseModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditDirectionOfTravelPageModel>>();
			
			var pageModel = SetupEditDirectionOfTravelPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostEditDirectionOfTravel_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditDirectionOfTravelPageModel>>();

			var pageModel = SetupEditDirectionOfTravelPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			// act
			var pageResponse = await pageModel.OnPostEditDirectionOfTravel("https://returnto/thispage");
			
			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostEditDirectionOfTravel_RouteData_MissingRequestForm_ReloadPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditDirectionOfTravelPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			
			var pageModel = SetupEditDirectionOfTravelPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "direction-of-travel", new StringValues("") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditDirectionOfTravel("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockCaseModelService.Verify(c => 
				c.PatchDirectionOfTravel(It.IsAny<PatchCaseModel>()), Times.Never);
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostEditDirectionOfTravel_RouteData_RequestForm_ReturnsToPreviousUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditDirectionOfTravelPageModel>>();

			mockCaseModelService.Setup(c => c.PatchDirectionOfTravel(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditDirectionOfTravelPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "direction-of-travel", new StringValues("direction-of-travel") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditDirectionOfTravel("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			Assert.That(page.Url, Is.EqualTo("https://returnto/thispage"));
			
			mockCaseModelService.Verify(c => 
				c.PatchDirectionOfTravel(It.IsAny<PatchCaseModel>()), Times.Once);
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}
		
		private static EditDirectionOfTravelPageModel SetupEditDirectionOfTravelPageModel(
			ICaseModelService mockCaseModelService, ILogger<EditDirectionOfTravelPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditDirectionOfTravelPageModel(mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}