﻿using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class AddConditionsTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockConditionsService = new Mock<INtiConditionsService>();
			var mockLogger = new Mock<ILogger<AddConditionsPageModel>>();

			var pageModel = SetupAddConditionsPageModel(mockNtiModelService, mockConditionsService, mockLogger);
			pageModel.ContinuationId = Guid.NewGuid().ToString();

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData[ErrorConstants.ErrorMessageKey], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenOnGetAsync_PopulatesPageModel()
		{
			// arrange
			var caseUrn = 191L;

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockConditionsService = new Mock<INtiConditionsService>();
			var mockLogger = new Mock<ILogger<AddConditionsPageModel>>();

			var continuationId = Guid.NewGuid().ToString();

			mockNtiModelService.Setup(svc => svc.GetNtiAsync(continuationId)).ReturnsAsync(new NtiModel
			{
				Id = 1,
				CaseUrn = caseUrn
			});

			var pageModel = SetupAddConditionsPageModel(mockNtiModelService, mockConditionsService, mockLogger);
			pageModel.ContinuationId = continuationId;
			pageModel.CaseUrn = caseUrn;

			var routeData = pageModel.RouteData.Values;

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel, Is.Not.Null);
			Assert.That(pageModel.CaseUrn, Is.EqualTo(caseUrn));
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var caseUrn = 191L;

			var mockNtiModelService = new Mock<INtiModelService>();
			var mockConditionsService = new Mock<INtiConditionsService>();
			var mockLogger = new Mock<ILogger<AddConditionsPageModel>>();

			var pageModel = SetupAddConditionsPageModel(mockNtiModelService, mockConditionsService, mockLogger);
			
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);

			// act
			var pageResponse = await pageModel.OnPostAsync(String.Empty);

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}

		[Test]
		public void OnGetAsync_MissingContinuationId_ThrowsException()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockConditionsService = new Mock<INtiConditionsService>();
			var mockLogger = new Mock<ILogger<AddConditionsPageModel>>();

			var pageModel = SetupAddConditionsPageModel(mockNtiModelService, mockConditionsService, mockLogger);

			// act, assert
			Assert.ThrowsAsync<InvalidOperationException>(async () => await pageModel.OnGetAsync());
		}

		private static AddConditionsPageModel SetupAddConditionsPageModel(Mock<INtiModelService> mockNtiModelService,
			Mock<INtiConditionsService> mockConditionsCachedService,
			Mock<ILogger<AddConditionsPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			tempData["ContinuationId"] = Guid.NewGuid().ToString();

			return new AddConditionsPageModel(
				mockNtiModelService.Object,
				mockConditionsCachedService.Object,
				mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}