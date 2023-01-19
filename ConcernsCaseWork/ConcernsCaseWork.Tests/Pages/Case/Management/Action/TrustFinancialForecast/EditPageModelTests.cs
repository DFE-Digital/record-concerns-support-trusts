using AutoFixture;
using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
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
public class EditPageModelTests
{
	private readonly Fixture _fixture = new Fixture();
	
	[Test]
	public async Task WhenOnGetAsync_WhenTrustFinancialForecastIsOpen_ReturnsPageModel()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<EditPageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();

		var getRequest = new GetTrustFinancialForecastByIdRequest{ TrustFinancialForecastId = trustFinancialForecastId, CaseUrn = caseUrn };
		var trustFinancialForecast = CreateOpenTrustFinancialForecastResponse(getRequest.TrustFinancialForecastId, getRequest.CaseUrn);

		mockTrustFinancialForecastService.Setup(fp => fp.GetById(getRequest))
			.ReturnsAsync(trustFinancialForecast);

		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
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
		var mockLogger = new Mock<ILogger<EditPageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();

		var getRequest = new GetTrustFinancialForecastByIdRequest{ TrustFinancialForecastId = trustFinancialForecastId, CaseUrn = caseUrn };		
		var trustFinancialForecast = CreateClosedTrustFinancialForecastResponse(trustFinancialForecastId, caseUrn);

		mockTrustFinancialForecastService.Setup(fp => fp.GetById(getRequest))
			.ReturnsAsync(trustFinancialForecast);

		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
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
		var mockLogger = new Mock<ILogger<EditPageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();
		
		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		sut.CaseUrn = caseUrn;

		// act
		var response = await sut.OnGetAsync();
		
		// assert
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		});
	}
	
	[Test]
	public async Task WhenOnGetAsync_WhenExceptionThrown_ReturnsError()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<EditPageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();
		
		mockTrustFinancialForecastService.Setup(fp => fp.GetById(It.IsAny<GetTrustFinancialForecastByIdRequest>()))
			.ThrowsAsync(new Exception());
		
		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		sut.CaseUrn = caseUrn;

		// act
		var response = await sut.OnGetAsync();
		
		// assert
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());
			Assert.That(sut.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		});
	}
	
	[Test]
	public async Task WhenOnPostAsync_ModelStateIsInvalid_ReturnsPageWithModelStateErrors()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<EditPageModel>>();
		
		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);

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
			
		mockTrustFinancialForecastService.Verify(f => f.Update(It.IsAny<UpdateTrustFinancialForecastRequest>()), Times.Never);
	}
	
	[Test]
	public async Task WhenOnPostAsync_AllValuesSet_ModelStateIsValid_Redirects()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<EditPageModel>>();
		
		var trustFinancialForecastId = _fixture.Create<int>();
		var urn = _fixture.Create<int>();
		var srmaOfferedAfterTFF = _fixture.Create<SRMAOfferedAfterTFF>();
		var forecastingToolRanAt = _fixture.Create<ForecastingToolRanAt>();
		var wasTrustResponseSatisfactory = _fixture.Create<WasTrustResponseSatisfactory>();
		var notes = _fixture.Create<string>();
		var sfsoInitialReviewHappenedAt = _fixture.Create<DateTime>().Date;
		var trustRespondedAt = _fixture.Create<DateTime>().Date;
		
		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		
		sut.CaseUrn = urn;
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		
		sut.SRMAOfferedAfterTFF = new RadioButtonsUiComponent("","","") { SelectedId = (int)srmaOfferedAfterTFF };
		sut.ForecastingToolRanAt = new RadioButtonsUiComponent("","","") { SelectedId = (int)forecastingToolRanAt };
		sut.WasTrustResponseSatisfactory = new RadioButtonsUiComponent("","","") { SelectedId = (int)wasTrustResponseSatisfactory };
		sut.Notes = new TextAreaUiComponent("","",""){Contents = notes};
		sut.SFSOInitialReviewHappenedAt = new OptionalDateTimeUiComponent("","",""){ Date = new OptionalDateModel(sfsoInitialReviewHappenedAt)};
		sut.TrustRespondedAt = new OptionalDateTimeUiComponent("","",""){ Date = new OptionalDateModel(trustRespondedAt)};
		
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
			.Verify(f => f.Update(It.Is<UpdateTrustFinancialForecastRequest>(x =>
				x.CaseUrn == urn &&
				x.SRMAOfferedAfterTFF == srmaOfferedAfterTFF &&
				x.ForecastingToolRanAt == forecastingToolRanAt &&
				x.WasTrustResponseSatisfactory == wasTrustResponseSatisfactory &&
				x.Notes == notes &&
				x.SFSOInitialReviewHappenedAt == sfsoInitialReviewHappenedAt &&
				x.TrustRespondedAt == trustRespondedAt &&
				x.TrustFinancialForecastId == trustFinancialForecastId)), Times.Once);
	}
	
	[Test]
	public async Task WhenOnPostAsync_NoValuesSet_ModelStateIsValid_Redirects()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<EditPageModel>>();
		
		var trustFinancialForecastId = _fixture.Create<int>();
		var urn = _fixture.Create<int>();

		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
		
		sut.CaseUrn = urn;
		sut.TrustFinancialForecastId = trustFinancialForecastId;
		sut.SRMAOfferedAfterTFF = new RadioButtonsUiComponent("","","");
		sut.ForecastingToolRanAt = new RadioButtonsUiComponent("","","");
		sut.WasTrustResponseSatisfactory = new RadioButtonsUiComponent("","","");
		sut.Notes = new TextAreaUiComponent("","","");
		sut.SFSOInitialReviewHappenedAt = new OptionalDateTimeUiComponent("","","");
		sut.TrustRespondedAt = new OptionalDateTimeUiComponent("","","");
		
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
			.Verify(f => f.Update(It.Is<UpdateTrustFinancialForecastRequest>(x =>
				x.CaseUrn == urn &&
				x.SRMAOfferedAfterTFF == null &&
				x.ForecastingToolRanAt == null &&
				x.WasTrustResponseSatisfactory == null &&
				x.Notes == null &&
				x.SFSOInitialReviewHappenedAt == null &&
				x.TrustRespondedAt == null &&
				x.TrustFinancialForecastId == trustFinancialForecastId)), Times.Once);
	}
	
	[Test]
	public async Task WhenOnPostAsync_WhenExceptionThrown_ReturnsError()
	{
		// arrange
		var mockTrustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
		var mockLogger = new Mock<ILogger<EditPageModel>>();

		var trustFinancialForecastId = _fixture.Create<int>();
		var caseUrn = _fixture.Create<int>();
		
		mockTrustFinancialForecastService.Setup(fp => fp.Update(It.IsAny<UpdateTrustFinancialForecastRequest>()))
			.ThrowsAsync(new Exception());
		
		var sut = SetupEditPageModel(mockTrustFinancialForecastService.Object, mockLogger.Object);
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
	
	private static EditPageModel SetupEditPageModel(
		ITrustFinancialForecastService mockTrustFinancialForecastService,
		ILogger<EditPageModel> mockLogger,
		bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

		var result = new EditPageModel(mockTrustFinancialForecastService, mockLogger)
		{
			PageContext = pageContext,
			TempData = tempData,
			Url = new UrlHelper(actionContext),
			MetadataProvider = pageContext.ViewData.ModelMetadata
		};

		return result;
	}
}