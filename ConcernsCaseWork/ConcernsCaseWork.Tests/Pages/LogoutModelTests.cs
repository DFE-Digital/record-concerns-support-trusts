using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class LogoutModelTests
	{
		[Test]
		public async Task WhenRequestOnGetAsyncIsAuthenticated_ReturnLogoutPage()
		{
			// arrange
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			var pageModel = new LogoutPageModel
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext)
			};
			
			// act
			var result = await pageModel.OnGetAsync();

			// assert
			Assert.IsInstanceOf(typeof(PageResult), result);
			Assert.That(pageModel.TempData.First().Key, Is.EqualTo("Message.UserName"));
			Assert.That(pageModel.TempData.First().Value, Is.EqualTo(PageContextFactory.ClaimName));
		}
	}
}