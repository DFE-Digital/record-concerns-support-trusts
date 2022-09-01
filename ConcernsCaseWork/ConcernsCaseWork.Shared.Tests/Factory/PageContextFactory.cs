using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class PageContextFactory
	{
		private const string ClaimNameIdentifier = "1";
		private const string AuthenticationType = "mock";
		public const string ClaimName = "Tester";

		public static (PageContext, TempDataDictionary, ActionContext) PageContextBuilder(bool isAuthenticated, string userName = ClaimName)
		{
			var authServiceMock = new Mock<IAuthenticationService>();
			authServiceMock
				.Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
				.Returns(Task.FromResult((object)null));

			var serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup(_ => _.GetService(typeof(IAuthenticationService)))
				.Returns(authServiceMock.Object);

			var httpContext = isAuthenticated ? new DefaultHttpContext
			{
				User = SetupClaimsPrincipal(userName),
				RequestServices = serviceProviderMock.Object
			} : new DefaultHttpContext();

			var modelState = new ModelStateDictionary();
			var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
			var modelMetadataProvider = new EmptyModelMetadataProvider();
			var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
			var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
			var pageContext = new PageContext(actionContext) { ViewData = viewData, HttpContext = httpContext };

			return (pageContext, tempData, actionContext);
		}

		private static ClaimsPrincipal SetupClaimsPrincipal(string claimName)
		{
			return new ClaimsPrincipal(new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, claimName),
					new Claim(ClaimTypes.NameIdentifier, ClaimNameIdentifier)
				},
				AuthenticationType
			));
		}
	}
}