using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiConditionsService = new Mock<INtiConditionsService>();
			var mockCasePermissionService = new Mock<ICasePermissionsService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiViewModelAsync(1, It.IsAny<long>())).ReturnsAsync(ntiModel);

			var pageModel = SetupIndexPageModel(mockNtiModelService, mockNtiConditionsService, mockCasePermissionService, mockLogger);

			var routeData = pageModel.RouteData.Values;

			pageModel.CaseId = 1;
			pageModel.NtiId = 1;

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.NtiModel);
		}

		private static IndexPageModel SetupIndexPageModel(
			Mock<INtiModelService> mockModelService,
			Mock<INtiConditionsService> mockConditionsService = null,
			Mock<ICasePermissionsService> mockCasePermissionService = null,
			Mock<ILogger<IndexPageModel>> mockLogger = null,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockModelService.Object, mockConditionsService.Object, mockCasePermissionService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}