using AutoFixture;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA.Edit;

[Parallelizable(ParallelScope.All)]
public class EditSRMAReasonOfferedPageModelTests
{
	private Fixture _fixture;

	public EditSRMAReasonOfferedPageModelTests()
	{
		_fixture = new ();
	}

	[Test] 
	public async Task WhenOnGetAsync_ReturnsPage()
	{
		// arrange 
		var mockSRMAModelService = new Mock<ISRMAService>();
		var mockLogger = new Mock<ILogger<EditSRMAReasonOfferedPageModel>>();

		var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

		mockSRMAModelService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
			.ReturnsAsync(srmaModel);

		var pageModel = SetupEditSRMAReasonOfferedPageModel(mockSRMAModelService.Object, mockLogger.Object);
		var routeData = pageModel.RouteData.Values;
		pageModel.CaseId = (int)srmaModel.CaseUrn;
		pageModel.SrmaId = (int)srmaModel.Id;

		// act
		var pageResponse = await pageModel.OnGetAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
		});
	}
		
	[Test]
	public async Task WhenOnGetAsync_AndSrmaIsClosed_RedirectsToClosedPage()
	{
		// arrange 
		var mockSRMAModelService = new Mock<ISRMAService>();
		var mockLogger = new Mock<ILogger<EditSRMAReasonOfferedPageModel>>();

		var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, closedAt:DateTime.Now);

		mockSRMAModelService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
			.ReturnsAsync(srmaModel);

		var pageModel = SetupEditSRMAReasonOfferedPageModel(mockSRMAModelService.Object, mockLogger.Object);
		var routeData = pageModel.RouteData.Values;
		pageModel.CaseId = (int)srmaModel.CaseUrn;
		pageModel.SrmaId = (int)srmaModel.Id;

		// act
		var pageResponse = await pageModel.OnGetAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page?.Url, Is.EqualTo($"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}/closed"));
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		});
	}

	[Test]
	[TestCase(SRMAReasonOffered.OfferLinked)]
	public async Task WhenOnPostAsync_RouteData_RequestForm_Return_To_SRMA_Page(SRMAReasonOffered reason)
	{
		// arrange 
		var mockSRMAModelService = new Mock<ISRMAService>();
		var mockLogger = new Mock<ILogger<EditSRMAReasonOfferedPageModel>>();

		var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

		var pageModel = SetupEditSRMAReasonOfferedPageModel(mockSRMAModelService.Object, mockLogger.Object);
		var routeData = pageModel.RouteData.Values;
		pageModel.CaseId = (int)srmaModel.CaseUrn;
		pageModel.SrmaId = (int)srmaModel.Id;

		pageModel.SRMAReasonOffered = _fixture.Create<RadioButtonsUiComponent>();
		pageModel.SRMAReasonOffered.SelectedId = (int)SRMAReasonOffered.OfferLinked;

		var pageResponse = await pageModel.OnPostAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo($"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}"));
		});
	}

	private EditSRMAReasonOfferedPageModel SetupEditSRMAReasonOfferedPageModel(
		ISRMAService mockSrmaModelService, ILogger<EditSRMAReasonOfferedPageModel> mockLogger, bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

		var caseModel = _fixture.Create<CaseModel>();

		var caseModelService = new Mock<ICaseModelService>();
		caseModelService.Setup(m => m.GetCaseByUrn(It.IsAny<long>())).ReturnsAsync(caseModel);

		return new EditSRMAReasonOfferedPageModel(mockSrmaModelService, caseModelService.Object, mockLogger)
		{
			PageContext = pageContext,
			TempData = tempData,
			Url = new UrlHelper(actionContext),
			MetadataProvider = pageContext.ViewData.ModelMetadata
		};
	}
}