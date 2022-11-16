using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;
using ConcernsCaseWork.Redis.Users;
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
	public class CreateNonConcernsCasePageModelTests
	{
		private readonly IFixture _fixture = new Fixture();

		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new CreateNonConcernsCasePageModel(
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<CreateNonConcernsCasePageModel>>(),
					null,
					Mock.Of<ICreateCaseService>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}

		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new CreateNonConcernsCasePageModel(
					Mock.Of<IUserStateCachedService>(),
					null,
					Mock.Of<IClaimsPrincipalHelper>(),
					Mock.Of<ICreateCaseService>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'logger')"));
		}

		[Test]
		public void Constructor_WithNullUserStateCachedService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new CreateNonConcernsCasePageModel(
					null,
					Mock.Of<ILogger<CreateNonConcernsCasePageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>(),
					Mock.Of<ICreateCaseService>()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'cachedService')"));
		}

		[Test]
		public void Constructor_WithNullCreateCaseService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new CreateNonConcernsCasePageModel(
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<CreateNonConcernsCasePageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>(),
					null));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'createCaseService')"));
		}

		[Test]
		public async Task WhenOnPost_WithDecisionSelectedAndCurrentUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateNonConcernsCasePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.SelectedActionOrDecision = CreateNonConcernsCasePageModel.Options.Decision;

			// act
			var result = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
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
			var mockLogger = new Mock<ILogger<CreateNonConcernsCasePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();
			var caseUrn = _fixture.Create<long>();
			var expectedUrl = $"/case/{caseUrn}/management/action/decision/addOrUpdate";

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockCreateCaseService.Setup(s => s.CreateNonConcernsCase(userName)).ReturnsAsync(caseUrn);

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.SelectedActionOrDecision = CreateNonConcernsCasePageModel.Options.Decision;

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
		public async Task WhenOnPost_WithSrmaActionSelected_RedirectsToNonConcernsAddSrmaPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateNonConcernsCasePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();
			var caseUrn = _fixture.Create<long>();
			var expectedUrl = "/case/create/nonconcerns/srma";

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockCreateCaseService.Setup(s => s.CreateNonConcernsCase(userName)).ReturnsAsync(caseUrn);

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.SelectedAction = CreateNonConcernsCasePageModel.Actions.SRMA;

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
		public async Task WhenOnPost_WithTFFActionSelected_ReturnsNotImplementedError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateNonConcernsCasePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.SelectedAction = CreateNonConcernsCasePageModel.Actions.TFF;

			// act
			var result = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
				Assert.That(result, Is.TypeOf<PageResult>());
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasCalled("The method or operation is not implemented.");
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public async Task WhenOnPost_WithNoActionSelected_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateNonConcernsCasePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);

			// act
			var result = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<PageResult>());
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public void WhenOnGet_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<CreateNonConcernsCasePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.SelectedAction = CreateNonConcernsCasePageModel.Actions.None;
			sut.SelectedActionOrDecision = CreateNonConcernsCasePageModel.Options.None;

			// act
			var result = sut.OnGet();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.SelectedAction, Is.EqualTo(CreateNonConcernsCasePageModel.Actions.None));
				Assert.That(sut.SelectedActionOrDecision, Is.EqualTo(CreateNonConcernsCasePageModel.Options.None));
			});

			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		private static CreateNonConcernsCasePageModel SetupPageModel(
			IMock<IUserStateCachedService> mockUserStateCachedService,
			IMock<ILogger<CreateNonConcernsCasePageModel>> mockLogger,
			IMock<IClaimsPrincipalHelper> mockClaimsPrincipleService,
			IMock<ICreateCaseService> mockCreateCaseService)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			return new CreateNonConcernsCasePageModel(mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipleService.Object, mockCreateCaseService.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}