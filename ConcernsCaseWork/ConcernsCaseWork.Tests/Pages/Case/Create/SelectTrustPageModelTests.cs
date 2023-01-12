using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.CreateCase;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Create
{
	[Parallelizable(ParallelScope.All)]
	public class SelectTrustPageModelTests
	{
		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectTrustPageModel(
					Mock.Of<ITrustModelService>(),
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectTrustPageModel>>(),
					null));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}

		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectTrustPageModel(
					Mock.Of<ITrustModelService>(),
					Mock.Of<IUserStateCachedService>(),
					null,
					Mock.Of<IClaimsPrincipalHelper>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'logger')"));
		}

		[Test]
		public void Constructor_WithNullUserStateCachedService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectTrustPageModel(
					Mock.Of<ITrustModelService>(),
					null,
					Mock.Of<ILogger<SelectTrustPageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'cachedUserService')"));
		}

		[Test]
		public void Constructor_WithNullTrustModelService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectTrustPageModel(
					null,
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectTrustPageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'trustModelService')"));
		}

		[Test]
		[TestCase("")]
		[TestCase("a-b")]
		[TestCase("ab")]
		public async Task When_OnPostSelectedTrust_With_TrustUkPrn_Invalid_Throws_Exception(string searchString)
		{
			var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			sut.FindTrustModel.SelectedTrustUkprn = searchString;

			var result = await sut.OnPostSelectedTrust();

			Assert.That(result, Is.TypeOf<ObjectResult>());
			var objResult = (ObjectResult)result;
			Assert.That(objResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
		}

		[Test]
		public async Task When_OnPostSelectedTrust_With_ModelState_Invalid_Returns_Error()
		{
			var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			sut.ModelState.AddModelError("Error1", "error_message_1");

			// act
			sut.FindTrustModel.SelectedTrustUkprn = "abc";
			var result = await sut.OnPostSelectedTrust();

			IEnumerable<string> errorMsgs = (IEnumerable<string>) sut.TempData["Message"];
			Assert.That(errorMsgs.Count(), Is.EqualTo(1));
			Assert.That(errorMsgs.First(), Is.EqualTo("error_message_1"));
			Assert.That(result, Is.TypeOf<PageResult>());
		}

		[Test]
		[TestCase("abc")]
		[TestCase("1234")]
		public async Task When_OnPost_WithValidSearchQuery_ReturnsSearchResults(string searchString)
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var searchResults = TrustFactory.BuildListTrustSummaryModel();

			mockTrustService
				.Setup(t => t.GetTrustsBySearchCriteria(It.Is<TrustSearch>(s => s.Ukprn == searchString && s.GroupName == searchString && s.CompaniesHouseNumber == searchString)))
				.ReturnsAsync(searchResults);

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			var userName = sut.User.Identity.Name;
			var userState = new UserState(userName);
			mockUserService.Setup(x => x.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockUserService.Setup(x => x.StoreData(userName, userState)).Returns(Task.CompletedTask);


			// act
			sut.FindTrustModel.SelectedTrustUkprn = searchString;
			var result = await sut.OnPostSelectedTrust();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.TempData["Message"], Is.Null);
				Assert.That(result, Is.TypeOf<RedirectToPageResult>());

			});

			mockUserService.Verify(x => x.StoreData(userName, userState), Times.Once);
			mockLogger.VerifyLogInformationWasCalled("OnPostSelectedTrust");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		//[Test]
		//[TestCase("abc")]
		//[TestCase("1234")]
		//public async Task WhenOnGetTrustsSearchResult_WithValidSearchQuery_ReturnsSearchResults(string searchString)
		//{
		//	// arrange
		//	var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
		//	var mockTrustService = new Mock<ITrustModelService>();
		//	var mockUserService = new Mock<IUserStateCachedService>();
		//	var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

		//	var searchResults = TrustFactory.BuildListTrustSummaryModel();

		//	mockTrustService
		//		.Setup(t => t.GetTrustsBySearchCriteria(It.Is<TrustSearch>(s => s.Ukprn == searchString && s.GroupName == searchString && s.CompaniesHouseNumber == searchString)))
		//		.ReturnsAsync(searchResults);

		//	var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

		//	// act
		//	sut.FindTrustModel.SelectedTrustUkprn = searchString;
		//	var result = await sut.OnPostSelectedTrust();

		//	// assert
		//	Assert.Multiple(() =>
		//	{
		//		Assert.That(sut.TrustAddress, Is.Null);
		//		Assert.That(sut.TempData["Error.Message"], Is.Null);

		//		Assert.That(result, Is.TypeOf<JsonResult>());

		//		var jsonResult = result as JsonResult;
		//		Assert.That(jsonResult?.Value, Is.Not.Null);
		//		Assert.That(jsonResult.Value, Is.TypeOf<List<TrustSearchModel>>());

		//		var searchModelResult = jsonResult.Value as List<TrustSearchModel>;
		//		Assert.That(searchModelResult, Is.Not.Null);
		//		Assert.That(searchModelResult, Has.Count.EqualTo(searchResults.Count));
		//	});

		//	mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
		//	mockLogger.VerifyLogErrorWasNotCalled();
		//	mockLogger.VerifyNoOtherCalls();
		//}

		//[Test]
		//[TestCase("ab")]
		//[TestCase("1")]
		//[TestCase("")]
		//[TestCase(null)]
		//public async Task WhenOnGetTrustsSearchResult_WithInvalidSearchQuery_DoesNotErrorAndReturnsEmptySearchResults(string searchString)
		//{
		//	// arrange
		//	var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
		//	var mockTrustService = new Mock<ITrustModelService>();
		//	var mockUserService = new Mock<IUserStateCachedService>();
		//	var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

		//	var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

		//	// act
		//	sut.FindTrustModel.SelectedTrustUkprn = searchString;
		//	var result = await sut.OnPostSelectedTrust();

		//	// assert
		//	Assert.Multiple(() =>
		//	{
		//		Assert.That(sut.TrustAddress, Is.Null);
		//		Assert.That(sut.TempData["Error.Message"], Is.Null);
		//		Assert.That(result, Is.TypeOf<JsonResult>());

		//		var jsonResult = result as JsonResult;
		//		Assert.That(jsonResult?.Value, Is.Not.Null);
		//		Assert.That(jsonResult.Value, Is.TypeOf<List<TrustSearchModel>>());

		//		var searchModelResult = jsonResult.Value as List<TrustSearchModel>;
		//		Assert.That(searchModelResult, Is.Not.Null);
		//		Assert.That(searchModelResult, Has.Count.EqualTo(0));
		//	});

		//	mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
		//	mockLogger.VerifyLogErrorWasNotCalled();
		//	mockLogger.VerifyNoOtherCalls();
		//}

		//[Test]
		//[TestCase("abc")]
		//[TestCase("123")]
		//public async Task WhenOnGetTrustsSearchResult_WithErrorThrownBySearchService_ReturnsErrorStatusCode(string searchString)
		//{
		//	// arrange
		//	var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
		//	var mockTrustService = new Mock<ITrustModelService>();
		//	var mockUserService = new Mock<IUserStateCachedService>();
		//	var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

		//	var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

		//	mockTrustService
		//		.Setup(t => t.GetTrustsBySearchCriteria(It.Is<TrustSearch>(s => s.Ukprn == searchString && s.GroupName == searchString && s.CompaniesHouseNumber == searchString)))
		//		.Throws(() => new Exception("some error message"));

		//	// act
		//	sut.FindTrustModel.SelectedTrustUkprn = searchString;
		//	var result = await sut.OnPostSelectedTrust();

		//	// assert
		//	Assert.Multiple(() =>
		//	{
		//		Assert.That(sut.TrustAddress, Is.Null);
		//		Assert.That(sut.TempData["Error.Message"], Is.Null);
		//		Assert.That(result, Is.TypeOf<ObjectResult>());

		//		Assert.That(result, Is.Not.Null);
		//		Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
		//	});

		//	mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
		//	mockLogger.VerifyLogErrorWasCalled("some error message");
		//	mockLogger.VerifyNoOtherCalls();
		//}

		//[Test]
		//[TestCase("abcdef")]
		//[TestCase("1234")]
		//public async Task WhenOnGetSelectedTrust_WithValidTrustUkPrn_CachesTrustAndRedirectsToNextStep(string trustUkPrn)
		//{
		//	// arrange
		//	var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
		//	var mockTrustService = new Mock<ITrustModelService>();
		//	var mockUserService = new Mock<IUserStateCachedService>();
		//	var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

		//	var userName = "some name of a user";

		//	mockUserService
		//		.Setup(s => s.GetData(userName))
		//		.ReturnsAsync(new UserState(userName){TrustUkPrn = trustUkPrn});

		//	mockUserService
		//		.Setup(s => s.StoreData(It.IsAny<string>(), It.IsAny<UserState>()))
		//		.Verifiable();

		//	mockClaimsPrincipalHelper
		//		.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
		//		.Returns(userName);

		//	var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

		//	// act
		//	sut.FindTrustModel.SelectedTrustUkprn = trustUkPrn;
		//	var result = await sut.OnPostSelectedTrust();

		//	// assert
		//	Assert.Multiple(() =>
		//	{
		//		Assert.That(sut.TrustAddress, Is.Null);
		//		Assert.That(sut.TempData["Error.Message"], Is.Null);

		//		Assert.That(result, Is.TypeOf<JsonResult>());

		//		var jsonResult = result as JsonResult;
		//		Assert.That(jsonResult?.Value, Is.Not.Null);
		//		Assert.That(jsonResult.Value?.ToString(), Is.EqualTo("{ redirectUrl = /case/create/type }"));

		//	});

		//	mockUserService.Verify(s => s.StoreData(userName, It.Is<UserState>(us => us.TrustUkPrn == trustUkPrn)));
		//	mockLogger.VerifyLogInformationWasCalled("OnGetSelectedTrust");
		//	mockLogger.VerifyLogErrorWasNotCalled();
		//	mockLogger.VerifyNoOtherCalls();
		//}

		//[Test]
		//[TestCase(null)]
		//[TestCase("")]
		//[TestCase("12")]
		//[TestCase("1")]
		//[TestCase("contains-hyphen")]
		//public async Task WhenOnGetSelectedTrust_WithInvalidTrustUkPrn_Errors(string trustUkPrn)
		//{
		//	// arrange
		//	var mockLogger = new Mock<ILogger<SelectTrustPageModel>>();
		//	var mockTrustService = new Mock<ITrustModelService>();
		//	var mockUserService = new Mock<IUserStateCachedService>();
		//	var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

		//	var userName = "some name of a user";

		//	mockUserService
		//		.Setup(s => s.GetData(userName))
		//		.ReturnsAsync(new UserState(userName){TrustUkPrn = trustUkPrn});

		//	mockUserService
		//		.Setup(s => s.StoreData(It.IsAny<string>(), It.IsAny<UserState>()))
		//		.Verifiable();

		//	mockClaimsPrincipalHelper
		//		.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
		//		.Returns(userName);

		//	var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

		//	// act
		//	sut.FindTrustModel.SelectedTrustUkprn = trustUkPrn;
		//	var result = await sut.OnPostSelectedTrust();

		//	// assert
		//	Assert.Multiple(() =>
		//	{
		//		Assert.That(sut.TrustAddress, Is.Null);
		//		Assert.That(sut.TempData["Error.Message"], Is.Null);
		//		Assert.That(result, Is.TypeOf<ObjectResult>());

		//		Assert.That(result, Is.Not.Null);
		//		Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
		//	});

		//	mockUserService.Verify(s => s.StoreData(userName, It.Is<UserState>(us => us.TrustUkPrn == trustUkPrn)),
		//		Times.Never);
		//	mockLogger.VerifyLogInformationWasCalled("OnGetSelectedTrust");
		//	mockLogger.VerifyLogErrorWasCalled($"Selected trust is incorrect - {trustUkPrn}");
		//	mockLogger.VerifyNoOtherCalls();
		//}

		private static SelectTrustPageModel SetupPageModel(
			IMock<ILogger<SelectTrustPageModel>> mockLogger,
			IMock<ITrustModelService> mockTrustService,
			IMock<IUserStateCachedService> mockUserService,
			Mock<IClaimsPrincipalHelper> mockClaimsHelper)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			mockClaimsHelper.Setup(x => x.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(pageContext.HttpContext.User.Identity.Name);

			return new SelectTrustPageModel(mockTrustService.Object, mockUserService.Object, mockLogger.Object, mockClaimsHelper.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}