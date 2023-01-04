using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.CreateCase;
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
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Create
{
	[Parallelizable(ParallelScope.All)]
	public class SelectCaseTypePageModelTests
	{
		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectCaseTypePageModel(
					Mock.Of<ITrustModelService>(),
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectCaseTypePageModel>>(),
					null));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}
		
		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectCaseTypePageModel(
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
				_ = new SelectCaseTypePageModel(
					Mock.Of<ITrustModelService>(),
					null,
					Mock.Of<ILogger<SelectCaseTypePageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'cachedUserService')"));
		}

		[Test]
		public void Constructor_WithNullTrustModelService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectCaseTypePageModel(
					null,
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectCaseTypePageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'trustModelService')"));
		}
		
		[Test]
		public async Task WhenOnGet_NoCachedDataFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(default));
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_WithTrustUkPrnSetAndAddressFound_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
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
			
			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Not.Null);
				Assert.That(sut.TrustAddress.TrustName, Is.EqualTo(trustAddress.TrustName));
				Assert.That(sut.TrustAddress.County, Is.EqualTo(trustAddress.County));
				Assert.That(sut.TrustAddress.DisplayAddress, Is.EqualTo(trustAddress.DisplayAddress));
				Assert.That(sut.CaseType, Is.EqualTo(default));
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_WithTrustAddressNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
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

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			
			// act / assert
			var result = await sut.OnGet();
			
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(default));
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
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName));

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act / assert
			var result = await sut.OnGet();
			
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(default));
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Could not retrieve trust from cache for user '{userName}'");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_WithUserStateNull_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			
			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(default));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Could not retrieve cache for user '{userName}'");
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnGet_WithUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			
			// act
			var result = await sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(default));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Some error message");
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		[TestCase(SelectCaseTypePageModel.CaseTypes.Concern, "/case/concern/index")]
		[TestCase(SelectCaseTypePageModel.CaseTypes.NonConcern, "/case/create/nonconcerns")]
		public async Task WhenOnPost_WithConcernsType_RedirectsToConcernsCreatePage(SelectCaseTypePageModel.CaseTypes selectedCaseType, string expectedUrl)
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
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

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			sut.CaseType = selectedCaseType;

			// act
			var result = await sut.OnPost();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
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
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName));

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
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
				Assert.That(sut.CaseType, Is.EqualTo(default));
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
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);

			// act
			var result = await sut.OnPost();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(default));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasCalled("Some error message");
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public async Task WhenOnPost_WithValidationError_ReturnsPageWithError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectCaseTypePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModel(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			var keyName = "testkey";
			var errorMsg = "some model validation error";
			sut.ModelState.AddModelError(keyName, errorMsg);

			// act
			var result = await sut.OnPost();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(default));
				
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<PageResult>());
				
				Assert.That(sut.ModelState.IsValid, Is.False);
				Assert.That(sut.ModelState.Keys.Count(), Is.EqualTo(1));
				Assert.That(sut.ModelState.First().Key, Is.EqualTo(keyName));
				Assert.That(sut.ModelState.First().Value?.Errors.Single().ErrorMessage, Is.EqualTo(errorMsg));
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyNoOtherCalls();
		}

		private static SelectCaseTypePageModel SetupPageModel(
			IMock<ILogger<SelectCaseTypePageModel>> mockLogger,
			IMock<ITrustModelService> mockTrustService,
			IMock<IUserStateCachedService> mockUserService,
			IMock<IClaimsPrincipalHelper> mockClaimsHelper)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			return new SelectCaseTypePageModel(mockTrustService.Object, mockUserService.Object, mockLogger.Object, mockClaimsHelper.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}