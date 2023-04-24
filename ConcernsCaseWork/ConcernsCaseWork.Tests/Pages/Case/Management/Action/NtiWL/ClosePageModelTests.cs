using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiWL
{
	[Parallelizable(ParallelScope.All)]
	public class ClosePageModelTests
	{
		private Fixture _fixture;

		public ClosePageModelTests()
		{
			_fixture = new Fixture();
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWarningLetterModelService, mockLogger);
						
			var ntiModel = NTIWarningLetterFactory.BuildNTIWarningLetterModel();

			mockNtiWarningLetterModelService.Setup(n => n.GetNtiWarningLetterId(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;
			pageModel.WarningLetterId = 1;

			// act
			var result = await pageModel.OnGetAsync();

			result.Should().BeAssignableTo<PageResult>();
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenIsClosed_RedirectsToIndexPage()
		{
			// arrange
			var caseUrn = 9823;
			var warningLetterId = 4849;

			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWarningLetterModelService, mockLogger);
			
			var ntiModel = NTIWarningLetterFactory.BuildNTIWarningLetterModel(DateTime.Now);

			mockNtiWarningLetterModelService.Setup(n => n.GetNtiWarningLetterId(warningLetterId))
				.ReturnsAsync(ntiModel);

			pageModel.CaseUrn = caseUrn;
			pageModel.WarningLetterId = warningLetterId;

			// act
			var response = await pageModel.OnGetAsync();
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/ntiwarningletter/{warningLetterId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_ReturnsPage()
		{
			// arrange
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService = new Mock<INtiWarningLetterModelService>();
			Mock<ILogger<ClosePageModel>> mockLogger = new Mock<ILogger<ClosePageModel>>();

			var warningLetterId = 1;

			var ntiModel = NTIWarningLetterFactory.BuildNTIWarningLetterModel(DateTime.Now);

			mockNtiWarningLetterModelService.Setup(n => n.GetNtiWarningLetterId(warningLetterId))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupAddPageModel(mockNtiWarningLetterModelService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.WarningLetterId = warningLetterId;

			pageModel.NtiWarningLetterStatus = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.NtiWarningLetterStatus.SelectedId = 1;

			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

			// act
			var pageResponse = await pageModel.OnPostAsync();

			var pageRedirectResult = pageResponse as RedirectResult;

			pageRedirectResult.Url.Should().Be("/case/1/management");
		}

		private static ClosePageModel SetupAddPageModel(
			Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService,
			Mock<ILogger<ClosePageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ClosePageModel(mockNtiWarningLetterModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}