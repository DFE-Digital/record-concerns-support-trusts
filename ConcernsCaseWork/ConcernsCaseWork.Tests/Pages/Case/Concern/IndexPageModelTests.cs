using AutoFixture;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Concern;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using ConcernsCaseWork.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Concern
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockCachedService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var expectedAddress = TrustFactory.BuildTrustAddressModel();
			var expectedRatingsModel = RatingFactory.BuildListRatingModel();

			mockCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(new UserState("testing") { TrustUkPrn = "trust-ukprn" });
			mockTrustModelService.Setup(s => s.GetTrustAddressByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedAddress);
			mockRatingModelService.Setup(r => r.GetRatingsModel()).ReturnsAsync(expectedRatingsModel);

			var pageModel = SetupIndexPageModel(mockTrustModelService.Object, mockCachedService.Object,
				mockRatingModelService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object, true);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.ConcernType, Is.Not.Null);
			Assert.That(pageModel.TrustAddress, Is.Not.Null);
			Assert.That(pageModel.CreateRecordsModel, Is.Not.Null);
			Assert.That(pageModel.ConcernRiskRating, Is.Not.Null);
			Assert.That(pageModel.MeansOfReferral, Is.Not.Null);
		}

		[Test]
		public async Task WhenOnGetAsync_MissingCacheUserState_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockCachedService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var expected = TrustFactory.BuildTrustDetailsModel();

			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);

			var pageModel = SetupIndexPageModel(mockTrustModelService.Object, mockCachedService.Object,
				mockRatingModelService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object, true);

			// act
			await pageModel.OnGetAsync();

			// Verify ILogger
			mockLogger.VerifyLogInformationWasCalled("OnGetAsync");
			mockLogger.VerifyLogErrorWasCalled("Cache CaseStateData is null");
			mockLogger.VerifyNoOtherCalls();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenOnGetAsync_TrustUkprnIsNullOrEmpty_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockCachedService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var createCaseModel = CaseFactory.BuildCreateCaseModel();
			createCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();

			var userState = new UserState("testing") { CreateCaseModel = createCaseModel };

			mockCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupIndexPageModel(mockTrustModelService.Object, mockCachedService.Object,
				mockRatingModelService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object, true);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.IsNull(pageModel.TrustAddress);
			Assert.IsNull(pageModel.CreateRecordsModel);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);

			// Verify ILogger
			mockLogger.VerifyLogInformationWasCalled("OnGetAsync");
			mockLogger.VerifyLogErrorWasCalled();
			mockLogger.VerifyNoOtherCalls();

			mockCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(s => s.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostAsync_UserStateIsNull_ThrowsException()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockCachedService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			mockCachedService.Setup(c => c.GetData(It.IsAny<string>()))
				.ReturnsAsync((UserState)null);
			mockRatingModelService.Setup(m => m.GetRatingModelById(1)).ReturnsAsync(() => new RatingModel() { Name = "Test" });

			var pageModel = SetupIndexPageModel(mockTrustModelService.Object, mockCachedService.Object,
				mockRatingModelService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object, true);

			pageModel.ConcernType = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.ConcernType.SelectedId = (int)ConcernType.ForceMajeure;
			pageModel.ConcernType.SelectedSubId = null;

			pageModel.ConcernRiskRating = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.ConcernRiskRating.SelectedId = 1;

			pageModel.MeansOfReferral = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.MeansOfReferral.SelectedId = 1;

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			mockCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostAsync_ReturnConcernPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCachedService = new Mock<IUserStateCachedService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockRatingModelService.Setup(m => m.GetRatingModelById(1)).ReturnsAsync(() => new RatingModel() { Name = "Test" });

			var pageModel = SetupIndexPageModel(mockTrustModelService.Object, mockCachedService.Object,
				mockRatingModelService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object, true);

			pageModel.ConcernType = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.ConcernType.SelectedId = (int)ConcernType.ForceMajeure;
			pageModel.ConcernType.SelectedSubId = null;

			pageModel.ConcernRiskRating = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.ConcernRiskRating.SelectedId = 1;

			pageModel.MeansOfReferral = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.MeansOfReferral.SelectedId = 1;

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
			var page = pageResponse as RedirectToPageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.PageName, Is.EqualTo("/case/rating"));

			mockCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetCancelCreateCase_EmptiesCreateRecords_ReturnsHome()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockCachedService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);

			var pageModel = SetupIndexPageModel(mockTrustModelService.Object, mockCachedService.Object,
				mockRatingModelService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object, true);

			// act
			var pageResponse = await pageModel.OnGetCancel();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo("/"));

			mockCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockCachedService.Verify(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGetCancel_UserStateIsNull_Return_Page()
		{
			// arrange
			var mockLogger = new Mock<ILogger<IndexPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockCachedService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			mockCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync((UserState)null);
			mockCachedService.Setup(c => c.StoreData(It.IsAny<string>(), It.IsAny<UserState>()));

			var pageModel = SetupIndexPageModel(mockTrustModelService.Object, mockCachedService.Object,
				mockRatingModelService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object, true);

			// act
			var pageResponse = await pageModel.OnGetCancel();
			var pageResponseInstance = pageResponse as PageResult;

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			Assert.IsNotNull(pageResponseInstance);
			Assert.IsNotNull(pageModel.TempData["Error.Message"]);
		}

		private static IndexPageModel SetupIndexPageModel(
			ITrustModelService mockTrustModelService,
			IUserStateCachedService cachedService,
			IRatingModelService mockRatingModelService,
			ILogger<IndexPageModel> mockLogger,
			IClaimsPrincipalHelper claimsPrincipalHelper,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			Mock<ICaseModelService> mockCaseModelService = new Mock<ICaseModelService>();

			return new IndexPageModel(mockTrustModelService, cachedService,
				mockRatingModelService, claimsPrincipalHelper,
				mockLogger,
				MockTelemetry.CreateMockTelemetryClient(),
				mockCaseModelService.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}