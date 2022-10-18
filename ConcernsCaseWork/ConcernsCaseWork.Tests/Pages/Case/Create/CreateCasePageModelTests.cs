using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.CreateCase;
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
using Service.Redis.Models;
using Service.Redis.Users;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Create
{
	[Parallelizable(ParallelScope.All)]
	public class CreateCasePageModelTests
	{
		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new CreateCasePageModel(
					Mock.Of<ITrustModelService>(),
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<CreateCasePageModel>>(),
					null));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}
		
		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new CreateCasePageModel(
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
				_ = new CreateCasePageModel(
					Mock.Of<ITrustModelService>(),
					null,
					Mock.Of<ILogger<CreateCasePageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'cachedUserService')"));
		}

		[Test]
		public void Constructor_WithNullTrustModelService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new CreateCasePageModel(
					null,
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<CreateCasePageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'trustModelService')"));
		}
		
		[Test]
		public async Task WhenOnGet_CaseTypeIsSelectTrust_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var sut = SetupPageModelForTrustStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SearchForTrust));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_CaseTypeIsSelectCaseType_WithTrustUkPrnSetAndAddressFound_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");
			
			mockUserService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(new UserState(userName){TrustUkPrn = "some uk prn"});
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);
			
			mockTrustService
				.Setup(t => t.GetTrustAddressByUkPrn(userName))
				.ReturnsAsync(trustAddress);

			mockTrustService
				.Setup(s => s.GetTrustAddressByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(trustAddress);
			
			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Not.Null);
				Assert.That(sut.TrustAddress.TrustName, Is.EqualTo(trustAddress.TrustName));
				Assert.That(sut.TrustAddress.County, Is.EqualTo(trustAddress.County));
				Assert.That(sut.TrustAddress.DisplayAddress, Is.EqualTo(trustAddress.DisplayAddress));
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_CaseTypeIsSelectCaseType_WithTrustAddressNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var prn = "some trustukprn";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);
			
			mockUserService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(new UserState(userName){ TrustUkPrn = prn });

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			
			// act / assert
			var result = await sut.OnGet();
			
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Could not find trust with UK PRN of {prn}");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_WithTrustUkPrnNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName));

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act / assert
			var result = await sut.OnGet();
			
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Could not retrieve trust from cache for user '{userName}'");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_CaseTypeIsSelectCaseType_WithUserStateNull_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			
			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Could not retrieve cache for user '{userName}'");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_CaseTypeIsSelectCaseType_WithUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			
			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Some error message");
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		[TestCase(CreateCasePageModel.CaseTypes.Concern, "/case/concern/index")]
		[TestCase(CreateCasePageModel.CaseTypes.NonConcern, "/case/create/nonconcerns")]
		public async Task WhenOnPost_WithConcernsType_RedirectsToConcernsCreatePage(CreateCasePageModel.CaseTypes selectedCaseType, string expectedUrl)
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);
			
			mockTrustService
				.Setup(t => t.GetTrustAddressByUkPrn(userName))
				.ReturnsAsync(trustAddress);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName));

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			sut.CaseType = selectedCaseType;

			// act
			var result = await sut.OnPost();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SearchForTrust));
				Assert.That(sut.CaseType, Is.EqualTo(selectedCaseType));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
				Assert.That(result, Is.TypeOf<RedirectResult>());
				Assert.That((result as RedirectResult)?.Url, Is.EqualTo(expectedUrl));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnPost_WithNoSelectedCaseType_ReturnsPageWithError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName));

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			sut.TrustAddress = trustAddress;

			// act
			var result = await sut.OnPost();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Not.Null);
				Assert.That(sut.TrustAddress.TrustName, Is.EqualTo(trustAddress.TrustName));
				Assert.That(sut.TrustAddress.County, Is.EqualTo(trustAddress.County));
				Assert.That(sut.TrustAddress.DisplayAddress, Is.EqualTo(trustAddress.DisplayAddress));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnPost_WithCurrentUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnPost();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasCalled("Some error message");
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		[TestCase("abc")]
		[TestCase("1234")]
		public async Task WhenOnGetTrustsSearchResult_WithValidSearchQuery_ReturnsSearchResults(string searchString)
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var searchResults = TrustFactory.BuildListTrustSummaryModel();
			
			mockTrustService
				.Setup(t => t.GetTrustsBySearchCriteria(It.Is<TrustSearch>(s => s.Ukprn == searchString && s.GroupName == searchString && s.CompaniesHouseNumber == searchString)))
				.ReturnsAsync(searchResults);

			var sut = SetupPageModelForTrustStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGetTrustsSearchResult(searchString);
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SearchForTrust));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				
				Assert.That(result, Is.TypeOf<JsonResult>());
				
				var jsonResult = result as JsonResult;
				Assert.That(jsonResult?.Value, Is.Not.Null);
				Assert.That(jsonResult.Value, Is.TypeOf<List<TrustSearchModel>>());
				
				var searchModelResult = jsonResult.Value as List<TrustSearchModel>;
				Assert.That(searchModelResult, Is.Not.Null);
				Assert.That(searchModelResult, Has.Count.EqualTo(searchResults.Count));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		[TestCase("ab")]
		[TestCase("1")]
		[TestCase("")]
		[TestCase(null)]
		public async Task WhenOnGetTrustsSearchResult_WithInvalidSearchQuery_DoesNotErrorAndReturnsEmptySearchResults(string searchString)
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModelForTrustStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGetTrustsSearchResult(searchString);
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SearchForTrust));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<JsonResult>());
				
				var jsonResult = result as JsonResult;
				Assert.That(jsonResult?.Value, Is.Not.Null);
				Assert.That(jsonResult.Value, Is.TypeOf<List<TrustSearchModel>>());
				
				var searchModelResult = jsonResult.Value as List<TrustSearchModel>;
				Assert.That(searchModelResult, Is.Not.Null);
				Assert.That(searchModelResult, Has.Count.EqualTo(0));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
				
		[Test]
		[TestCase("abc")]
		[TestCase("123")]
		public async Task WhenOnGetTrustsSearchResult_WithErrorThrownBySearchService_ReturnsErrorStatusCode(string searchString)
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModelForTrustStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
						
			mockTrustService
				.Setup(t => t.GetTrustsBySearchCriteria(It.Is<TrustSearch>(s => s.Ukprn == searchString && s.GroupName == searchString && s.CompaniesHouseNumber == searchString)))
				.Throws(() => new Exception("some error message"));

			// act
			var result = await sut.OnGetTrustsSearchResult(searchString);
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SearchForTrust));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<ObjectResult>());
				
				Assert.That(result, Is.Not.Null);
				Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGetTrustsSearchResult");
			mockLogger.VerifyLogErrorWasCalled("some error message");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		[TestCase("abcdef")]
		[TestCase("1234")]
		public async Task WhenOnGetSelectedTrust_WithValidTrustUkPrn_CachesTrustAndRedirectsToNextStep(string trustUkPrn)
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = "some name of a user";
			
			mockUserService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(new UserState(userName){TrustUkPrn = trustUkPrn});
			
			mockUserService
				.Setup(s => s.StoreData(It.IsAny<string>(), It.IsAny<UserState>()))
				.Verifiable();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModelForTrustStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGetSelectedTrust(trustUkPrn);
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				
				Assert.That(result, Is.TypeOf<JsonResult>());
				
				var jsonResult = result as JsonResult;
				Assert.That(jsonResult?.Value, Is.Not.Null);
				Assert.That(jsonResult.Value?.ToString(), Is.EqualTo("{ redirectUrl = /case/create }"));
				
			});
			
			mockUserService.Verify(s => s.StoreData(userName, It.Is<UserState>(us => us.TrustUkPrn == trustUkPrn)));
			mockLogger.VerifyLogInformationWasCalled("OnGetSelectedTrust");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase("12")]
		[TestCase("1")]
		[TestCase("contains-hyphen")]
		public async Task WhenOnGetSelectedTrust_WithInvalidTrustUkPrn_CachesTrustAndRedirectsToNextStep(string trustUkPrn)
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = "some name of a user";
			
			mockUserService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(new UserState(userName){TrustUkPrn = trustUkPrn});
			
			mockUserService
				.Setup(s => s.StoreData(It.IsAny<string>(), It.IsAny<UserState>()))
				.Verifiable();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModelForTrustStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGetSelectedTrust(trustUkPrn);
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SearchForTrust));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<ObjectResult>());
				
				Assert.That(result, Is.Not.Null);
				Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
			});
			
			mockUserService.Verify(s => s.StoreData(userName, It.Is<UserState>(us => us.TrustUkPrn == trustUkPrn)), 
				Times.Never);
			mockLogger.VerifyLogInformationWasCalled("OnGetSelectedTrust");
			mockLogger.VerifyLogErrorWasCalled($"Selected trust is incorrect - {trustUkPrn}");
			mockLogger.VerifyNoOtherCalls();
		}
		
		private static CreateCasePageModel SetupPageModel(
			IMock<ILogger<CreateCasePageModel>> mockLogger,
			IMock<ITrustModelService> mockTrustService,
			IMock<IUserStateCachedService> mockUserService,
			IMock<IClaimsPrincipalHelper> mockClaimsHelper)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			return new CreateCasePageModel(mockTrustService.Object, mockUserService.Object, mockLogger.Object, mockClaimsHelper.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
		
		private static CreateCasePageModel SetupPageModelForCaseStep(
			IMock<ILogger<CreateCasePageModel>> mockLogger,
			IMock<ITrustModelService> mockTrustService,
			IMock<IUserStateCachedService> mockUserService,
			IMock<IClaimsPrincipalHelper> mockClaimsHelper)
		{
			var model = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsHelper);
			model.CreateCaseStep = CreateCasePageModel.CreateCaseSteps.SelectCaseType;
			return model;
		}
		
		private static CreateCasePageModel SetupPageModelForTrustStep(
			IMock<ILogger<CreateCasePageModel>> mockLogger,
			IMock<ITrustModelService> mockTrustService,
			IMock<IUserStateCachedService> mockUserService,
			IMock<IClaimsPrincipalHelper> mockClaimsHelper)
		{
			var model = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsHelper);
			model.CreateCaseStep = CreateCasePageModel.CreateCaseSteps.SearchForTrust;
			return model;
		}
	}
}