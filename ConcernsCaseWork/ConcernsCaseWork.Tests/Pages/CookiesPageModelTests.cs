using ConcernsCaseWork.Pages;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Routing;
using System.Web;
using System.Collections.Generic;
using ConcernsCaseWork.Constants;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;

namespace ConcernsCaseWork.Tests.Pages
{
	public class CookiesPageModelTests
	{
		[TestCase("true", true)]
		[TestCase("false", false)]
		[TestCase(null, false)]
		public void When_OnGet_ReadsExistingCookieValue(string cookieValue, bool expected)
		{
			var model = CreatePageModel();

			var cookieManager = new Mock<IRequestCookieCollection>();
			cookieManager.Setup(m => m[It.IsAny<string>()]).Returns(cookieValue);

			model.Request.Cookies = cookieManager.Object;

			model.OnGet();

			model.HasConsented.Should().Be(expected);
		}

		[TestCase(true, "True")]
		[TestCase(false, "False")]
		public void When_OnPost_Then_CookieSet(bool hasConsented, string expected)
		{
			var model = CreatePageModel();

			model.HasConsented = hasConsented;

			var pageResult = model.OnPost();

			var cookies = model.Response.GetTypedHeaders().SetCookie;
			cookies.Should().HaveCount(1);

			var cookie = cookies.FirstOrDefault(c => c.Name == CookieConstants.CookieConsentName);

			cookie.Value.Value.Should().Be(expected);
			pageResult.Should().BeOfType(typeof(PageResult));
		}

		[TestCase("true", "true")]
		[TestCase("false", "false")]
		public void When_OnPost_BannerValue_Then_CookieSet(string hasConsented, string expected)
		{
			var model = CreatePageModel();

			model.Request.Headers["Referer"] = "/case";

			var cookieManager = new Mock<IRequestCookieCollection>();                  

			var pageResult = model.OnPost(hasConsented) as RedirectResult;

			var cookies = model.Response.GetTypedHeaders().SetCookie;
			cookies.Should().HaveCount(1);

			var cookie = cookies.FirstOrDefault(c => c.Name == CookieConstants.CookieConsentName);

			cookie.Value.Value.Should().Be(expected);

			pageResult.Url.Should().Be("/case");
		}

		private static CookiesPageModel CreatePageModel()
		{
			var mockLogger = new Mock<ILogger<CookiesPageModel>>();

			var httpContext = new DefaultHttpContext();
			var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), new ModelStateDictionary());
			var pageContext = new PageContext(actionContext);

			var result = new CookiesPageModel(mockLogger.Object)
			{
				PageContext = pageContext
			};

			return result;
		}
	}
}
