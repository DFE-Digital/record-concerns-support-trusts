using ConcernsCaseWork.Pages.Admin;
using ConcernsCaseWork.Security;
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
	public class AdminPageModelTests
	{
		[Test]
		public async Task WhenOnGet_ReturnsPage()
		{
			// arrange
			var mockRbacManager = new Mock<IRbacManager>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			
			var pageModel = SetupAdminPageModel(mockRbacManager.Object, mockLogger.Object);
			
			// act
			await pageModel.OnGetAsync();

			// assert
			
		}

		private static IndexPageModel SetupAdminPageModel(IRbacManager rbacManager, ILogger<IndexPageModel> logger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new IndexPageModel(rbacManager, logger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}