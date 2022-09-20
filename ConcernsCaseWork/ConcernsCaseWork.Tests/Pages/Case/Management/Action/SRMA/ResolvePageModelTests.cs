using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Pages.Case.Management.Action.Srma;
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
	public class ResolvePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1); 
			routeData.Add("resolution", "complete"); 

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.SRMAModel);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::SRMA::ResolvePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_Invalid_SRMA_Resolution_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);
			routeData.Add("resolution", "invalid");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Complete_ReturnsPageModel()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);
			routeData.Add("resolution", "complete");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.ConfirmText, Is.EqualTo("Confirm SRMA is complete"));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Canceled_ReturnsPageModel()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);
			routeData.Add("resolution", "cancel");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.ConfirmText, Is.EqualTo("Confirm SRMA was cancelled"));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Declined_ReturnsPageModel()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);
			routeData.Add("resolution", "decline");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.ConfirmText, Is.EqualTo("Confirm SRMA was declined by trust"));
		}

		[Test]
		public async Task WhenOnPostAsync_MissingSrmaId_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			// act
			await pageModel.OnPostAsync();
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_ReturnsPageModel()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.SetNotes(It.IsAny<long>(), It.IsAny<string>()));
			mockSrmaService.Setup(s => s.SetStatus(It.IsAny<long>(), It.IsAny<SRMAStatus>()));

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);


			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);
			routeData.Add("resolution", "complete");


			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "srma-notes", new StringValues("srma-notes") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo($"/case/{1}/management"));
		}

		private static ResolvePageModel SetupResolvePageModel(
			ISRMAService mockSrmaService,
			ILogger<ResolvePageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ResolvePageModel(mockSrmaService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}