using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management;

[Parallelizable(ParallelScope.All)]
public class EditTerritoryPageModelTests
{
	private readonly IFixture _fixture = new Fixture();

	[Test]
	public async Task WhenOnGetAsync_ReturnsPage()
	{
		// arrange
		var caseUrn = _fixture.Create<int>();
		var userName = _fixture.Create<string>();
		var caseModel = CaseFactory.BuildCaseModel();

		var mockCaseModelService = new Mock<ICaseModelService>();
		var mockLogger = new Mock<ILogger<EditTerritoryPageModel>>();
		var mockClaimsServicePrincipal = new Mock<IClaimsPrincipalHelper>();

		mockClaimsServicePrincipal.Setup(s => s.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(userName);

		mockCaseModelService.Setup(c => c.GetCaseByUrn(caseUrn)).ReturnsAsync(caseModel);

		var sut = SetupEditTerritoryPageModel(mockCaseModelService.Object, mockClaimsServicePrincipal.Object, mockLogger.Object);
		sut.CaseUrn = caseUrn;

		// act
		var pageResponse = await sut.OnGetAsync();

		// assert
		Assert.That(pageResponse, Is.InstanceOf<PageResult>());
		var page = pageResponse as PageResult;
		Assert.Multiple(() =>
		{
			Assert.That(page, Is.Not.Null);
			Assert.That((Territory)sut.Territory.SelectedId, Is.EqualTo(caseModel.Territory));
			Assert.That(sut.ReferrerUrl, Is.EqualTo($"/case/{caseUrn}/management"));
		});

		mockCaseModelService.Verify(c =>
			c.GetCaseByUrn(caseUrn), Times.Once);

		mockLogger.VerifyLogInformationWasCalled();
		mockLogger.VerifyLogErrorWasNotCalled();
		mockLogger.VerifyNoOtherCalls();
	}

	[Test]
	public async Task WhenOnPost_WithValidData_ReturnsPage()
	{
		// arrange
		var caseUrn = _fixture.Create<int>();
		var userName = _fixture.Create<string>();
		var caseTerritory = _fixture.Create<Territory>();

		var mockCaseModelService = new Mock<ICaseModelService>();
		var mockLogger = new Mock<ILogger<EditTerritoryPageModel>>();
		var mockClaimsServicePrincipal = new Mock<IClaimsPrincipalHelper>();

		mockClaimsServicePrincipal.Setup(s => s.GetPrincipalName(It.IsAny<IPrincipal>())).Returns(userName);

		var sut = SetupEditTerritoryPageModel(mockCaseModelService.Object, mockClaimsServicePrincipal.Object, mockLogger.Object);
		sut.CaseUrn = caseUrn;
		sut.Territory = _fixture.Create<RadioButtonsUiComponent>();
		sut.Territory.SelectedId = (int)caseTerritory;

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
			c.PatchTerritory(caseUrn, userName, caseTerritory), Times.Once);
	}

	private static EditTerritoryPageModel SetupEditTerritoryPageModel(
		ICaseModelService mockCaseModelService, IClaimsPrincipalHelper claimsPrincipalHelper, ILogger<EditTerritoryPageModel> mockLogger, bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

		return new EditTerritoryPageModel(mockCaseModelService, claimsPrincipalHelper, mockLogger)
		{
			PageContext = pageContext, TempData = tempData, Url = new UrlHelper(actionContext), MetadataProvider = pageContext.ViewData.ModelMetadata
		};
	}
}