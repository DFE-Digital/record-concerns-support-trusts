using ConcernsCaseWork.Models;
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class EditNextStepsPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditNextStepsPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupEditNextStepsPageModel(mockCaseModelService.Object, mockLogger.Object);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
		}

		[Test]
		public async Task WhenOnGetAsync_RouteData_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditNextStepsPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			
			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);
			var pageModel = SetupEditNextStepsPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			Assert.That(pageModel.CaseModel.Description, Is.EqualTo(caseModel.Description));
			Assert.That(pageModel.CaseModel.Issue, Is.EqualTo(caseModel.Issue));
			Assert.That(pageModel.CaseModel.StatusUrn, Is.EqualTo(caseModel.StatusUrn));
			Assert.That(pageModel.CaseModel.Urn, Is.EqualTo(caseModel.Urn));
			Assert.That(pageModel.CaseModel.CaseAim, Is.EqualTo(caseModel.CaseAim));
			Assert.That(pageModel.CaseModel.CaseType, Is.EqualTo(caseModel.CaseType));
			Assert.That(pageModel.CaseModel.ClosedAt, Is.EqualTo(caseModel.ClosedAt));
			Assert.That(pageModel.CaseModel.CreatedAt, Is.EqualTo(caseModel.CreatedAt));
			Assert.That(pageModel.CaseModel.CreatedBy, Is.EqualTo(caseModel.CreatedBy));
			Assert.That(pageModel.CaseModel.CrmEnquiry, Is.EqualTo(caseModel.CrmEnquiry));
			Assert.That(pageModel.CaseModel.CurrentStatus, Is.EqualTo(caseModel.CurrentStatus));
			Assert.That(pageModel.CaseModel.DeEscalation, Is.EqualTo(caseModel.DeEscalation));
			Assert.That(pageModel.CaseModel.NextSteps, Is.EqualTo(caseModel.NextSteps));
			//Assert.That(pageModel.CaseModel.RagRating, Is.EqualTo(caseModel.RagRating)); //TODOEA
			Assert.That(pageModel.CaseModel.ReviewAt, Is.EqualTo(caseModel.ReviewAt));
			Assert.That(pageModel.CaseModel.StatusName, Is.EqualTo(caseModel.StatusName));
			Assert.That(pageModel.CaseModel.TypesDictionary, Is.EqualTo(caseModel.TypesDictionary));
			Assert.That(pageModel.CaseModel.UpdatedAt, Is.EqualTo(caseModel.UpdatedAt));
			Assert.That(pageModel.CaseModel.CaseSubType, Is.EqualTo(caseModel.CaseSubType));
			Assert.That(pageModel.CaseModel.CaseTypeDescription, Is.EqualTo(caseModel.CaseTypeDescription));
			Assert.That(pageModel.CaseModel.DeEscalationPoint, Is.EqualTo(caseModel.DeEscalationPoint));
			Assert.That(pageModel.CaseModel.DirectionOfTravel, Is.EqualTo(caseModel.DirectionOfTravel));
			//Assert.That(pageModel.CaseModel.RagRatingCss, Is.EqualTo(caseModel.RagRatingCss)); //TODOEA
			//Assert.That(pageModel.CaseModel.RagRatingName, Is.EqualTo(caseModel.RagRatingName)); //TODOEA
			Assert.That(pageModel.CaseModel.ReasonAtReview, Is.EqualTo(caseModel.ReasonAtReview));
			Assert.That(pageModel.CaseModel.TrustUkPrn, Is.EqualTo(caseModel.TrustUkPrn));
			
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
		}

		[Test]
		public void WhenOnPostEditNextSteps_MissingRouteData_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditNextStepsPageModel>>();

			var pageModel = SetupEditNextStepsPageModel(mockCaseModelService.Object, mockLogger.Object);

			// assert/act
			Assert.ThrowsAsync<Exception>(() => pageModel.OnPostEditNextSteps("https://returnto/thispage"));
			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostEditNextSteps_RouteData_RequestForm_ReturnsToPreviousUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<EditNextStepsPageModel>>();

			mockCaseModelService.Setup(c => c.PatchIssue(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditNextStepsPageModel(mockCaseModelService.Object, mockLogger.Object);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("id", 1);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "de-escalation-point", new StringValues("de-escalation-point") }
				});
			
			// act
			var pageResponse = await pageModel.OnPostEditNextSteps("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Null);
			Assert.That(page.Url, Is.EqualTo("https://returnto/thispage"));
		}
		
		private static EditNextStepsPageModel SetupEditNextStepsPageModel(
			ICaseModelService mockCaseModelService, ILogger<EditNextStepsPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new EditNextStepsPageModel(mockCaseModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}