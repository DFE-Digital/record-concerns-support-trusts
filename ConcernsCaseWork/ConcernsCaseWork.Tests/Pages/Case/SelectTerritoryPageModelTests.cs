using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case;

[Parallelizable(ParallelScope.All)]
public class SelectTerritoryPageModelTests
{
	[Test]
	public async Task WhenOnGetAsync_ReturnsPage()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockTrustModelService = new Mock<ITrustModelService>();
		
		var expectedCreateCaseModel = CaseFactory.BuildCreateCaseModel();
		var expectedTrustDetailsModel = TrustFactory.BuildTrustDetailsModel();
		expectedCreateCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();

		var username = "testing";
		var userState = new UserState(username)
		{
			TrustUkPrn = "trust-ukprn", 
			CreateCaseModel = expectedCreateCaseModel
		};

		mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
			.ReturnsAsync(expectedTrustDetailsModel);
		mockUserStateCachedService.Setup(c => c.GetData(username))
			.ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(h => h.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(username);
		
		var sut = SetupTerritoryModel(
			mockTrustModelService.Object, mockUserStateCachedService.Object,  
			mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
		
		// act
		await sut.OnGetAsync();

		// assert
		var createCaseModel = sut.CreateCaseModel;
		var trustDetailsModel = sut.TrustDetailsModel;
		var createRecordsModel = sut.CreateRecordsModel;
		
		Assert.Multiple(() =>
		{
			Assert.That(createCaseModel, Is.Not.Null);
			Assert.That(createCaseModel.TrustUkPrn, Is.EqualTo(expectedCreateCaseModel.TrustUkPrn));
			Assert.That(trustDetailsModel, Is.Not.Null);
			Assert.That(trustDetailsModel.TrustNameCounty, Is.Not.Null);
			Assert.That(trustDetailsModel.DisplayAddress, Is.Not.Null);
			Assert.That(createRecordsModel, Is.Not.Null);
			Assert.That(createRecordsModel, Has.Count.EqualTo(expectedCreateCaseModel.CreateRecordsModel.Count));
			Assert.That(sut.TempData["Error.Message"], Is.Null);
		});
		
		mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
		mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		mockLogger.VerifyLogInformationWasCalled("Territory");
	}

	[Test]
	public async Task WhenOnGetAsync_Missing_TrustUkprn_Returns_PageWithError()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockTrustModelService = new Mock<ITrustModelService>();

		var username = "testing";
		var userState = new UserState(username)
		{
			TrustUkPrn = null
		};

		mockUserStateCachedService.Setup(c => c.GetData(username)).ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(h => h.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(username);
			
		var sut = SetupTerritoryModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
			
		// act
		await sut.OnGetAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(sut.CreateCaseModel, Is.Null);
			Assert.That(sut.TrustDetailsModel, Is.Null);
			Assert.That(sut.CreateRecordsModel, Is.Null);
			Assert.That(sut.TempData["Error.Message"], Is.Not.Null);
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		});
			
		mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
		mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);

		mockLogger.VerifyLogInformationWasCalled("Territory");
		mockLogger.VerifyLogErrorWasCalled("Cache TrustUkprn is null");
	}

	[Test]
	public async Task WhenOnGetAsync_AndUserStateIsNull_ReturnsPageWithError()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockTrustModelService = new Mock<ITrustModelService>();
		
		var username = "testing";

		mockClaimsPrincipalHelper.Setup(h => h.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(username);
		
		var sut = SetupTerritoryModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
		
		// act
		await sut.OnGetAsync();
		
		// assert
		Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		
		mockLogger.VerifyLogInformationWasCalled("Territory");
		mockLogger.VerifyLogErrorWasCalled("Could not retrieve cached new case data for user");
	}
	
	[Test]
	public async Task WhenOnGetCancel_UserStateNotFound_ClearsUserStateAndRedirectToDetailsPage()
	{
		// arrange
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var username = "testing";
		
		mockClaimsPrincipalHelper.Setup(h => h.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(username);
		
		var sut = SetupTerritoryModel(Mock.Of<ITrustModelService>(), mockUserStateCachedService.Object, Mock.Of<ILogger<SelectTerritoryPageModel>>(), mockClaimsPrincipalHelper.Object,true);

		// act
		var pageResponse = await sut.OnGetCancel();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
		var page = pageResponse as RedirectResult;
		
		Assert.That(page, Is.Not.Null);
		Assert.That(page.Url, Is.EqualTo("/"));
		
		mockUserStateCachedService
			.Verify(cs => cs.StoreData(username, It.Is<UserState>(s => s.CreateCaseModel.TrustUkPrn == null)), 
				Times.Once);
	}

	[Test]
	public async Task WhenOnGetCancel_UserStateFound_ClearsUserStateAndRedirectToDetailsPage()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();

		var username = "testing";
		var userState = new UserState(username) { TrustUkPrn = null };

		mockUserStateCachedService.Setup(c => c.GetData(username)).ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(h => h.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(username);
		
		var sut = SetupTerritoryModel(
			Mock.Of<ITrustModelService>(), 
			mockUserStateCachedService.Object, 
			Mock.Of<ILogger<SelectTerritoryPageModel>>(), 
			mockClaimsPrincipalHelper.Object,true);
		
		// act
		var pageResponse = await sut.OnGetCancel();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
		var page = pageResponse as RedirectResult;
		
		Assert.That(page, Is.Not.Null);
		Assert.That(page.Url, Is.EqualTo("/"));
		
		mockUserStateCachedService
			.Verify(cs => cs.StoreData(username, It.Is<UserState>(s => s.CreateCaseModel.TrustUkPrn == null)), 
			Times.Once);
	}

	[Test]
	public async Task WhenOnPost_RedirectToDetailsPage()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockTrustModelService = new Mock<ITrustModelService>();
		
		var expected = CaseFactory.BuildCreateCaseModel();
		var username = "testuser";
		var userState = new UserState(username) { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

		mockUserStateCachedService.Setup(c => c.GetData(username)).ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns(username);
		
		var sut = SetupTerritoryModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);

		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
		var page = pageResponse as RedirectToPageResult;
		
		Assert.That(page, Is.Not.Null);
		Assert.That(page.PageName, Is.EqualTo("details"));
	}
	
	[Test]
	public async Task WhenOnPost_ModelStateValidationFailed_ReturnsPageWithError()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockTrustModelService = new Mock<ITrustModelService>();

		var expectedTrustByUkprn = TrustFactory.BuildTrustDetailsModel();
		var expected = CaseFactory.BuildCreateCaseModel();

		var username = "some.test.user";
		var userState = new UserState(username) { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

		mockUserStateCachedService.Setup(c => c.GetData(username)).ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns(username);
		mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedTrustByUkprn);
		
		var sut = SetupTerritoryModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
		var keyName = "testkey";
		var errorMsg = "some model validation error";
		sut.ModelState.AddModelError(keyName, errorMsg);
		
		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			Assert.That(sut.ModelState.IsValid, Is.False);
			Assert.That(sut.ModelState.Keys.Count(), Is.EqualTo(1));
			Assert.That(sut.ModelState.First().Key, Is.EqualTo(keyName));
			Assert.That(sut.ModelState.First().Value?.Errors.Single().ErrorMessage, Is.EqualTo(errorMsg));
		});
	}

	[Test]
	public async Task WhenOnPost_CouldNotGetUserState_ReturnsPageWithError()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();

		var username = "some.test.user";

		mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns(username);
		
		var sut = SetupTerritoryModel(Mock.Of<ITrustModelService>(), mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
		
		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<PageResult>());
		var page = pageResponse as PageResult;
		
		Assert.That(page, Is.Not.Null);
		
		mockLogger.VerifyLogErrorWasCalled("Could not retrieve cached new case data for user");
	}
	
	[Test]
	public async Task WhenOnPost_ModelStateIsInvalidAndCouldNotGetTrustUkPrn_ReturnsPageWithError()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();

		var username = "some.test.user";
		var userState = new UserState(username) { TrustUkPrn = null };

		mockUserStateCachedService.Setup(c => c.GetData(username)).ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns(username);

		mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns(username);
		
		var sut = SetupTerritoryModel(Mock.Of<ITrustModelService>(), mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
		var keyName = "testkey";
		var errorMsg = "some model validation error";
		sut.ModelState.AddModelError(keyName, errorMsg);
		
		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<PageResult>());
		var page = pageResponse as PageResult;
		
		Assert.That(page, Is.Not.Null);
		
		mockLogger.VerifyLogErrorWasCalled("Cache TrustUkprn is null");
	}
	
	private static SelectTerritoryPageModel SetupTerritoryModel(
		ITrustModelService mockTrustModelService,
		IUserStateCachedService mockUserStateCachedService, 
		ILogger<SelectTerritoryPageModel> mockLogger,
		IClaimsPrincipalHelper mockClaimsPrincipalHelper,
		bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
		
		return new SelectTerritoryPageModel(mockTrustModelService, mockUserStateCachedService, mockLogger, mockClaimsPrincipalHelper)
		{
			PageContext = pageContext,
			TempData = tempData,
			Url = new UrlHelper(actionContext),
			MetadataProvider = pageContext.ViewData.ModelMetadata
		};
	}
}