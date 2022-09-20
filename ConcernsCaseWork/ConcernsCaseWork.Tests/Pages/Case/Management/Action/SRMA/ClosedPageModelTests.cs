using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

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
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.SRMAModel);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::SRMA::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_RedirectToIndexPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			var pageResponse = await pageModel.OnPostAsync();
			var pageResponseInstance = pageResponse as RedirectToPageResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.PageName, Is.EqualTo("index"));
		}

		[Test]
		public async Task WhenOnGetDeclineComplete_SRMA_Status_IsDeployed_Valid_SRMA_RedirectToResolvePage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, SRMAReasonOffered.OfferLinked);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			var pageResponse = await pageModel.OnGetDeclineComplete();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo("resolve/complete"));
		}

		[Test]
		public async Task WhenOnGetDeclineComplete_SRMA_Status_IsDeployed_Invalid_SRMA_RedirectToSRMAIndexPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, SRMAReasonOffered.Unknown);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			var pageResponse = await pageModel.OnGetDeclineComplete();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo($"/case/{1}/management/action/srma/{1}"));
		}

		[Test]
		public async Task WhenOnGetDeclineComplete_SRMA_Status_IsNotDeployed_Invalid_SRMA_RedirectToSRMAIndexPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.TrustConsidering, SRMAReasonOffered.Unknown);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			var pageResponse = await pageModel.OnGetDeclineComplete();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo($"/case/{1}/management/action/srma/{1}"));
		}

		[Test]
		public async Task WhenOnGetDeclineComplete_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetDeclineComplete();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetCancel_MissingSrmaId_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetCancel();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetCancel_Invalid_SRMA_RedirectToSRMAIndexPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.PreparingForDeployment, SRMAReasonOffered.Unknown);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			var pageResponse = await pageModel.OnGetCancel();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo($"/case/{1}/management/action/srma/{1}"));
		}

		[Test]
		public async Task WhenOnGetCancel_Valid_SRMA_RedirectToSRMAIndexPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.PreparingForDeployment, SRMAReasonOffered.RegionsGroupIntervention);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupIndexPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("srmaId", 1);

			// act
			var pageResponse = await pageModel.OnGetCancel();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo("resolve/cancel"));
		}

		private static IndexPageModel SetupIndexPageModel(
			ISRMAService mockSrmaService,
			ILogger<IndexPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockSrmaService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}


}