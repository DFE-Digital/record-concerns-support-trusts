using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.All)]
	public class ClosurePageModelTests
	{
		private static readonly Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			var expectedCaseModel = CaseFactory.BuildCaseModel();
			var expectedTrustDetailsModel = TrustFactory.BuildTrustDetailsModel();

			DateTime closedAt = DateTime.Now;

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<long>()))
				.ReturnsAsync(expectedCaseModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(expectedTrustDetailsModel);

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object, 
				mockTrustModelService.Object,
				mockLogger.Object, 
				mockUserStateCachedService.Object, 
				true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();
			var trustDetailsModel = pageModel.TrustDetailsModel;

			// assert
			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			Assert.That(pageModel.RationaleForClosure, Is.Not.Null);
		}

		[Test]
		public async Task WhenOnPostCloseCase_RedirectToHomePage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();

			mockCaseModelService.Setup(c => c.PatchClosure(It.IsAny<PatchCaseModel>()));

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync((UserState)new UserState("testing"));
			mockUserStateCachedService.Setup(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()));

			var pageModel = SetupClosurePageModel(
				mockCaseModelService.Object,
				mockTrustModelService.Object,
				mockLogger.Object,
				mockUserStateCachedService.Object,
				true);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.RationaleForClosure = _fixture.Create<TextAreaUiComponent>();

			// act
			var actionResult = await pageModel.OnPostCloseCase();
			var redirectResult = actionResult as RedirectResult;

			// assert
			Assert.That(actionResult, Is.AssignableFrom<RedirectResult>());
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("/"));
		}

		private static ClosurePageModel SetupClosurePageModel(
			ICaseModelService mockCaseModelService, 
			ITrustModelService mockTrustModelService,
			ILogger<ClosurePageModel> mockLogger, 
			IUserStateCachedService mockUserStateCachedService, 
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			var telemetry = MockTelemetry.CreateMockTelemetryClient();
			return new ClosurePageModel(
				mockCaseModelService,
				mockTrustModelService,
				mockLogger,
				mockUserStateCachedService,
				telemetry
				)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata

			};
		}
	}
}