using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Nti;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiUc
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiModelService> ntiunderConsiderationModel = new Mock<INtiModelService>();
			var mockNtiReasonsCachedService = new Mock<INtiReasonsCachedService>();
			Mock<ILogger<IndexPageModel>> mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(ntiunderConsiderationModel, mockNtiReasonsCachedService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			Mock<INtiModelService> mockNtiUnderConsiderationModelService = new Mock<INtiModelService>();
			var mockNtiReasonsCachedService = new Mock<INtiReasonsCachedService>();
			Mock<ILogger<IndexPageModel>> mockLogger = new Mock<ILogger<IndexPageModel>>();

			var ntiunderConsiderationModel = NTIUnderConsiderationFactory.BuildNTIUnderConsiderationModel();

			mockNtiUnderConsiderationModelService.Setup(n => n.GetNtiUnderConsideration(It.IsAny<long>()))
				.ReturnsAsync(ntiunderConsiderationModel);

			var pageModel = SetupIndexPageModel(mockNtiUnderConsiderationModelService, mockNtiReasonsCachedService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiUnderConsiderationId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.NTIUnderConsiderationModel);


			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-UC::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}


		[Test]
		public async Task WhenOnGetAsync_MissingNTIUnderConsideration_ThrowsException_ReturnPage()
		{
			// arrange
			Mock<INtiModelService> ntiunderConsiderationModel = new Mock<INtiModelService>();
			var mockNtiReasonsCachedService = new Mock<INtiReasonsCachedService>();
			Mock<ILogger<IndexPageModel>> mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(ntiunderConsiderationModel, mockNtiReasonsCachedService, mockLogger);

			var routeData = pageModel.RouteData.Values;

			routeData.Add("urn", 1);
			routeData.Add("ntiUnderConsiderationId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}


		private static IndexPageModel SetupIndexPageModel(
			Mock<INtiModelService> mockNtiModelService,
			Mock<INtiReasonsCachedService> reasonsCachedService,
			Mock<ILogger<IndexPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockNtiModelService.Object, reasonsCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}