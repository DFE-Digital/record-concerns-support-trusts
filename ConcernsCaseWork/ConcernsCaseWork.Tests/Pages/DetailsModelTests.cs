using ConcernsCaseWork.Pages.Case;
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
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class DetailsModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockCasesCachedService = new Mock<ICachedService>();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };
			
			mockCasesCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>()))
				.ReturnsAsync(userState);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			var createCaseModel = pageModel.CreateCaseModel;

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			Assert.IsAssignableFrom<CreateCaseModel>(createCaseModel);
			
			Assert.That(createCaseModel, Is.Not.Null);
			Assert.That(createCaseModel.Description, Is.EqualTo(expected.Description));
			Assert.That(createCaseModel.Issue, Is.EqualTo(expected.Issue));
			Assert.That(createCaseModel.Status, Is.EqualTo(expected.Status));
			Assert.That(createCaseModel.Urn, Is.EqualTo(expected.Urn));
			Assert.That(createCaseModel.CaseAim, Is.EqualTo(expected.CaseAim));
			Assert.That(createCaseModel.ClosedAt, Is.EqualTo(expected.ClosedAt));
			Assert.That(createCaseModel.CreatedAt, Is.EqualTo(expected.CreatedAt));
			Assert.That(createCaseModel.CreatedBy, Is.EqualTo(expected.CreatedBy));
			Assert.That(createCaseModel.CrmEnquiry, Is.EqualTo(expected.CrmEnquiry));
			Assert.That(createCaseModel.CurrentStatus, Is.EqualTo(expected.CurrentStatus));
			Assert.That(createCaseModel.DeEscalation, Is.EqualTo(expected.DeEscalation));
			Assert.That(createCaseModel.NextSteps, Is.EqualTo(expected.NextSteps));
			Assert.That(createCaseModel.RagRating, Is.EqualTo(expected.RagRating));
			Assert.That(createCaseModel.RecordType, Is.EqualTo(expected.RecordType));
			Assert.That(createCaseModel.ReviewAt, Is.EqualTo(expected.ReviewAt));
			Assert.That(createCaseModel.TrustName, Is.EqualTo(expected.TrustName));
			Assert.That(createCaseModel.UpdatedAt, Is.EqualTo(expected.UpdatedAt));
			Assert.That(createCaseModel.DeEscalationPoint, Is.EqualTo(expected.DeEscalationPoint));
			Assert.That(createCaseModel.DirectionOfTravel, Is.EqualTo(expected.DirectionOfTravel));
			Assert.That(createCaseModel.ReasonAtReview, Is.EqualTo(expected.ReasonAtReview));
			Assert.That(createCaseModel.RecordSubType, Is.EqualTo(expected.RecordSubType));
			Assert.That(createCaseModel.TrustUkPrn, Is.EqualTo(expected.TrustUkPrn));

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("DetailsPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_ReturnsErrorLoadingPage_MissingRedisCaseStateData()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockCasesCachedService = new Mock<ICachedService>();

			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockCasesCachedService.Object, mockLogger.Object, true);
			
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
			var mockCachedService = new Mock<ICachedService>();

			var caseModel = CaseFactory.BuildCaseModel();
			
			var expected = CaseFactory.BuildCreateCaseModel();
			var userState = new UserState { TrustUkPrn = "trust-ukprn", CreateCaseModel = expected };

			mockCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(userState);
			mockCaseModelService.Setup(c => c.PostCase(It.IsAny<CreateCaseModel>())).ReturnsAsync(caseModel);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockCachedService.Object, mockLogger.Object, true);
			
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
			Assert.That(page.PageName, Is.EqualTo("Management"));
		}
		
		[Test]
		public async Task WhenOnPost_MissingFormParameters_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockCasesCachedService = new Mock<ICachedService>();

			var caseModel = CaseFactory.BuildCaseModel();
			
			mockCaseModelService.Setup(c => c.PostCase(It.IsAny<CreateCaseModel>())).ReturnsAsync(caseModel);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockCasesCachedService.Object, mockLogger.Object, true);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "subType", new StringValues("subType") },
					{ "riskRating", new StringValues("riskRating") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo("details"));
		}

		[Test]
		public async Task WhenOnPost_ReturnPageWhenCaseTypeInput_IsEmptyOrNull()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockCasesCachedService = new Mock<ICachedService>();

			var pageModel = SetupDetailsModel(mockCaseModelService.Object, 
				mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo("details"));
		}
		
		private static DetailsPageModel SetupDetailsModel(
			ICaseModelService mockCaseModelService, ICachedService mockCachedService, 
			ILogger<DetailsPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new DetailsPageModel(mockCaseModelService, mockCachedService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}