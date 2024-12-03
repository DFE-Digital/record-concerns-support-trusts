using AutoFixture;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
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

			pageModel.CaseId = 1;
			pageModel.SrmaId = 1;
			pageModel.Resolution = "complete";

			// act
			var result = await pageModel.OnGetAsync();

			result.Should().BeAssignableTo<PageResult>();
		}
		
		[Test]
		public async Task WhenOnGetAsync_AndSrmaIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var caseId = 1;
			var srmaId = 1;
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed, closedAt: _fixture.Create<DateTime>());

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);
			pageModel.CaseId = caseId;
			pageModel.SrmaId = srmaId;
			pageModel.Resolution = "complete";

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseId}/management/action/srma/{srmaId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnGetAsync_Invalid_SRMA_Resolution_ThrowsException_ReturnPage()
		{
			// arrange
			var caseId = 1;
			var srmaId = 1;
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			pageModel.CaseId = caseId;
			pageModel.SrmaId = srmaId;
			pageModel.Resolution = "invalid";

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Complete_ReturnsPageModel()
		{
			// arrange
			var caseId = 1;
			var srmaId = 1;
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			pageModel.CaseId = caseId;
			pageModel.SrmaId = srmaId;
			pageModel.Resolution = "complete";

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.CloseTextModel.ConfirmText, Is.EqualTo("Confirm SRMA action is complete"));
			Assert.That(pageModel.ViewData[ViewDataConstants.Title], Is.EqualTo("Complete SRMA"));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Canceled_ReturnsPageModel()
		{
			// arrange
			var caseId = 1;
			var srmaId = 1;
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			pageModel.CaseId = caseId;
			pageModel.SrmaId = srmaId;
			pageModel.Resolution = "cancel";

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.CloseTextModel.ConfirmText, Is.EqualTo("Confirm SRMA action was cancelled"));
			Assert.That(pageModel.ViewData[ViewDataConstants.Title], Is.EqualTo("Cancel SRMA"));
		}

		[Test]
		public async Task WhenOnGetAsync_SRMA_Resolution_Is_Declined_ReturnsPageModel()
		{
			// arrange
			var caseId = 1;
			var srmaId = 1;
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);

			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			pageModel.CaseId = caseId;
			pageModel.SrmaId = srmaId;
			pageModel.Resolution = "decline";

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.CloseTextModel.ConfirmText, Is.EqualTo("Confirm SRMA action was declined by trust"));
			Assert.That(pageModel.ViewData[ViewDataConstants.Title], Is.EqualTo("Decline SRMA"));
		}

		[Test]
		public async Task WhenOnPostAsync_ReturnsPageModel()
		{
			// arrange
			var caseId = 1;
			var srmaId = 1;
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<ResolvePageModel>>();

			var srmaModel = SrmaFactory.BuildSrmaModel(SRMAStatus.Deployed);

			mockSrmaService.Setup(s => s.SetNotes(It.IsAny<long>(), It.IsAny<string>()));
			mockSrmaService.Setup(s => s.SetStatus(It.IsAny<long>(), It.IsAny<SRMAStatus>()));

			mockSrmaService.Setup(s => s.GetSRMAById(It.IsAny<long>()))
				.ReturnsAsync(srmaModel);
			
			var pageModel = SetupResolvePageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseId = caseId;
			pageModel.SrmaId = srmaId;
			pageModel.Resolution = "complete";

			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

			// act
			var pageResponse = await pageModel.OnPostAsync();
			var pageResponseInstance = pageResponse as RedirectResult;

			// assert
			Assert.That(pageResponseInstance, Is.Not.Null);
			Assert.That(pageResponseInstance.Url, Is.EqualTo($"/case/{caseId}/management"));
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