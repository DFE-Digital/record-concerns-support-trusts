using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA.Edit;

[Parallelizable(ParallelScope.All)]
public class EditSRMAReasonOfferedPageModelTests
{
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
		routeData.Add("caseUrn", srmaModel.CaseUrn); 
		routeData.Add("srmaId", srmaModel.Id); 

		// act
		var pageResponse = await pageModel.OnGetAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.SRMA, Is.Not.Null);
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
		routeData.Add("caseUrn", srmaModel.CaseUrn); 
		routeData.Add("srmaId", srmaModel.Id); 

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
	public async Task WhenOnGetAsync_MissingRouteData_ReturnsPageWithValidationErrors()
	{
		// arrange 
		var mockSRMAModelService = new Mock<ISRMAService>();
		var mockLogger = new Mock<ILogger<EditSRMAReasonOfferedPageModel>>();

		var pageModel = SetupEditSRMAReasonOfferedPageModel(mockSRMAModelService.Object, mockLogger.Object);

		// act 
		var pageResponse = await pageModel.OnGetAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMA);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			Assert.That(pageModel.TempData["SRMA.Message"], Is.TypeOf<List<string>>());

			var validationErrors = ((List<string>)pageModel.TempData["SRMA.Message"]);
			Assert.That(validationErrors.Contains("Invalid case Id"));
			Assert.That(validationErrors.Contains("SRMA Id not found"));
			Assert.That(validationErrors.Count, Is.EqualTo(2));

			mockSRMAModelService.Verify(s =>
				s.GetSRMAById(It.IsAny<long>()), Times.Never);
		});
	}

	[Test]
	public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
	{
		// arrange 
		var mockSRMAModelService = new Mock<ISRMAService>();
		var mockLogger = new Mock<ILogger<EditSRMAReasonOfferedPageModel>>();

		var pageModel = SetupEditSRMAReasonOfferedPageModel(mockSRMAModelService.Object, mockLogger.Object);

		// act 
		var pageResponse = await pageModel.OnPostAsync("");

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.SRMA);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"],
				Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockSRMAModelService.Verify(s =>
				s.GetSRMAById(It.IsAny<long>()), Times.Never);
		});
	}

	[Test]
	[TestCase(SRMAReasonOffered.OfferLinked)]
	[TestCase(SRMAReasonOffered.RegionsGroupIntervention)]
	[TestCase(SRMAReasonOffered.SchoolsFinancialSupportAndOversight)]
	public async Task WhenOnPostAsync_RouteData_RequestForm_Return_To_SRMA_Page(SRMAReasonOffered reason)
	{
		// arrange 
		var mockSRMAModelService = new Mock<ISRMAService>();
		var mockLogger = new Mock<ILogger<EditSRMAReasonOfferedPageModel>>();

		var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

		var pageModel = SetupEditSRMAReasonOfferedPageModel(mockSRMAModelService.Object, mockLogger.Object);
		var routeData = pageModel.RouteData.Values;
		routeData.Add("caseUrn", srmaModel.CaseUrn);
		routeData.Add("srmaId", srmaModel.Id);

		pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
				{ "reason", new StringValues(reason.ToString()) }
			});

		var pageResponse = await pageModel.OnPostAsync(""); // This url parameter doesn't look to be used?

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo($"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}"));
		});
	}

	private static EditSRMAReasonOfferedPageModel SetupEditSRMAReasonOfferedPageModel(
		ISRMAService mockSrmaModelService, ILogger<EditSRMAReasonOfferedPageModel> mockLogger, bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

		return new EditSRMAReasonOfferedPageModel(mockSrmaModelService, mockLogger)
		{
			PageContext = pageContext,
			TempData = tempData,
			Url = new UrlHelper(actionContext),
			MetadataProvider = pageContext.ViewData.ModelMetadata
		};

	}
}