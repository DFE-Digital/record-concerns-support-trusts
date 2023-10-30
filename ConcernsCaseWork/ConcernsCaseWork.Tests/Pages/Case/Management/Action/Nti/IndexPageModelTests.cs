using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Services.Nti;
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
			var mockNtiConditionsService = new Mock<INtiConditionsCachedService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiViewModelAsync(1, It.IsAny<long>())).ReturnsAsync(ntiModel);

			var pageModel = SetupIndexPageModel(mockNtiModelService, mockNtiConditionsService, mockLogger);

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
			Mock<INtiConditionsCachedService> mockConditionsCachedService = null,
			Mock<ILogger<IndexPageModel>> mockLogger = null,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(mockModelService.Object, mockConditionsCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}