using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase;
using ConcernsCaseWork.Services.Cases.Create;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
using FluentAssertions;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Create.NonConcernsCase
{
	[Parallelizable(ParallelScope.All)]
	public class AddSrmaPageModelTests
	{
		private readonly IFixture _fixture = new Fixture();
		
		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new AddSrmaPageModel(
					null,
					Mock.Of<ICreateCaseService>(),
					Mock.Of<ILogger<AddSrmaPageModel>>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}
		
		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new AddSrmaPageModel(
					Mock.Of<IClaimsPrincipalHelper>(),
					Mock.Of<ICreateCaseService>(),
					null));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'logger')"));
		}
				
		[Test]
		public void Constructor_WithNullCreateCaseService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new AddSrmaPageModel(
					Mock.Of<IClaimsPrincipalHelper>(),
					null,
					Mock.Of<ILogger<AddSrmaPageModel>>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'createCaseService')"));
		}
		
		[Test]
		public async Task WhenOnPost_WithCurrentUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddSrmaPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModel(mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			// act
			var result = await sut.OnPostAsync();
			
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
		public async Task WhenOnPost_WithValidValues_RedirectsToCasePage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddSrmaPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();
			var caseUrn = _fixture.Create<long>();
			var expectedRedirectUrl = $"/case/{caseUrn}/management";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			mockCreateCaseService.Setup(s => s.CreateNonConcernsCase(userName, It.IsAny<SRMAModel>())).ReturnsAsync(caseUrn);

			var sut = SetupPageModel(mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.Notes = _fixture.Create<string>();
			sut.OfferedDate = new ConcernsDateValidatable{ Day = DateTime.Now.Day.ToString(), Month = DateTime.Now.Month.ToString(), Year = DateTime.Now.Year.ToString() };
			sut.Status = SRMAStatus.TrustConsidering;

			// act
			var result = await sut.OnPostAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<RedirectResult>());
				Assert.That(((RedirectResult)result).Url, Is.EqualTo(expectedRedirectUrl));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public async Task WhenOnPost_WithValidationError_ReturnsPageWithError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<AddSrmaPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var userName = _fixture.Create<string>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			sut.ModelState.AddModelError(nameof(sut.Status), "Status is required.");

			// act
			var result = await sut.OnPostAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.ModelState.IsValid, Is.EqualTo(false));
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
			var mockLogger = new Mock<ILogger<AddSrmaPageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupPageModel(mockLogger, mockClaimsPrincipalHelper, mockCreateCaseService);
			
			// act
			var result = sut.OnGet();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}
		
		[Test]
		public void Statuses_ShouldContainOpenStatuses()
		{
			// arrange
			var sut = new AddSrmaPageModel(Mock.Of<IClaimsPrincipalHelper>(), Mock.Of<ICreateCaseService>(), Mock.Of<ILogger<AddSrmaPageModel>>());
			
			// act
			var result = sut.SRMAStatuses.Select(s => s.Id).ToArray();
			
			// assert
			result.Should().NotBeNullOrEmpty();
			result.Length.Should().Be(3);
			result.Should().Contain(SRMAStatus.TrustConsidering.ToString());
			result.Should().Contain(SRMAStatus.PreparingForDeployment.ToString());
			result.Should().Contain(SRMAStatus.Deployed.ToString());
		}

		private static AddSrmaPageModel SetupPageModel(
			IMock<ILogger<AddSrmaPageModel>> mockLogger,
			IMock<IClaimsPrincipalHelper> mockClaimsPrincipleService,
			IMock<ICreateCaseService> mockCreateCaseService)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			return new AddSrmaPageModel(mockClaimsPrincipleService.Object, mockCreateCaseService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}