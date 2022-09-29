using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
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
using Service.Redis.Base;
using Service.Redis.Models;
using Service.Redis.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case
{
	[Parallelizable(ParallelScope.All)]
	public class DetailsPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			
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
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			var createCaseModel = pageModel.CreateCaseModel;
			var trustDetailsModel = pageModel.TrustDetailsModel;
			var createRecordsModel = pageModel.CreateRecordsModel;

			// assert
			Assert.IsAssignableFrom<CreateCaseModel>(createCaseModel);
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsModel);
			Assert.IsAssignableFrom<List<CreateRecordModel>>(createRecordsModel);
			
			Assert.That(createCaseModel, Is.Not.Null);
			Assert.That(createCaseModel.Issue, Is.EqualTo(expectedCreateCaseModel.Issue));
			Assert.That(createCaseModel.StatusId, Is.EqualTo(expectedCreateCaseModel.StatusId));
			Assert.That(createCaseModel.CaseAim, Is.EqualTo(expectedCreateCaseModel.CaseAim));
			Assert.That(createCaseModel.ClosedAt, Is.EqualTo(expectedCreateCaseModel.ClosedAt));
			Assert.That(createCaseModel.CreatedAt, Is.EqualTo(expectedCreateCaseModel.CreatedAt));
			Assert.That(createCaseModel.CreatedBy, Is.EqualTo(expectedCreateCaseModel.CreatedBy));
			Assert.That(createCaseModel.CrmEnquiry, Is.EqualTo(expectedCreateCaseModel.CrmEnquiry));
			Assert.That(createCaseModel.CurrentStatus, Is.EqualTo(expectedCreateCaseModel.CurrentStatus));
			Assert.That(createCaseModel.DeEscalation, Is.EqualTo(expectedCreateCaseModel.DeEscalation));
			Assert.That(createCaseModel.NextSteps, Is.EqualTo(expectedCreateCaseModel.NextSteps));
			Assert.That(createCaseModel.RagRating, Is.EqualTo(expectedCreateCaseModel.RagRating));
			Assert.That(createCaseModel.ReviewAt, Is.EqualTo(expectedCreateCaseModel.ReviewAt));
			Assert.That(createCaseModel.UpdatedAt, Is.EqualTo(expectedCreateCaseModel.UpdatedAt));
			Assert.That(createCaseModel.DeEscalationPoint, Is.EqualTo(expectedCreateCaseModel.DeEscalationPoint));
			Assert.That(createCaseModel.DirectionOfTravel, Is.EqualTo(expectedCreateCaseModel.DirectionOfTravel));
			Assert.That(createCaseModel.ReasonAtReview, Is.EqualTo(expectedCreateCaseModel.ReasonAtReview));
			Assert.That(createCaseModel.TrustUkPrn, Is.EqualTo(expectedCreateCaseModel.TrustUkPrn));

			Assert.That(trustDetailsModel, Is.Not.Null);
			Assert.That(trustDetailsModel.TrustNameCounty, Is.Not.Null);
			Assert.That(trustDetailsModel.DisplayAddress, Is.Not.Null);
			
			Assert.That(createRecordsModel, Is.Not.Null);
			Assert.That(createRecordsModel.Count, Is.EqualTo(expectedCreateCaseModel.CreateRecordsModel.Count));
			
			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("DetailsPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
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
			
			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>()))
				.ReturnsAsync(new UserState("testing"));
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);
			
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
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("DetailsPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsErrorLoadingPage_MissingRedisCaseStateData()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();

			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("DetailsPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("DetailsPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnPost_RedirectToManagementPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockCaseModelService.Setup(c => c.PostCase(It.IsAny<CreateCaseModel>())).ReturnsAsync(1);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, 
				mockUserStateCachedService.Object, 
				mockLogger.Object, true);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "issue", new StringValues("issue") },
					{ "current-status", new StringValues("current-status") },
					{ "next-steps", new StringValues("next-steps") },
					{ "case-aim", new StringValues("case-aim") },
					{ "de-escalation-point", new StringValues("de-escalation-point") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectToPageResult>());
			var page = pageResponse as RedirectToPageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.PageName, Is.EqualTo("management/index"));
		}
		
		[Test]
		public async Task WhenOnPost_MissingFormParameters_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();

			var expectedTrustByUkprn = TrustFactory.BuildTrustDetailsModel();
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockCaseModelService.Setup(c => c.PostCase(It.IsAny<CreateCaseModel>())).ReturnsAsync(1);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedTrustByUkprn);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, mockUserStateCachedService.Object, 
				mockLogger.Object, true);
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			
			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}
		
		[Test]
		public async Task WhenOnPost_EmptyFormParameters_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockUserStateCachedService = new Mock<IUserStateCachedService>();
			var mockTrustModelService = new Mock<ITrustModelService>();

			var expectedTrustByUkprn = TrustFactory.BuildTrustDetailsModel();
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState("testing") { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockUserStateCachedService.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(userState);
			mockCaseModelService.Setup(c => c.PostCase(It.IsAny<CreateCaseModel>())).ReturnsAsync(1);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expectedTrustByUkprn);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockTrustModelService.Object, mockUserStateCachedService.Object, 
				mockLogger.Object, true);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "issue", new StringValues("") },
					{ "current-status", new StringValues("") },
					{ "next-steps", new StringValues("") },
					{ "case-aim", new StringValues("") },
					{ "de-escalation-point", new StringValues("") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			
			mockUserStateCachedService.Verify(c => c.GetData(It.IsAny<string>()), Times.Once);
			mockTrustModelService.Verify(t => t.GetTrustByUkPrn(It.IsAny<string>()), Times.Once);
		}
		
		private static DetailsPageModel SetupDetailsModel(
			ICaseModelService mockCaseModelService, 
			ITrustModelService mockTrustModelService,
			IUserStateCachedService mockUserStateCachedService, 
			ILogger<DetailsPageModel> mockLogger, 
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new DetailsPageModel(mockCaseModelService, mockTrustModelService, mockUserStateCachedService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}