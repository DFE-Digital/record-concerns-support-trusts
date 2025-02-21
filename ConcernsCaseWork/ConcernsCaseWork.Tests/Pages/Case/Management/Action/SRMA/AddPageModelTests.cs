using AutoFixture;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.SRMA;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.MockHelpers;
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
	public class AddPageModelTests
	{		
		private readonly IFixture _fixture;
		
		public AddPageModelTests()
		{
			_fixture = new Fixture();
		}

		[Test]
		public void WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var caseUrn = 1;
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = caseUrn;

			// act
			var result = pageModel.OnGet();

			result.Should().BeAssignableTo<PageResult>();

			// assert
			mockLogger.VerifyLogErrorWasNotCalled();
		}

		[Test]
		public async Task WhenOnPostAsync_FormData_IsValid_SRMA_Is_Created_ReturnsToManagementPage()
		{
			// arrange
			var mockSrmaService = new Mock<ISRMAService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockSrmaService.Object, mockLogger.Object);

			pageModel.CaseUrn = 1;
			pageModel.SRMAStatus = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.SRMAStatus.SelectedId = (int)SRMAStatus.TrustConsidering;
			pageModel.DateOffered = _fixture.Create<OptionalDateTimeUiComponent>();
			pageModel.DateOffered.Date = new OptionalDateModel(new DateTime(2020, 1, 2));
			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var page = pageResponse as RedirectResult;

			Assert.That(pageModel.TempData, Is.Empty);
			Assert.That(page, Is.Not.Null);
			Assert.That(page.Url, Is.EqualTo($"/case/1/management"));
		}

		private static AddPageModel SetupAddPageModel(
			ISRMAService mockSrmaService,
			ILogger<AddPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockSrmaService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}