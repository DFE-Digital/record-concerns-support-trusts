﻿using AutoFixture;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.SRMA
{
	[Parallelizable(ParallelScope.All)]
	public class ResolvePageModelTests
	{
		private readonly IFixture _fixture;
		
		public ResolvePageModelTests()
		{
			_fixture = new Fixture();
		}
		
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId); 
			routeData.Add("resolution", "complete"); 

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.IsNotNull(pageModel.SRMAModel);

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::SRMA::ResolvePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		[Test]
		public async Task WhenOnGetAsync_AndSrmaIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, closedAt: _fixture.Create<DateTime>());

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId); 
			routeData.Add("resolution", "complete"); 

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/srma/{srmaId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});

			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::SRMA::ResolvePageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_Invalid_SRMA_Resolution_ThrowsException_ReturnPage()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId);
			routeData.Add("resolution", "invalid");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Complete_ReturnsPageModel()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId);
			routeData.Add("resolution", "complete");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.ConfirmText, Is.EqualTo("Confirm SRMA is complete"));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Canceled_ReturnsPageModel()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId);
			routeData.Add("resolution", "cancel");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.ConfirmText, Is.EqualTo("Confirm SRMA was cancelled"));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Declined_ReturnsPageModel()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId);
			routeData.Add("resolution", "decline");

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.ConfirmText, Is.EqualTo("Confirm SRMA was declined by trust"));
		}

		[Test]
		public async Task WhenOnPostAsync_MissingSrmaId_ThrowsException_ReturnPage()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			// act
			await pageModel.OnPostAsync();
			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}

		[Test]
		public async Task WhenOnPostAsync_ReturnsPageModel()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.SetNotes(It.IsAny<long>(), It.IsAny<string>()));
			mockSrmaService.Setup(s => s.SetStatus(It.IsAny<long>(), It.IsAny<SRMAStatus>()));

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);
			
			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId);
			routeData.Add("resolution", "complete");


			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "srma-notes", new StringValues("srma-notes") }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.NotNull(pageResponseInstance);
			Assert.That(pageResponseInstance.Url, Is.EqualTo($"/case/{caseUrn}/management"));
		}

		[Test]
		public async Task WhenOnPostAsync_NotesTooLongThrowsExceptionReturnsPage()
		{
			// arrange
			var caseUrn = _fixture.Create<long>();
			var srmaId = _fixture.Create<long>();
			var notes = "1".PadLeft(2001);
			
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.SetNotes(It.IsAny<long>(), It.IsAny<string>()));
			mockSrmaService.Setup(s => s.SetStatus(It.IsAny<long>(), It.IsAny<SRMAStatus>()));

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);
			
			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", caseUrn);
			routeData.Add("srmaId", srmaId);
			routeData.Add("resolution", "complete");

			pageModel.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "srma-notes", new StringValues(notes) }
				});

			// act
			var pageResponse = await pageModel.OnPostAsync();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));

		}

		private static ResolvePageModel SetupResolvePageModel(
			ISRMAService mockSrmaService,
			ILogger<ResolvePageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ResolvePageModel(mockSrmaService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}