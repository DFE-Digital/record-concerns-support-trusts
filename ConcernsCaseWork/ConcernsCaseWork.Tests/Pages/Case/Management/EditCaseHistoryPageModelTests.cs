using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Pages.Case.Management;
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
using System.Security.Principal;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.All)]
	public class EditCaseHistoryPageModelTests
	{
		private readonly IFixture _fixture = new Fixture();

		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var userName = _fixture.Create<string>();
			var caseModel = CaseFactory.BuildCaseModel();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditCaseHistoryPageModel>>();
			var mockClaimsServicePrincipal = new Mock<IClaimsPrincipalHelper>();

			mockClaimsServicePrincipal.Setup(s => s.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(userName);

			mockCaseModelService.Setup(c => c.GetCaseByUrn(caseUrn)).ReturnsAsync(caseModel);

			var sut = SetupEditCaseHistoryPageModel(mockCaseModelService.Object, mockClaimsServicePrincipal.Object, mockLogger.Object);
			sut.CaseUrn = caseUrn;

			// act
			var pageResponse = await sut.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			Assert.Multiple(() =>
			{
				Assert.That(page, Is.Not.Null);
				Assert.That(sut.CaseHistory, Is.EqualTo(caseModel.CaseHistory));
				Assert.That(sut.ReferrerUrl, Is.EqualTo($"/case/{caseUrn}/management"));
			});

			mockCaseModelService.Verify(c => c.GetCaseByUrn(caseUrn), Times.Once);

			mockLogger.VerifyLogInformationWasCalled();
			mockLogger.VerifyLogErrorWasNotCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnsPage()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var userName = _fixture.Create<string>();
			var caseModel = CaseFactory.BuildCaseModel();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditCaseHistoryPageModel>>();
			var mockClaimsServicePrincipal = new Mock<IClaimsPrincipalHelper>();

			mockClaimsServicePrincipal.Setup(s => s.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(userName);

			mockCaseModelService.Setup(c => c.GetCaseByUrn(caseUrn)).ReturnsAsync(caseModel);

			var sut = SetupEditCaseHistoryPageModel(mockCaseModelService.Object, mockClaimsServicePrincipal.Object, mockLogger.Object);

			// act
			var pageResponse = await sut.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.Multiple(() =>
			{
				Assert.That(page, Is.Not.Null);
				Assert.That(sut.CaseHistory, Is.Null);
				Assert.That(sut.TempData, Is.Not.Null);
				Assert.That(sut.TempData["Error.Message"],
					Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			});

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<long>()), Times.Never);

			mockLogger.VerifyLogInformationWasCalled();
			mockLogger.VerifyLogErrorWasCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public async Task WhenOnPost_MissingCaseUrn_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditCaseHistoryPageModel>>();
			var mockClaimsServicePrincipal = new Mock<IClaimsPrincipalHelper>();

			var sut = SetupEditCaseHistoryPageModel(mockCaseModelService.Object, mockClaimsServicePrincipal.Object, mockLogger.Object);

			// act
			var pageResponse = await sut.OnPost();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.Multiple(() =>
			{
				Assert.That(page, Is.Not.Null);
				Assert.That(sut.CaseHistory, Is.Null);
				Assert.That(sut.TempData, Is.Not.Null);
				Assert.That(sut.TempData["Error.Message"],
					Is.EqualTo("There was an error and your changes were not saved. Refresh the page and if the problem continues, try again later. Email the Record concerns and support for trusts team at regionalservices.rg@education.gov.uk if this problem continues."));
			});

			mockCaseModelService.Verify(c =>
				c.GetCaseByUrn(It.IsAny<long>()), Times.Never);

			mockLogger.VerifyLogInformationWasCalled();
			mockLogger.VerifyLogErrorWasCalled();
			mockLogger.VerifyNoOtherCalls();
		}

		[Test]
		public async Task WhenOnPost_WithValidData_ReturnsToPreviousUrl()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var userName = _fixture.Create<string>();
			var caseHistory = _fixture.Create<string>();

			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditCaseHistoryPageModel>>();
			var mockClaimsServicePrincipal = new Mock<IClaimsPrincipalHelper>();

			mockClaimsServicePrincipal.Setup(s => s.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(userName);

			var sut = SetupEditCaseHistoryPageModel(mockCaseModelService.Object, mockClaimsServicePrincipal.Object, mockLogger.Object);
			sut.CaseUrn = caseUrn;
			sut.CaseHistory = caseHistory;

			// act
			var pageResponse = await sut.OnPost();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
				var page = pageResponse as RedirectResult;

				Assert.That(page, Is.Not.Null);
				Assert.That(page.Url, Is.EqualTo($"/case/{caseUrn}/management"));
			});

			mockCaseModelService.Verify(c =>
				c.PatchCaseHistory(caseUrn, userName, caseHistory), Times.Once);
		}

		private static EditCaseHistoryPageModel SetupEditCaseHistoryPageModel(
			ICaseModelService mockCaseModelService, IClaimsPrincipalHelper claimsPrincipalHelper, ILogger<EditCaseHistoryPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditCaseHistoryPageModel(mockCaseModelService, claimsPrincipalHelper, mockLogger)
			{
				PageContext = pageContext, TempData = tempData, Url = new UrlHelper(actionContext), MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}
