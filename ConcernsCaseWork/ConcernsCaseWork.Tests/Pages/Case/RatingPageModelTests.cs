using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
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
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class RatingPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateService = new Mock<IUserStateCachedService>();

			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = createCaseModel };
			var expected = TrustFactory.BuildTrustDetailsModel();
			
			mockUserStateService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateService.Object,
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.TrustDetailsModel, Is.Not.Null);
			Assert.That(pageModel.CreateRecordsModel, Is.Not.Null);
			Assert.IsNull(pageModel.TempData["Error.Message"]);
			
			var trustDetailsPageModel = pageModel.TrustDetailsModel;
			var createRecordsPageModel = pageModel.CreateRecordsModel;
			
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsPageModel);
			Assert.IsAssignableFrom<List<CreateRecordModel>>(createRecordsPageModel);
			
			mockUserStateService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_TrustUkprnIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateService = new Mock<IUserStateCachedService>();

			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();
			var userState = new UserState("testing") { CreateCaseModel = createCaseModel };
			var expected = TrustFactory.BuildTrustDetailsModel();
			
			mockUserStateService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateService.Object, 
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.CreateRecordsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
			
			mockUserStateService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}		
		
		[Test]
		public async Task WhenOnGetAsync_UserStateIsNull_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync((UserState)null);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.IsNull(pageModel.TrustDetailsModel);
			Assert.IsNull(pageModel.CreateRecordsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
			
			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}		

		[Test]
		public async Task WhenOnPostAsync_RedirectToPage_Details()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 1;

			// act
			var pageResponse = await pageModel.OnPostAsync();
			var pageResponseInstance = pageResponse as RedirectToPageResult;
			
			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.PageName, Is.EqualTo("details"));

			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockUserStateCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostAsync_And_RatingId_Invalid_Raise_Exception()
		{
			// arrange
			var mockLogger = new Mock<ILogger<RatingPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			
			var pageModel = SetupRatingPageModel(mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);

			pageModel.RiskToTrust = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.RiskToTrust.SelectedId = 101;

			// act
			_ = await pageModel.OnPostAsync();
			
			// assert
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
		}		
		
		private static RatingPageModel SetupRatingPageModel(
			ITrustModelService mockTrustModelService, 
			IUserStateCachedService mockUserStateCachedService,
			ILogger<RatingPageModel> mockLogger, bool isAuthenticated = false)
		{
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns("Tester");

			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new RatingPageModel(mockTrustModelService, mockUserStateCachedService,
				mockLogger, mockClaimsPrincipalHelper.Object,MockTelemetry.CreateMockTelemetryClient())
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}