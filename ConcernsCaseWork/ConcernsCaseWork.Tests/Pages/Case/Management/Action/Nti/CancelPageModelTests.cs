using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Services.Nti;
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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	public class StringLengthValidatedType
	{
		[MinLength(2001)]
		public string Notes { get; set; }
	}

	[Parallelizable(ParallelScope.All)]
	public class CancelPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1); 
			routeData.Add("ntiId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("CancelPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockNtiModelService.Verify(n => n.GetNtiByIdAsync(It.IsAny<long>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenNtiIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseUrn = 3;
			var ntiId = 9;
			
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var ntiModel = NTIFactory.BuildClosedNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn); 
			routeData.Add("ntiId", ntiId);

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("CancelPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);

			mockNtiModelService.Verify(n => n.GetNtiByIdAsync(It.IsAny<long>()), Times.Once);
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/nti/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

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
		public async Task WhenOnPostAsync_ValidatesNotes_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiId", 1);

			var fixture = new Fixture();
			var createdString = fixture.Create<StringLengthValidatedType>();

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "nti-notes", new StringValues(createdString.Notes) }
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

		[Test]
		public async Task WhenOnPostAsync_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiId", 1);

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "nti-notes", new StringValues("valid string") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			mockNtiModelService.Verify(n => n.PatchNtiAsync(It.IsAny<NtiModel>()), Times.Once);
		}

		private static CancelPageModel SetupCancelPageModel(
			Mock<INtiModelService> mockNtiModelService,
			Mock<ILogger<CancelPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new CancelPageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}