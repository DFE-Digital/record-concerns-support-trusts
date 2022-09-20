using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Nti;
using Service.Redis.NtiWarningLetter;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockNtiConditionsService = new Mock<INtiConditionsCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockNtiConditionsService, mockLogger);

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
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockNtiConditionsService = new Mock<INtiConditionsCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>())).ReturnsAsync(ntiModel);

			var pageModel = SetupIndexPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockNtiConditionsService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.NtiModel);


			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingNTIModel_ThrowsException_ReturnPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockNtiConditionsService = new Mock<INtiConditionsCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockNtiConditionsService, mockLogger);

			var routeData = pageModel.RouteData.Values;

			routeData.Add("urn", 1);
			routeData.Add("ntiId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		private static IndexPageModel SetupIndexPageModel(
			Mock<INtiModelService> mockModelService,
			Mock<INtiReasonsCachedService> mockReasonsCachedService,
			Mock<INtiStatusesCachedService> mockStatusesCachedService,
			Mock<INtiConditionsCachedService> mockConditionsCachedService = null,
			Mock<ILogger<IndexPageModel>> mockLogger = null,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockModelService.Object, mockReasonsCachedService.Object, mockStatusesCachedService.Object, mockConditionsCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}