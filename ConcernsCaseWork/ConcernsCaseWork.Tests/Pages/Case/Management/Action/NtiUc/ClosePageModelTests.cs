﻿using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiUc
{
	[Parallelizable(ParallelScope.All)]
	public class ClosePageModelTests
	{
		[Test]
		[TestCase("1", "")]
		[TestCase("", "2")]
		[TestCase("", "")]
		public async Task WhenOnGetAsync_EmptyRouteData_ThrowsException_ReturnPage(string urn, string ntiUcId)
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", urn);
			routeData.Add("ntiUCId", ntiUcId);

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
			Assert.That(pageModel.NtiModel, Is.Null);
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingRouteData_ThrowsException_ReturnPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
			Assert.That(pageModel.NtiModel, Is.Null);
			
		}
		
		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
			Assert.That(pageModel.NtiModel, Is.Null);
			
		}
		
		[Test]
		[TestCase("1", "")]
		[TestCase("", "2")]
		[TestCase("", "")]
		public async Task WhenOnPostAsync_EmptyRouteData_ThrowsException_ReturnPage(string urn, string ntiUcId)
		{
			// arrange
			var mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", urn);
			routeData.Add("ntiUCId", ntiUcId);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());

			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
			Assert.That(pageModel.NtiModel, Is.Null);
			
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			Mock<INtiUnderConsiderationModelService> mockNtiModelService = new Mock<INtiUnderConsiderationModelService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 3;
			var ntiId = 4;

			var pageModel = SetupClosePageModel(mockNtiModelService, mockLogger);
			var ntiModel = SetupOpenNtiUnderConsiderationModel(ntiId, caseUrn);
				
			mockNtiModelService
				.Setup(fp => fp.GetNtiUnderConsideration(ntiId))
				.ReturnsAsync(ntiModel);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;
			
			// act
			var pageResponse = await pageModel.OnGetAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Empty);
			
			Assert.That(pageModel.NtiModel, Is.EqualTo(ntiModel));
		}
		
		

		private static ClosePageModel SetupClosePageModel(
			Mock<INtiUnderConsiderationModelService> mockNtiModelService,
			Mock<ILogger<ClosePageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ClosePageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private static NtiUnderConsiderationModel SetupOpenNtiUnderConsiderationModel(int id, long caseUrn)
			=> new() { Id = id, CaseUrn = caseUrn };
		
		private static NtiUnderConsiderationModel SetupClosedNtiUnderConsiderationModel(int id, long caseUrn)
			=> new() { Id = id, CaseUrn = caseUrn, ClosedAt = DateTime.Now};
	}

}