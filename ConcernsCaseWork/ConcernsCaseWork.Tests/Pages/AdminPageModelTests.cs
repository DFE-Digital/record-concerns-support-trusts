using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class AdminPageModelTests
	{
		[Test]
		public void WhenOnGet_ReturnsPage()
		{
			// arrange
			var pageModel = SetupAdminPageModel();
			
			// act
			pageModel.OnGet();

			// assert
			
		}

		private static AdminPageModel SetupAdminPageModel(bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new AdminPageModel()
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}