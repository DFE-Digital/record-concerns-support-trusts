using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
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

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class EditRiskRatingPageModelTests
	{
		[Test]
		public void WhenOnGet_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditRiskRatingPageModel>>();
			
			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			var pageResponse = pageModel.OnGet();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
		}
		
		[Test]
		public async Task WhenOnPostEditRiskRating_MissingRouteData_ThrowsException_ReloadPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditRiskRatingPageModel>>();

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			// act
			var pageResponse = await pageModel.OnPostEditRiskRating("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostEditRiskRating_RouteData_MissingRequestForm_ReloadPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditRiskRatingPageModel>>();

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "riskRating", new StringValues("") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditRiskRating("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
		}
		
		[Test]
		public async Task WhenOnPostEditRiskRating_RouteData_RequestForm_ReturnsToPreviousUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditRiskRatingPageModel>>();

			mockCaseModelService.Setup(c => c.PatchConcernType(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "riskRating", new StringValues("riskRating") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditRiskRating("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.PreviousUrl, Is.Null);
			Assert.That(page.Url, Is.EqualTo("https://returnto/thispage"));
		}
		
		private static EditRiskRatingPageModel SetupEditRiskRatingPageModel(
			ICaseModelService mockCaseModelService, ILogger<EditRiskRatingPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditRiskRatingPageModel(mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}