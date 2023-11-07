using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Redis.Teams;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Redis.Types;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class ClearDataPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsHomePage_ClearCache()
		{
			// arrange
			const string ExpectedUserIdentity = "Tester";
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockTeamsCachedService = new Mock<ITeamsCachedService>();
			var mockLogger = new Mock<ILogger<ClearDataPageModel>>();
			
			var pageModel = SetupClearDataModel(
				mockTypeCachedService.Object,
				mockUserStateCachedService.Object,
				mockTrustCachedService.Object,
				mockTeamsCachedService.Object,
				mockLogger.Object,
				true);

			// act
			var response = await pageModel.OnGetAsync();
			
			Assert.IsInstanceOf(typeof(RedirectToPageResult), response);
			var redirectResult = response as RedirectToPageResult;
			
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.EqualTo("home"));
			
			mockTypeCachedService.Verify(c => c.ClearData(), Times.Once);
			mockUserStateCachedService.Verify(c => c.ClearData(It.IsAny<string>()), Times.Once);
			mockTrustCachedService.Verify(c => c.ClearData(), Times.Once);
			mockTeamsCachedService.Verify(c => c.ClearData(ExpectedUserIdentity), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsHomePage_NotAuthenticated()
		{
			// arrange
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustCachedService = new Mock<ITrustCachedService>();
			var mockTeamsCachedService = new Mock<ITeamsCachedService>();

			var mockLogger = new Mock<ILogger<ClearDataPageModel>>();
			
			var pageModel = SetupClearDataModel(
				mockTypeCachedService.Object,
                mockUserStateCachedService.Object,
				mockTrustCachedService.Object,
				mockTeamsCachedService.Object,
				mockLogger.Object);

			// act
			var response = await pageModel.OnGetAsync();
			
			Assert.IsInstanceOf(typeof(RedirectToPageResult), response);
			var redirectResult = response as RedirectToPageResult;
			
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.Not.Null);
			Assert.That(redirectResult.PageName, Is.EqualTo("home"));
			
			mockTypeCachedService.Verify(c => c.ClearData(), Times.Never);
			mockUserStateCachedService.Verify(c => c.ClearData(It.IsAny<string>()), Times.Never);
			mockTrustCachedService.Verify(c => c.ClearData(), Times.Never);
			mockTeamsCachedService.Verify(c => c.ClearData(It.IsAny<string>()), Times.Never);
		}
		
		private static ClearDataPageModel SetupClearDataModel(
			ITypeCachedService mockTypeCachedService, 
			IUserStateCachedService mockUserStateCachedService,
			ITrustCachedService mockTrustCachedService,
			ITeamsCachedService mockTeamsCachedService,
			ILogger<ClearDataPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClearDataPageModel(
				mockUserStateCachedService,
				mockTypeCachedService,
				mockTrustCachedService,
				mockTeamsCachedService,
				mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}