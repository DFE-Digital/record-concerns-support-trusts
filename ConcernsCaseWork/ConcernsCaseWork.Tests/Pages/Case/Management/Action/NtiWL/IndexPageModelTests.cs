using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiWL
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var ntiWarningLetterModel = NTIWarningLetterFactory.BuildNTIWarningLetterModel();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();	

			mockNtiWarningLetterModelService.Setup(n => n.GetNtiWarningLetterViewModel(1, It.IsAny<long>())).ReturnsAsync(ntiWarningLetterModel);

			var pageModel = SetupIndexPageModel(mockModelService: mockNtiWarningLetterModelService, mockLogger: mockLogger);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseId = 1;
			pageModel.NtiWarningLetterId = 1;

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.NtiWarningLetterModel);
		}

		private static IndexPageModel SetupIndexPageModel(
			Mock<INtiWarningLetterModelService> mockModelService = null,
			Mock<INtiWarningLetterReasonsCachedService> mockReasonsCachedService = null,
			Mock<INtiWarningLetterConditionsCachedService> mockConditionsCachedService = null,
			Mock<ILogger<IndexPageModel>> mockLogger = null,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			mockModelService ??= new Mock<INtiWarningLetterModelService>();
			mockReasonsCachedService ??= new Mock<INtiWarningLetterReasonsCachedService>();
			mockConditionsCachedService ??= new Mock<INtiWarningLetterConditionsCachedService>();
			mockLogger ??= new Mock<ILogger<IndexPageModel>>();

			return new IndexPageModel(mockReasonsCachedService.Object, mockModelService.Object, mockConditionsCachedService.Object,
				mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}