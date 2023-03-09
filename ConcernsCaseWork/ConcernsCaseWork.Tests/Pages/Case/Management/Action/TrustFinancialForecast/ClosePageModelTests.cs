using AutoFixture;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.TrustFinancialForecast;

[Parallelizable(ParallelScope.All)]
public class ClosePageModelTests
{
	private readonly Fixture _fixture = new Fixture();
	
	[Test]
	public async Task WhenOnGetAsync_WhenTrustFinancialForecastIsOpen_ReturnsPageModel()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();

		var getRequest = new GetTrustFinancialForecastByIdRequest{ TrustFinancialForecastId = trustFinancialForecastId, CaseUrn = caseUrn };
		var trustFinancialForecast = CreateOpenTrustFinancialForecastResponse(getRequest.TrustFinancialForecastId, getRequest.CaseUrn);

		mockTrustFinancialForecastService.Setup(fp => fp.GetById(getRequest))
			.ReturnsAsync(trustFinancialForecast);

		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		sut.CaseUrn = caseUrn;

		// act
		var response = await sut.OnGetAsync();
		
		// assert
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());

			Assert.That(sut.TempData["Error.Message"], Is.Null);
		});
	}

	[Test]
	public async Task WhenOnGetAsync_WhenTrustFinancialForecastIsClosed_RedirectsToClosedPage()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();

		var getRequest = new GetTrustFinancialForecastByIdRequest{ TrustFinancialForecastId = trustFinancialForecastId, CaseUrn = caseUrn };		
		var trustFinancialForecast = CreateClosedTrustFinancialForecastResponse(trustFinancialForecastId, caseUrn);

		mockTrustFinancialForecastService.Setup(fp => fp.GetById(getRequest))
			.ReturnsAsync(trustFinancialForecast);

		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		sut.CaseUrn = caseUrn;

		// act
		var response = await sut.OnGetAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<RedirectResult>());
			Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/trustfinancialforecast/{trustFinancialForecastId}/closed"));
			Assert.That(sut.TempData["Error.Message"], Is.Null);
		});
	}
	
	[Test]
	public async Task WhenOnGetAsync_WhenTrustFinancialForecastNotFound_ReturnsError()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();
		
		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		sut.CaseUrn = caseUrn;

		// act
		var response = await sut.OnGetAsync();
		
		// assert
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo("There was an error. Refresh the page or try again later.\n\nIf the problem still continues, email the Record concerns and support for trusts team at regionalservices.rg@education.go.uk"));
		});
	}

	[Test]
	public async Task WhenOnGetAsync_WhenExceptionThrown_ReturnsError()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();
		
		mockTrustFinancialForecastService.Setup(fp => fp.GetById(It.IsAny<GetTrustFinancialForecastByIdRequest>()))
			.ThrowsAsync(new Exception());
		
		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);

		// act
		var response = await sut.OnGetAsync();
		
		// assert
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo("There was an error. Refresh the page or try again later.\n\nIf the problem still continues, email the Record concerns and support for trusts team at regionalservices.rg@education.go.uk"));
		});
	}
	
	[Test]
	public async Task WhenOnPostAsync_ModelStateIsInvalid_ReturnsPageWithModelStateErrors()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();
		
		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);

		var keyName = "testkey";
		var errorMsg = "some model validation error";
		
		sut.ModelState.AddModelError(keyName, errorMsg);
		
		// act
		var pageResponse = await sut.OnPostAsync();
		
		await sut.OnPostAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(sut.ModelState.IsValid, Is.False);
			Assert.That(sut.ModelState.Keys.Count(), Is.EqualTo(1));
			Assert.That(sut.ModelState.First().Key, Is.EqualTo(keyName));
			Assert.That(sut.ModelState.First().Value?.Errors.Single().ErrorMessage, Is.EqualTo(errorMsg));

			Assert.That(page, Is.Not.Null);
			Assert.That(sut.TempData, Is.Empty);
		});
			
		mockTrustFinancialForecastService.Verify(f => f.Close(It.IsAny<CloseTrustFinancialForecastRequest>()), Times.Never);
	}
	
	[Test]
	public async Task WhenOnPostAsync_AllValuesSet_ModelStateIsValid_Redirects()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();
		
		var trustFinancialForecastId = _fixture.Create<int>();
		var urn = _fixture.Create<int>();
		var notes = _fixture.Create<string>();
		
		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		
		sut.CaseUrn = urn;
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		
		sut.Notes = new TextAreaUiComponent("","",""){ Text = new ValidateableString(){StringContents = notes}};
		
		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(sut.TempData, Is.Empty);
		});
		
		mockTrustFinancialForecastService
			.Verify(f => f.Close(It.Is<CloseTrustFinancialForecastRequest>(x =>
				x.CaseUrn == urn &&
				x.Notes == notes &&
				x.TrustFinancialForecastId == trustFinancialForecastId)), Times.Once);
	}
	
	[Test]
	public async Task WhenOnPostAsync_NoNotesSet_ModelStateIsValid_Redirects()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();
		
		var trustFinancialForecastId = _fixture.Create<int>();
		var urn = _fixture.Create<int>();
		
		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		
		sut.CaseUrn = urn;
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		
		// act
		var pageResponse = await sut.OnPostAsync();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(sut.TempData, Is.Empty);
		});
		
		mockTrustFinancialForecastService
			.Verify(f => f.Close(It.Is<CloseTrustFinancialForecastRequest>(x =>
				x.CaseUrn == urn &&
				x.TrustFinancialForecastId == trustFinancialForecastId &&
				string.IsNullOrEmpty(x.Notes))), Times.Once);
	}
	
	[Test]
	public async Task WhenOnPostAsync_WhenExceptionThrown_ReturnsError()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<ClosePageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();
		
		mockTrustFinancialForecastService.Setup(fp => fp.Close(It.IsAny<CloseTrustFinancialForecastRequest>()))
			.ThrowsAsync(new Exception());
		
		var sut = SetupClosePageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		sut.CaseUrn = caseUrn;

		// act
		var response = await sut.OnPostAsync();
		
		// assert
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		});
	}
	
	private TrustFinancialForecastResponse CreateOpenTrustFinancialForecastResponse(int id, long caseUrn)
		=> _fixture.Build<TrustFinancialForecastResponse>()
			.With(x => x.TrustFinancialForecastId, id)
			.With(x => x.CaseUrn, caseUrn)
			.Without(x => x.ClosedAt)
			.Create();
	
	private TrustFinancialForecastResponse CreateClosedTrustFinancialForecastResponse(int id, long caseUrn)
		=> _fixture.Build<TrustFinancialForecastResponse>()
			.With(x => x.TrustFinancialForecastId, id)
			.With(x => x.CaseUrn, caseUrn)
			.Create();
	
	private static ClosePageModel SetupClosePageModel(
		ITrustFinancialForecastService mockTrustFinancialForecastService,
		ILogger<ClosePageModel> mockLogger,
		bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

		var result = new ClosePageModel(mockTrustFinancialForecastService, mockLogger)
		{
			PageContext = pageContext,
			TempData = tempData,
			Url = new UrlHelper(actionContext),
			MetadataProvider = pageContext.ViewData.ModelMetadata
		};

		return result;
	}
}