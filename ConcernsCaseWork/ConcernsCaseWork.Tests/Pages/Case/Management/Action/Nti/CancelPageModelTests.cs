﻿using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class CancelPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;
			pageModel.NtiId = 1;

			// act
			var pageResult = await pageModel.OnGetAsync();

			pageResult.Should().BeAssignableTo<PageResult>();
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenNtiIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseUrn = 3;
			var ntiId = 9;
			
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var ntiModel = NTIFactory.BuildClosedNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			// act
			var response = await pageModel.OnGetAsync();
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/nti/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_ReturnsPage()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockLogger = new Mock<ILogger<CancelPageModel>>();

			var ntiModel = NTIFactory.BuildNTIModel();
			mockNtiModelService.Setup(n => n.GetNtiByIdAsync(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupCancelPageModel(mockNtiModelService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;
			pageModel.NtiId = 1;

			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(page, Is.Not.Null);
			mockNtiModelService.Verify(n => n.PatchNtiAsync(It.IsAny<NtiModel>()), Times.Once);
		}

		private static CancelPageModel SetupCancelPageModel(
			Mock<INtiModelService> mockNtiModelService,
			Mock<ILogger<CancelPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new CancelPageModel(mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}