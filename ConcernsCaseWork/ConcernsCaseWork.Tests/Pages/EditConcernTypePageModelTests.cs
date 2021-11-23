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
using System;
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
			var casesDto = CaseFactory.BuildCaseModel();

			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(casesDto);

			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.CaseModel.TypesDictionary, Is.Empty);
		}

		[Test]
		public async Task WhenOnGetAsync_RouteData_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();
			
			var caseModel = CaseFactory.BuildCaseModel();
			
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.CaseModel.TypesDictionary, Is.Empty);
		}

		[Test]
		public void WhenOnPostEditConcernType_MissingRouteData_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();
			var casesDto = CaseFactory.BuildCaseModel();

			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);

			// act/assert
			Assert.ThrowsAsync<Exception>(() => pageModel.OnPostEditConcernType("https://returnto/thispage"));
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockTypeModelService.Verify(c => c.GetTypes(), Times.Never);
		}

		[TestCase("", "")]
		[TestCase(null, null)]
		[TestCase("test", "")]
		public async Task WhenOnPostEditConcernType_RouteData_MissingRequestForm_ReloadPage(string type, string subType)
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();

			var caseModel = CaseFactory.BuildCaseModel(type, subType);
			
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues(type) },
					{ "subType", new StringValues(subType) }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.CaseModel.TypesDictionary, Is.Empty);
		}
		
		[TestCase("Force Majeure", "test")]
		[TestCase("Force Majeure", "")]
		[TestCase("Force Majeure", null)]
		[TestCase("Compliance", "Financial reporting")]
		[TestCase("Compliance", "Financial returns")]
		[TestCase("Financial", "Deficit")]
		[TestCase("Financial", "Projected deficit / Low future surplus")]
		[TestCase("Financial", "Cash flow shortfall")]
		[TestCase("Financial", "Clawback")]
		[TestCase("Governance", "Governance")]
		[TestCase("Irregularity", "Allegations and self reported concerns")]
		public async Task WhenOnPostEditConcernType_RouteData_RequestForm_ReturnsToPreviousUrl(string type, string subType)
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockLogger = new Mock<ILogger<EditConcernTypePageModel>>();

			mockCaseModelService.Setup(c => c.PatchConcernType(It.IsAny<PatchCaseModel>()));
			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			
			var pageModel = SetupEditConcernTypePageModel(mockCaseModelService.Object, mockTypeModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "type", new StringValues(type) },
					{ "subType", new StringValues(subType) }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditConcernType("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
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