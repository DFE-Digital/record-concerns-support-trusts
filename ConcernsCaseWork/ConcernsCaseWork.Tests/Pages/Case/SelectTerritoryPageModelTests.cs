using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
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
	public async Task WhenOnGetAsync_Missing_TrustUkprn_Returns_Page()
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
	public async Task WhenOnGetAsync_AndUserStateIsNull_ReturnsErrorLoadingPage()
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
		mockLogger.VerifyLogErrorWasCalled("LoadPage::Exception - Could not retrieve cached new case data for user");
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
	public async Task WhenOnPost_MissingFormParameters_ReturnsPage()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockTrustModelService = new Mock<ITrustModelService>();

		var expectedTrustByUkprn = TrustFactory.BuildTrustDetailsModel();
		var expected = CaseFactory.BuildCreateCaseModel();
		var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

		mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns("Tester");
		mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedTrustByUkprn);
		
		var sut = SetupTerritoryModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
		
		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<PageResult>());
		var page = pageResponse as PageResult;
		
		Assert.That(page, Is.Not.Null);
		
		mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
		mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
	}
	
	[Test]
	public async Task WhenOnPost_EmptyFormParameters_ReturnsPage()
	{
		// arrange
		var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
		var mockLogger = new Mock<ILogger<SelectTerritoryPageModel>>();
		var mockUserStateCachedService = new Mock<IUserStateCachedService>();
		var mockTrustModelService = new Mock<ITrustModelService>();

		var expectedTrustByUkprn = TrustFactory.BuildTrustDetailsModel();
		var expected = CaseFactory.BuildCreateCaseModel();
		var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

		mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
		mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns("Tester");
		mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedTrustByUkprn);
		
		var sut = SetupTerritoryModel(mockTrustModelService.Object, mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipalHelper.Object,true);
		
		sut.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
				{ "issue", new StringValues("") },
				{ "current-status", new StringValues("") },
				{ "next-steps", new StringValues("") },
				{ "case-aim", new StringValues("") },
				{ "de-escalation-point", new StringValues("") },
				{ "case-history", new StringValues("case-history") }
			});
		
		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<PageResult>());
		var page = pageResponse as PageResult;
		
		Assert.That(page, Is.Not.Null);
		
		mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
		mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
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