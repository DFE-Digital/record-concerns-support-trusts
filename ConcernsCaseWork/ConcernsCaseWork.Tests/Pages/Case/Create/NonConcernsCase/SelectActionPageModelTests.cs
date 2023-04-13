using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases.Create;
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
using System;
using System.Security.Claims;

namespace ConcernsCaseWork.Tests.Pages.Case.Create.NonConcernsCase
{
	[Parallelizable(ParallelScope.All)]
	public class SelectActionPageModelTests
	{
		private readonly IFixture _fixture = new Fixture();

		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionPageModel(
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectActionPageModel>>(),
					null,
					Mock.Of<ICreateCaseService>(),
					MockTelemetry.CreateMockTelemetryClient()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}

		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionPageModel(
					Mock.Of<IUserStateCachedService>(),
					null,
					Mock.Of<IClaimsPrincipalHelper>(),
					Mock.Of<ICreateCaseService>(),
					MockTelemetry.CreateMockTelemetryClient()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'logger')"));
		}

		[Test]
		public void Constructor_WithNullUserStateCachedService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionPageModel(
					null,
					Mock.Of<ILogger<SelectActionPageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>(),
					Mock.Of<ICreateCaseService>(),
					MockTelemetry.CreateMockTelemetryClient()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'cachedService')"));
		}

		[Test]
		public void Constructor_WithNullCreateCaseService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new SelectActionPageModel(
					Mock.Of<IUserStateCachedService>(),
					Mock.Of<ILogger<SelectActionPageModel>>(),
					Mock.Of<IClaimsPrincipalHelper>(),
					null,
					MockTelemetry.CreateMockTelemetryClient()));

			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'createCaseService')"));
		}

		[Test]
		public void WhenOnPost_WithSrmaActionSelected_RedirectsToNonConcernsAddSrmaPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectActionPageModel>>();
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
			sut.SelectedAction = SelectActionPageModel.Actions.SRMA;

			// act
			var result = sut.OnPost();

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
		public void WhenOnPost_WithTFFActionSelected_ReturnsNotImplementedError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectActionPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.SelectedAction = SelectActionPageModel.Actions.TFF;

			// act
			var result = sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
				Assert.That(result, Is.TypeOf<PageResult>());
			});

			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasCalled("The method or operation is not implemented.");
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public void WhenOnPost_WithNoActionSelected_ReturnsValidationError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<SelectActionPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();

			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);

			// act
			var result = sut.OnPost();

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
			var mockLogger = new Mock<ILogger<SelectActionPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockUserService = new Mock<IUserStateCachedService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModel(mockUserService, mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);

			// act
			var result = sut.OnGet();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.SelectedAction, Is.EqualTo(default));
			});

			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		private static SelectActionPageModel SetupPageModel(
			IMock<IUserStateCachedService> mockUserStateCachedService,
			IMock<ILogger<SelectActionPageModel>> mockLogger,
			IMock<IClaimsPrincipalHelper> mockClaimsPrincipleService,
			IMock<ICreateCaseService> mockCreateCaseService)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			return new SelectActionPageModel(mockUserStateCachedService.Object, mockLogger.Object, mockClaimsPrincipleService.Object, 
				mockCreateCaseService.Object,MockTelemetry.CreateMockTelemetryClient())
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}