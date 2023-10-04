using AutoFixture;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Action;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
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
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action
{
	[Parallelizable(ParallelScope.All)]
	public class IndexPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockRecordsModelService = new Mock<IRecordModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var recordsModel = RecordFactory.BuildListRecordModel();

			mockRecordsModelService.Setup(m => m.GetRecordsModelByCaseUrn(It.IsAny<long>())).ReturnsAsync(recordsModel);

			var pageModel = SetupIndexPageModel(mockRecordsModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			var pageResult = await pageModel.OnGetAsync();

			pageResult.Should().BeAssignableTo<PageResult>();
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_ActionIs_SRMA_ReturnsToAddSRMAPage()
		{
			// arrange
			var mockRecordsModelService = new Mock<IRecordModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockRecordsModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.CaseActionEnum = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.CaseActionEnum.SelectedId = (int)CaseActionEnum.Srma;

			var expectedRedirectLink = "srma/add";

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
			var page = pageResponse as RedirectToPageResult;

			Assert.IsEmpty(pageModel.TempData);
			Assert.That(page, Is.Not.Null);
			Assert.That(page.PageName, Is.EqualTo(expectedRedirectLink));
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_ActionIs_Unknown_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockRecordsModelService = new Mock<IRecordModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockRecordsModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object
				, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			pageModel.CaseActionEnum = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.CaseActionEnum.SelectedId = -1;

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_ActionIs_SRMA_SRMA_Already_Exists_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockRecordsModelService = new Mock<IRecordModelService>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockNtiUnderConsiderationModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<IndexPageModel>>();

			var pageModel = SetupIndexPageModel(mockRecordsModelService.Object, mockSrmaService.Object, mockFinancialPlanModelService.Object, mockNtiUnderConsiderationModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);


			pageModel.CaseActionEnum = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.CaseActionEnum.SelectedId = (int)CaseActionEnum.Srma;

			var srmaModelList = SrmaFactory.BuildListSrmaModel(SRMAStatus.TrustConsidering);

			mockSrmaService.Setup(s => s.GetSRMAsForCase(It.IsAny<long>())).ReturnsAsync(srmaModelList);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);

			var errors = pageModel.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

			errors.Should().Contain("There is already an open SRMA action linked to this case. Please resolve that before opening another one.");
		}

		private static IndexPageModel SetupIndexPageModel(
			IRecordModelService recordModelService,
			ISRMAService mockSrmaService,
			IFinancialPlanModelService mockFinancialPlanModelService,
			INtiUnderConsiderationModelService ntiUnderConsiderationModelService,
			ILogger<IndexPageModel> mockLogger,
			INtiWarningLetterModelService ntiWarningLetterModelService = null,
			INtiModelService ntiModelService = null,
			ITrustFinancialForecastService trustFinancialForecastService = null,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new IndexPageModel(recordModelService, mockSrmaService, mockFinancialPlanModelService, ntiUnderConsiderationModelService
				, ntiWarningLetterModelService ?? Mock.Of<INtiWarningLetterModelService>()
				, ntiModelService ?? Mock.Of<INtiModelService>()
				, trustFinancialForecastService ?? Mock.Of<ITrustFinancialForecastService>()
				, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}