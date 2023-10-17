using AutoFixture;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class DetailsPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTrustService = new Mock<ITrustService>();
			
			var expectedCreateCaseModel = CaseFactory.BuildCreateCaseModel();
			var expectedTrustDetailsModel = TrustFactory.BuildTrustDetailsModel();
			expectedCreateCaseModel.CreateRecordsModel = RecordFactory.BuildListCreateRecordModel();
			
			var userState = new UserState("testing")
			{
				TrustUkPrn = "trust-ukprn", 
				CreateCaseModel = expectedCreateCaseModel
			};

			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(expectedTrustDetailsModel);
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>()))
				.ReturnsAsync(userState);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object,
				mockTrustService.Object,
				true);
			
			// act
			await pageModel.OnGetAsync();
			
			var createCaseModel = pageModel.CreateCaseModel;
			var trustDetailsModel = pageModel.TrustDetailsModel;
			var createRecordsModel = pageModel.CreateRecordsModel;

			// assert
			Assert.IsAssignableFrom<CreateCaseModel>(createCaseModel);
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsModel);
			Assert.IsAssignableFrom<List<CreateRecordModel>>(createRecordsModel);

			Assert.That(trustDetailsModel, Is.Not.Null);
			Assert.That(trustDetailsModel.TrustNameCounty, Is.Not.Null);
			Assert.That(trustDetailsModel.DisplayAddress, Is.Not.Null);
			
			Assert.That(createRecordsModel, Is.Not.Null);
			Assert.That(createRecordsModel.Count, Is.EqualTo(expectedCreateCaseModel.CreateRecordsModel.Count));
			
			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}
		
		[Test]
		public async Task WhenOnGetAsync_Missing_TrustUkprn_Returns_Page()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTrustService = new Mock<ITrustService>();
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>()))
				.ReturnsAsync(new UserState("testing"));
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object,
				mockTrustService.Object,
				true);
			
			// act
			await pageModel.OnGetAsync();
			
			var createCaseModel = pageModel.CreateCaseModel;
			var trustDetailsModel = pageModel.TrustDetailsModel;
			var createRecordsModel = pageModel.CreateRecordsModel;

			// assert
			Assert.That(createCaseModel, Is.Null);
			Assert.That(trustDetailsModel, Is.Null);
			Assert.That(createRecordsModel, Is.Null);
			
			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Never);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsErrorLoadingPage_MissingRedisCaseStateData()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTrustService = new Mock<ITrustService>();
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, 
				mockTrustService.Object,
				true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenOnPost_RedirectToManagementPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockTrustService = new Mock<ITrustService>();
			var mockTrust = new Mock<TrustDetailsDto>();
			var trustUkPrn = "trust-ukprn";
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = trustUkPrn, CreateCaseModel = expected };

			mockTrust.Setup(x => x.GiasData.UkPrn).Returns(trustUkPrn);
			mockTrust.Setup(x => x.GiasData.CompaniesHouseNumber).Returns("12345678");
			mockTrustService.Setup(x => x.GetTrustByUkPrn(trustUkPrn)).ReturnsAsync(mockTrust.Object);
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockCaseModelService.Setup(c => c.PostCase(It.IsAny<CreateCaseModel>())).ReturnsAsync(1);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, 
				mockTrustService.Object,	
				true);

			pageModel.Issue = _fixture.Create<TextAreaUiComponent>();
			pageModel.CaseAim = _fixture.Create<TextAreaUiComponent>();
			pageModel.NextSteps = _fixture.Create<TextAreaUiComponent>();
			pageModel.CaseHistory = _fixture.Create<TextAreaUiComponent>();
			pageModel.CurrentStatus = _fixture.Create<TextAreaUiComponent>();
			pageModel.DeEscalationPoint = _fixture.Create<TextAreaUiComponent>();

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
			var page = pageResponse as RedirectToPageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.PageName, Is.EqualTo("management/index"));
		}
		
		private static DetailsPageModel SetupDetailsModel(
			ICaseModelService mockCaseModelService, 
			ITrustModelService mockTrustModelService,
			IUserStateCachedService mockUserStateCachedService, 
			ILogger<DetailsPageModel> mockLogger, 
			ITrustService mockTrustService,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			var mockClaimsPrincipalHelper = new Mock<IClaimsPrincipalHelper>();
			mockClaimsPrincipalHelper.Setup(x => x.GetPrincipalName(It.IsAny<ClaimsPrincipal>())).Returns("Tester");

			return new DetailsPageModel(
				mockCaseModelService, 
				mockTrustModelService, 
				mockUserStateCachedService, 
				mockLogger,MockTelemetry.CreateMockTelemetryClient(), 
				mockTrustService, 
				mockClaimsPrincipalHelper.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}