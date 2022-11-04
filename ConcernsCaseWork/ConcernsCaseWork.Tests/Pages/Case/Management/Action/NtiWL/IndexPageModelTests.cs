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
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var pageModel = SetupIndexPageModel();

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var ntiWarningLetterModel = NTIWarningLetterFactory.BuildNTIWarningLetterModel();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();	

			mockNtiWarningLetterModelService.Setup(n => n.GetNtiWarningLetterId(It.IsAny<long>())).ReturnsAsync(ntiWarningLetterModel);

			var pageModel = SetupIndexPageModel(mockModelService: mockNtiWarningLetterModelService, mockLogger: mockLogger);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("ntiWarningLetterId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.NtiWarningLetterModel);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-Warning letter::IndexPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingNTIUnderConsideration_ThrowsException_ReturnPage()
		{
			// arrange
			var pageModel = SetupIndexPageModel();

			var routeData = pageModel.RouteData.Values;

			routeData.Add("urn", 1);
			routeData.Add("ntiUnderConsiderationId", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		private static IndexPageModel SetupIndexPageModel(
			Mock<INtiWarningLetterModelService> mockModelService = null,
			Mock<INtiWarningLetterStatusesCachedService> statusesCachedService = null,
			Mock<INtiWarningLetterReasonsCachedService> mockReasonsCachedService = null,
			Mock<INtiWarningLetterConditionsCachedService> mockConditionsCachedService = null,
			Mock<ILogger<IndexPageModel>> mockLogger = null,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			mockModelService ??= new Mock<INtiWarningLetterModelService>();
			statusesCachedService ??= new Mock<INtiWarningLetterStatusesCachedService>();
			mockReasonsCachedService ??= new Mock<INtiWarningLetterReasonsCachedService>();
			mockConditionsCachedService ??= new Mock<INtiWarningLetterConditionsCachedService>();
			mockLogger ??= new Mock<ILogger<IndexPageModel>>();

			return new IndexPageModel(statusesCachedService.Object, mockReasonsCachedService.Object, mockModelService.Object, mockConditionsCachedService.Object,
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