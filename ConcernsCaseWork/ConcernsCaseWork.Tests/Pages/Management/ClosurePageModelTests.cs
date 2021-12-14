using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Management
{
	[Parallelizable(ParallelScope.All)]
	public class ClosurePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();
			
			var expectedCaseModel = CaseFactory.BuildCaseModel();
			var expectedTrustDetailsModel = TrustFactory.BuildTrustDetailsModel(); 

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(expectedCaseModel);
			mockTrustModelService.Setup(t => t.GetTrustByUkPrn(It.IsAny<string>()))
				.ReturnsAsync(expectedTrustDetailsModel);
			
			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockLogger.Object, true);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			// act
			await pageModel.OnGetAsync();
			var caseModel = pageModel.CaseModel;
			var trustDetailsModel = pageModel.TrustDetailsModel;

			// assert
			Assert.IsAssignableFrom<CaseModel>(caseModel);

			Assert.That(caseModel.Description, Is.EqualTo(expectedCaseModel.Description));
			Assert.That(caseModel.Issue, Is.EqualTo(expectedCaseModel.Issue));
			Assert.That(caseModel.StatusUrn, Is.EqualTo(expectedCaseModel.StatusUrn));
			Assert.That(caseModel.Urn, Is.EqualTo(expectedCaseModel.Urn));
			Assert.That(caseModel.ClosedAt, Is.EqualTo(expectedCaseModel.ClosedAt));
			Assert.That(caseModel.CreatedAt, Is.EqualTo(expectedCaseModel.CreatedAt));
			Assert.That(caseModel.CreatedBy, Is.EqualTo(expectedCaseModel.CreatedBy));
			Assert.That(caseModel.CrmEnquiry, Is.EqualTo(expectedCaseModel.CrmEnquiry));
			Assert.That(caseModel.CurrentStatus, Is.EqualTo(expectedCaseModel.CurrentStatus));
			Assert.That(caseModel.DeEscalation, Is.EqualTo(expectedCaseModel.DeEscalation));
			Assert.That(caseModel.NextSteps, Is.EqualTo(expectedCaseModel.NextSteps));
			Assert.That(caseModel.CaseAim, Is.EqualTo(expectedCaseModel.CaseAim));
			Assert.That(caseModel.DeEscalationPoint, Is.EqualTo(expectedCaseModel.DeEscalationPoint));
			Assert.That(caseModel.ReviewAt, Is.EqualTo(expectedCaseModel.ReviewAt));
			Assert.That(caseModel.StatusName, Is.EqualTo(expectedCaseModel.StatusName));
			Assert.That(caseModel.UpdatedAt, Is.EqualTo(expectedCaseModel.UpdatedAt));
			Assert.That(caseModel.DirectionOfTravel, Is.EqualTo(expectedCaseModel.DirectionOfTravel));
			Assert.That(caseModel.ReasonAtReview, Is.EqualTo(expectedCaseModel.ReasonAtReview));
			Assert.That(caseModel.TrustUkPrn, Is.EqualTo(expectedCaseModel.TrustUkPrn));
			Assert.That(caseModel.PreviousUrl, Is.EqualTo(expectedCaseModel.PreviousUrl));

			Assert.That(trustDetailsModel, Is.Not.Null);
			Assert.That(trustDetailsModel.Establishments, Is.Not.Null);
			Assert.That(trustDetailsModel.Establishments.Count, Is.EqualTo(1));
			Assert.That(trustDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustDetailsModel.IfdData, Is.Not.Null);
			Assert.That(trustDetailsModel.TotalPupils, Is.Not.Null);
			Assert.That(trustDetailsModel.PupilCapacityPercentage, Is.Not.Null);
			Assert.That(trustDetailsModel.TotalPupilCapacity, Is.Not.Null);
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingRoutes_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();
			
			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));

			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostCloseCase_RedirectToHomePage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();
			
			mockCaseModelService.Setup(c => c.PatchClosure(It.IsAny<PatchCaseModel>()));
			
			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockLogger.Object, true);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("case-outcomes") }
				});
			
			// act
			var actionResult = await pageModel.OnPostCloseCase();
			var redirectResult = actionResult as RedirectResult;

			// assert
			Assert.That(actionResult, Is.AssignableFrom<RedirectResult>());
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("/"));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
			
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		}		
		
		[Test]
		public async Task WhenOnPostCloseCase_MissingRoutes_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();
			
			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockLogger.Object, true);
			
			// act
			var actionResult = await pageModel.OnPostCloseCase();
			var redirectResult = actionResult as RedirectResult;
			
			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(actionResult, Is.AssignableFrom<RedirectResult>());
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("closure"));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnPostCloseCase_MissingFormValues_ThrowsException()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockLogger = new Mock<ILogger<ClosurePageModel>>();
			
			var pageModel = SetupClosurePageModel(mockCaseModelService.Object, mockTrustModelService.Object, mockLogger.Object, true);
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "case-outcomes", new StringValues("") }
				});
			
			// act
			var actionResult = await pageModel.OnPostCloseCase();
			var redirectResult = actionResult as RedirectResult;

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
			Assert.That(actionResult, Is.AssignableFrom<RedirectResult>());
			Assert.That(redirectResult, Is.Not.Null);
			Assert.That(redirectResult.Url, Is.EqualTo("closure"));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ClosurePageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}		
		
		private static ClosurePageModel SetupClosurePageModel(
			ICaseModelService mockCaseModelService, ITrustModelService mockTrustModelService, ILogger<ClosurePageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new ClosurePageModel(mockCaseModelService, mockTrustModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}