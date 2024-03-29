using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
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
		private static Fixture _fixture = new();
		
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
				Assert.That(result, Is.TypeOf<PageResult>());
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnGet");
			mockLogger.VerifyLogErrorWasNotCalled();
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
		[TestCase(CaseType.Concerns, "/case/concern")]
		[TestCase(CaseType.NonConcerns, "/case/create/nonconcerns/details")]
		public async Task WhenOnPost_WithConcernsType_RedirectsToConcernsCreatePage(CaseType selectedCaseType, string expectedUrl)
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
			sut.CaseType = _fixture.Create<RadioButtonsUiComponent>();
			sut.CaseType.SelectedId = (int)selectedCaseType;

			// act
			var result = await sut.OnPost();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(sut.TrustAddress, Is.Null);
				Assert.That(sut.TempData["Error.Message"], Is.EqualTo(null));
				Assert.That(result, Is.TypeOf<RedirectResult>());
				Assert.That((result as RedirectResult)?.Url, Is.EqualTo(expectedUrl));
			});
			
			mockLogger.VerifyLogInformationWasCalled("OnPost");
			mockLogger.VerifyLogErrorWasNotCalled();
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