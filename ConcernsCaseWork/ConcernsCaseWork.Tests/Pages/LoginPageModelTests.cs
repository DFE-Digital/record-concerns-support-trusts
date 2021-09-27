using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class LoginPageModelTests
	{
		[Test]
		public void WhenRequestOnGetIsAuthenticated_ReturnHomePage()
		{
			// arrange
			var pageModel = SetupLoginModel(true);
			
			// act
			var result = pageModel.OnGet(string.Empty);

			// assert
			Assert.IsInstanceOf(typeof(RedirectResult), result);
			Assert.That(pageModel.Credentials.ReturnUrl, Is.EqualTo("/home"));
		}
		
		[Test]
		public void WhenRequestOnGetIsNotAuthenticated_ReturnLoginPage()
		{
			// arrange
			var pageModel = SetupLoginModel();
			
			// act
			var returnUrl = string.Empty;
			var result = pageModel.OnGet(returnUrl);

			// assert
			Assert.IsInstanceOf(typeof(PageResult), result);
			Assert.That(pageModel.Credentials.ReturnUrl, Is.EqualTo(returnUrl));
		}

		[Test]
		public async Task WhenRequestOnPostAsyncHasIncorrectCredentials_ReturnLoginPage()
		{
			// arrange
			var pageModel = SetupLoginModel();
			
			// act
			var returnUrl = string.Empty;
			var result = await pageModel.OnPostAsync(returnUrl);

			// assert
			Assert.IsInstanceOf(typeof(PageResult), result);
			Assert.That(pageModel.Credentials.ReturnUrl, Is.EqualTo(returnUrl));
			Assert.That(pageModel.TempData.First().Key, Is.EqualTo("Error.Message"));
			Assert.That(pageModel.TempData.First().Value, Is.EqualTo("Incorrect username and password"));
		}
		
		[TestCase("", "/home")]
		[TestCase("/home", "/home")]
		[TestCase("/admin", "/admin")]
		public async Task WhenRequestOnPostAsyncHasCorrectCredentials_ReturnHomePage(string actualReturnUrl, string expectedReturnUrl)
		{
			// arrange
			var pageModel = SetupLoginModel(true);
			pageModel.Credentials.UserName = "username";
			pageModel.Credentials.Password = "password";
			pageModel.Credentials.ReturnUrl = expectedReturnUrl;
			
			// act
			var result = await pageModel.OnPostAsync(actualReturnUrl);

			// assert
			Assert.IsInstanceOf(typeof(RedirectResult), result);
			var redirectResult = result as RedirectResult;
			
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo(expectedReturnUrl));
			Assert.That(pageModel.Credentials.ReturnUrl, Is.EqualTo(actualReturnUrl));
		}
		
		private static LoginPageModel SetupLoginModel(bool isAuthenticated = false)
		{
			var mockLogger = new Mock<ILogger<LoginPageModel>>();
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			var initialData = new Dictionary<string, string> { { "app:username", "username" }, { "app:password", "password" } };
			return new LoginPageModel(new ConfigurationBuilder().ConfigurationInMemoryBuilder(initialData).Build(), mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext)
			};
		}
	}
}