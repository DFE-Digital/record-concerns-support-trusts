﻿using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management
{
	[Parallelizable(ParallelScope.All)]
	public class EditRiskRatingPageModelTests
	{
		[Test]
		public async Task WhenOnGet_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var recordModel = RecordFactory.BuildRecordModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockRatingModelService.Object, mockLogger.Object);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1); 
			
			// act
			var pageResponse = await pageModel.OnGet();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.CaseModel.PreviousUrl, Is.EqualTo("https://returnto/thispage"));
			
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
		}

		[Test]
		public async Task WhenOnGet_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockRatingModelService.Object, mockLogger.Object);
			pageModel.Request.Headers.Add("Referer", "https://returnto/thispage");

			// act
			var pageResponse = await pageModel.OnGet();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.CaseModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
			
			mockCaseModelService.Verify(c => 
				c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostEditRiskRating_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockRatingModelService.Object, mockLogger.Object);
			
			// act
			var pageResponse = await pageModel.OnPostEditRiskRating("https://returnto/thispage");
			
			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;
			
			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.CaseModel);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
			mockRatingModelService.Verify(c => c.GetSelectedRatingsModelById(It.IsAny<long>()), Times.Never);
		}

		[Test]
		public async Task WhenOnPostEditRiskRating_RouteData_MissingRequestForm_ReloadPage()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			var caseModel = CaseFactory.BuildCaseModel();
			var ratingsModel = RatingFactory.BuildListRatingModel();

			mockCaseModelService.Setup(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()))
				.ReturnsAsync(caseModel);

			mockRatingModelService.Setup(r => r.GetSelectedRatingsModelById(It.IsAny<long>()))
				.ReturnsAsync(ratingsModel);
			
			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockRatingModelService.Object, mockLogger.Object);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "rating", new StringValues("") }
				});
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("recordId", 1);
			
			// act
			var pageResponse = await pageModel.OnPostEditRiskRating("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.CaseModel, Is.Not.Null);
			Assert.That(pageModel.RatingsModel, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));

			mockCaseModelService.Verify(c => c.GetCaseByUrn(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
			mockRatingModelService.Verify(c => c.GetSelectedRatingsModelById(It.IsAny<long>()), Times.Once);
		}

		[Test]
		public async Task WhenOnPostEditRiskRating_RouteData_RequestForm_ReturnsToPreviousUrl()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockRatingModelService = new Mock<IRatingModelService>();
			var mockRecordModelService = new Mock<IRecordModelService>();
			var mockLogger = new Mock<ILogger<EditRatingPageModel>>();

			mockCaseModelService.Setup(c => c.PatchCaseRating(It.IsAny<PatchCaseModel>()));

			var pageModel = SetupEditRiskRatingPageModel(mockCaseModelService.Object, mockRatingModelService.Object, mockLogger.Object);
			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "rating", new StringValues("123:ragRating") }
				});
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);
			routeData.Add("recordId", 1);
			
			// act
			var pageResponse = await pageModel.OnPostEditRiskRating("https://returnto/thispage");

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			Assert.IsNull(pageModel.CaseModel);
			Assert.IsNull(pageModel.RatingsModel);
			Assert.That(page.Url, Is.EqualTo("https://returnto/thispage"));
		}

		private static EditRatingPageModel SetupEditRiskRatingPageModel(
			ICaseModelService mockCaseModelService, IRatingModelService mockRatingModelService, ILogger<EditRatingPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new EditRatingPageModel(mockCaseModelService, mockRatingModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}