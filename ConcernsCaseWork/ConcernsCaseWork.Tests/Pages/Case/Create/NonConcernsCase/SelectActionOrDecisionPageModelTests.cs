using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Cases.Create;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Create.NonConcernsCase
{
	[Parallelizable(ParallelScope.All)]
	public class SelectActionOrDecisionPageModelTests
	{
		private readonly IFixture _fixture = new Fixture();

		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionOrDecisionPageModel(
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectActionOrDecisionPageModel>>(),
					null,
					Mock.Of<ICreateCaseService>(),
					Mock.Of<ITrustService>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}

		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionOrDecisionPageModel(
					Mock.Of<IUserStateCachedService>(),
					null,
					Mock.Of<IClaimsPrincipalHelper>(),
					Mock.Of<ICreateCaseService>(),
					Mock.Of<ITrustService>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'logger')"));
		}

		[Test]
		public void Constructor_WithNullUserStateCachedService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionOrDecisionPageModel(
					null,
					Mock.Of<ILogger<SelectActionOrDecisionPageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>(),
					Mock.Of<ICreateCaseService>(),
					Mock.Of<ITrustService>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'cachedService')"));
		}

		[Test]
		public void Constructor_WithNullCreateCaseService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionOrDecisionPageModel(
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectActionOrDecisionPageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>(),
					null,
					Mock.Of<ITrustService>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'createCaseService')"));
		}

		[Test]
		public async Task WhenOnPost_WithDecisionSelectedAndCurrentUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectActionOrDecisionPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockTrustService = new Mock<ITrustService>();

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService, mockTrustService);
			sut.SelectedActionOrDecision = SelectActionOrDecisionPageModel.Options.Decision;

			// act
			var result = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
				Assert.That(result, Is.TypeOf<PageResult>());
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasCalled("Some error message");
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public async Task WhenOnPost_WithDecisionSelected_RedirectsToDecisionsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectActionOrDecisionPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockTrustService = new Mock<ITrustService>();
			
			var userName = _fixture.Create<string>();
			var caseUrn = _fixture.Create<long>();
			var expectedUrl = $"/case/{caseUrn}/management/action/decision/addOrUpdate";
			var trustUkPrn = _fixture.Create<string>();
			var trustCompaniesHouseNumber = _fixture.CreateMany<char>(8).ToString();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockCreateCaseService.Setup(s => s.CreateNonConcernsCase(userName, trustUkPrn, trustCompaniesHouseNumber)).ReturnsAsync(caseUrn);

			mockUserService.Setup(x => x.GetData(userName)).ReturnsAsync(new UserState(userName) { TrustUkPrn = trustUkPrn });
			var mockTrust = new Mock<TrustDetailsDto>();
			mockTrust.Setup(x => x.GiasData.UkPrn).Returns(trustUkPrn);
			mockTrust.Setup(x => x.GiasData.CompaniesHouseNumber).Returns(trustCompaniesHouseNumber);
			mockTrustService.Setup(x => x.GetTrustByUkPrn(trustUkPrn)).ReturnsAsync(mockTrust.Object);					
			
			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService, mockTrustService);
			sut.SelectedActionOrDecision = SelectActionOrDecisionPageModel.Options.Decision;

			// act
			var result = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<RedirectResult>());
				Assert.That((result as RedirectResult)?.Url, Is.EqualTo(expectedUrl));
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnPost_WithActionSelected_RedirectsToSelectActionPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectActionOrDecisionPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserStateService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockTrustService = new Mock<ITrustService>();
			var mockTrust = new Mock<TrustDetailsDto>();
			
			var userName = _fixture.Create<string>();
			var caseUrn = _fixture.Create<long>();
			var expectedUrl = $"/case/create/nonconcerns/action";
			var trustUkPrn = _fixture.Create<string>();
			var trustCompaniesHouseNumber = _fixture.CreateMany<char>(8).ToString();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockCreateCaseService.Setup(s => s.CreateNonConcernsCase(userName, trustUkPrn, trustCompaniesHouseNumber)).ReturnsAsync(caseUrn);

			mockUserStateService.Setup(x => x.GetData(userName)).ReturnsAsync(new UserState(userName) { TrustUkPrn = trustUkPrn });
			mockTrust.Setup(x => x.GiasData.UkPrn).Returns(trustUkPrn);
			mockTrust.Setup(x => x.GiasData.CompaniesHouseNumber).Returns(trustCompaniesHouseNumber);
			mockTrustService.Setup(x => x.GetTrustByUkPrn(trustUkPrn)).ReturnsAsync(mockTrust.Object);			
			
			var sut = SetupPageModel(mockUserStateService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService, mockTrustService);
			sut.SelectedActionOrDecision = SelectActionOrDecisionPageModel.Options.Action;

			// act
			var result = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<RedirectResult>());
				Assert.That((result as RedirectResult)?.Url, Is.EqualTo(expectedUrl));
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public void WhenOnGet_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectActionOrDecisionPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			var mockTrustService = new Mock<ITrustService>();
			
			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService, mockTrustService);

			// act
			var result = sut.OnGet();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.SelectedActionOrDecision, Is.EqualTo(default));
			});

			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		private static SelectActionOrDecisionPageModel SetupPageModel(
			IMock<IUserStateCachedService> mockUserStateCachedService,
			IMock<ILogger<SelectActionOrDecisionPageModel>> mockLogger,
			IMock<IClaimsPrincipalHelper> mockClaimsPrincipleService,
			IMock<ICreateCaseService> mockCreateCaseService,
			IMock<ITrustService> mockTrustService)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			return new SelectActionOrDecisionPageModel(mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipleService.Object, mockCreateCaseService.Object, mockTrustService.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}