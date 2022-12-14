using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.CreateCase;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
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
			var trustUkPrn = "12345";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");

			mockUserService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(new UserState(userName) {TrustUkPrn = trustUkPrn});

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockTrustService
				.Setup(t => t.GetTrustAddressByUkPrn(trustUkPrn))
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
			var trustUkPrn = "12345";

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockUserService
				.Setup(s => s.GetData(userName))
				.ReturnsAsync(new UserState(userName){ TrustUkPrn = trustUkPrn });

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
			mockLogger.VerifyLogErrorWasCalled($"Could not find trust with UK PRN of {trustUkPrn}");
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
			var trustUkPrn = "12345";

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName) { TrustUkPrn = trustUkPrn });

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
			mockLogger.VerifyLogErrorWasCalled($"Could not find trust with UK PRN of {trustUkPrn}");
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
			var trustUkPrn = "12345";

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
			var trustUkPrn = "12345";

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
			var trustUkPrn = "12345";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockTrustService
				.Setup(t => t.GetTrustAddressByUkPrn(trustUkPrn))
				.ReturnsAsync(trustAddress);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName) { TrustUkPrn = trustUkPrn });

			var sut = SetupPageModelForCaseStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			sut.CaseType = selectedCaseType;

			// act
			var result = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
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
			var trustUkPrn = "12345";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockUserService.Setup(s => s.GetData(userName)).ReturnsAsync(new UserState(userName) { TrustUkPrn = trustUkPrn});

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


				// assert that a model validation error exists for the case type.
				ModelStateEntry caseTypeModelStateEntry;
				Assert.That(sut.ModelState.TryGetValue("CaseType", out caseTypeModelStateEntry), Is.True);
				Assert.That(caseTypeModelStateEntry.ValidationState, Is.EqualTo(ModelValidationState.Invalid));
				Assert.That(caseTypeModelStateEntry.Errors.First().ErrorMessage, Is.EqualTo("Invalid case type"));
				Assert.That(result, Is.TypeOf<PageResult>());
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
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
			var trustUkPrn = "12345";

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
		public async Task OnPostSelectedTrust_WithInvalidTrustUkprn_Returns_Error()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateCasePageModel>>();
			var mockTrustService = new Mock<ITrustModelService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var searchResults = TrustFactory.BuildListTrustSummaryModel();

			var sut = SetupPageModelForTrustStep(mockLogger, mockTrustService, mockUserService, mockClaimsPrincipalHelper);
			SetPageModelToHaveInvalidTrustSearch(sut);

			// act
			var result = await sut.OnPostSelectedTrust();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SearchForTrust));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
			});

			mockTrustService.VerifyNoOtherCalls();
			mockLogger.VerifyLogInformationWasCalled("OnPostSelectedTrust");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		[TestCase("abcdef")]
		[TestCase("1234")]
		public async Task WhenOnPostSelectedTrust_WithValidTrustUkPrn_CachesTrustAndRedirectsToNextStep(string trustUkPrn)
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
			sut.FindTrustModel.SelectedTrustUkprn = trustUkPrn;
			var result = await sut.OnPostSelectedTrust();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CreateCaseStep, Is.EqualTo(CreateCasePageModel.CreateCaseSteps.SelectCaseType));
				Assert.That(sut.CaseType, Is.EqualTo(CreateCasePageModel.CaseTypes.NotSelected));
				Assert.That(sut.TempData["Error.Message"], Is.Null);


				var redirectResult = result as RedirectToPageResult;
				Assert.That(redirectResult.PageName, Is.EqualTo("CreateCase"));

			});

			mockUserService.Verify(s => s.StoreData(userName, It.Is<UserState>(us => us.TrustUkPrn == trustUkPrn)));
			mockLogger.VerifyLogInformationWasCalled("OnPostSelectedTrust");
			mockLogger.VerifyLogErrorWasNotCalled();
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


		private static void SetPageModelToHaveInvalidTrustSearch(CreateCasePageModel model)
		{
			model.FindTrustModel.SelectedTrustUkprn = string.Empty;
			model.ModelState.AddModelError("trustError", "Trust search is invalid");
		}
	}
}