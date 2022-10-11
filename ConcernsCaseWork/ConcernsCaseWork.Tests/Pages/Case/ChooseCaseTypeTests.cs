using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
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

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class ChooseCaseTypeTests
	{
		[Test]
		public void Constructor_WithNullClaimsService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new ChooseCaseTypePageModel(
					Mock.Of<ILogger<ChooseCaseTypePageModel>>(), 
					Mock.Of<ICreateCaseService>(), 
					null));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'claimsPrincipalHelper')"));
		}
		
		[Test]
		public void Constructor_WithNullLogger_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new ChooseCaseTypePageModel(
					null, 
					Mock.Of<ICreateCaseService>(), 
					Mock.Of<IClaimsPrincipalHelper>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'logger')"));
		}
				
		[Test]
		public void Constructor_WithNullCreateCaseService_ThrowsException()
		{
			var exception = Assert.Throws<ArgumentNullException>(() =>
				_ = new ChooseCaseTypePageModel(
					Mock.Of<ILogger<ChooseCaseTypePageModel>>(), 
					null, 
					Mock.Of<IClaimsPrincipalHelper>()));
			
			Assert.That(exception?.Message, Is.EqualTo("Value cannot be null. (Parameter 'createCaseService')"));
		}
		
		[Test]
		public async Task WhenOnGet_WithTrustNameSet_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ChooseCaseTypePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);
			
			mockCreateCaseService
				.Setup(t => t.GetSelectedTrustAddress(userName))
				.ReturnsAsync(trustAddress);
			
			var sut = SetupPageModel(mockLogger, mockCreateCaseService, mockClaimsPrincipalHelper);
			
			// act
			var result = await sut.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Not.Null);
				Assert.That(sut.TrustAddress.TrustName, Is.EqualTo(trustAddress.TrustName));
				Assert.That(sut.TrustAddress.County, Is.EqualTo(trustAddress.County));
				Assert.That(sut.TrustAddress.DisplayAddress, Is.EqualTo(trustAddress.DisplayAddress));
				Assert.That(sut.CaseType, Is.EqualTo(0));
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
		}
		
		[Test]
		public async Task WhenOnGet_WithTrustNameNull_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ChooseCaseTypePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockLogger, mockCreateCaseService, mockClaimsPrincipalHelper);
			
			// act / assert
			var result = await sut.OnGetAsync();
			
			Assert.Multiple(() =>
			{
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(0));
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Could not retrieve trust from cache for user '{userName}'");
		}
		
		[Test]
		public async Task WhenOnGet_WithStateNull_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ChooseCaseTypePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);

			var sut = SetupPageModel(mockLogger, mockCreateCaseService, mockClaimsPrincipalHelper);
			
			// act
			var result = await sut.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(0));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Could not retrieve trust from cache for user '{userName}'");
		}
		
		[Test]
		public async Task WhenOnGet_WithUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ChooseCaseTypePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModel(mockLogger, mockCreateCaseService, mockClaimsPrincipalHelper);
			
			// act
			var result = await sut.OnGetAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(0));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasCalled($"Some error message");
		}

		[Test]
		public async Task WhenOnPost_WithValidValues_RedirectsToPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ChooseCaseTypePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);
			
			mockCreateCaseService
				.Setup(t => t.GetSelectedTrustAddress(userName))
				.ReturnsAsync(trustAddress);

			var sut = SetupPageModel(mockLogger, mockCreateCaseService, mockClaimsPrincipalHelper);
			sut.CaseType = 1;

			// act
			var result = await sut.OnPostAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(1));
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
				Assert.That(result, Is.TypeOf<RedirectToPageResult>());
				Assert.That((result as RedirectToPageResult)?.PageName, Is.EqualTo("Concern/Index"));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
		}
		
		[Test]
		public async Task WhenOnPost_WithNoSelectedCaseType_ReturnsPage()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ChooseCaseTypePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			var userName = "some user name";
			var trustAddress = new TrustAddressModel("Some trust name", "a county", "Full display address, town, postcode");
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Returns(userName);
			
			mockCreateCaseService
				.Setup(t => t.GetSelectedTrustAddress(userName))
				.ReturnsAsync(trustAddress);

			var sut = SetupPageModel(mockLogger, mockCreateCaseService, mockClaimsPrincipalHelper);
			sut.ModelState.AddModelError("missingCaseType", "Case Type is missing");

			// act
			var result = await sut.OnPostAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Not.Null);
				Assert.That(sut.TrustAddress.TrustName, Is.EqualTo(trustAddress.TrustName));
				Assert.That(sut.TrustAddress.County, Is.EqualTo(trustAddress.County));
				Assert.That(sut.TrustAddress.DisplayAddress, Is.EqualTo(trustAddress.DisplayAddress));
				Assert.That(sut.CaseType, Is.EqualTo(0));
				Assert.That(sut.TempData["Error.Message"], Is.Null);
				Assert.That(result, Is.TypeOf<PageResult>());
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
		}
		
		[Test]
		public async Task WhenOnPost_WithCurrentUserNotFound_ReturnsError()
		{
			// arrange
			var mockLogger = new Mock<ILogger<ChooseCaseTypePageModel>>();
			var mockCreateCaseService = new Mock<ICreateCaseService>();
			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			
			mockClaimsPrincipalHelper
				.Setup(t => t.GetPrincipalName(It.IsAny<ClaimsPrincipal>()))
				.Throws(new NullReferenceException("Some error message"));

			var sut = SetupPageModel(mockLogger, mockCreateCaseService, mockClaimsPrincipalHelper);
			sut.CaseType = 1;

			// act
			var result = await sut.OnPostAsync();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.CaseType, Is.EqualTo(1));			
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
				Assert.That(result, Is.TypeOf<RedirectToPageResult>());
				Assert.That((result as RedirectToPageResult)?.PageName, Is.EqualTo("choosecasetype"));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasCalled("Some error message");
		}
		
		private static ChooseCaseTypePageModel SetupPageModel(
			Mock<ILogger<ChooseCaseTypePageModel>> mockLogger,
			Mock<ICreateCaseService> mockService,
			Mock<IClaimsPrincipalHelper> mockClaimsHelper)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(true);

			return new ChooseCaseTypePageModel(mockLogger.Object, mockService.Object, mockClaimsHelper.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}