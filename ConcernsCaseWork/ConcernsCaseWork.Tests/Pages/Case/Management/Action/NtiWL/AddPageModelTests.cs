using AutoFixture;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
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
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiWL
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		private Fixture _fixture;

		public AddPageModelTests()
		{
			_fixture = new Fixture();
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockNtiWLStatusesService, mockLogger);

			pageModel.CaseUrn = 1;

			// act
			var result = await pageModel.OnGetAsync();

			result.Should().BeAssignableTo<PageResult>();
		}

		[Test]
		public async Task WhenOnGetAsync_When_NTI_WL_Is_Already_Closed_RedirectsToIndexPage()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var ntiModel = NTIWarningLetterFactory.BuildNTIWarningLetterModel(DateTime.Now);

			mockNtiWLModelService.Setup(n => n.GetNtiWarningLetterId(It.IsAny<long>()))
				.ReturnsAsync(ntiModel);

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockNtiWLStatusesService, mockLogger);

			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;
			pageModel.WarningLetterId = 1;

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/1/management/action/ntiwarningletter/1"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_SetsDefaultCondition()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockNtiWLConditionsService = new Mock<INtiWarningLetterConditionsCachedService>();

			mockNtiWLConditionsService.Setup(cs => cs.GetAllConditionsAsync()).ReturnsAsync(
					new NtiWarningLetterConditionDto[]
					{
						new NtiWarningLetterConditionDto
						{
							Id = (int)NtiWarningLetterCondition.FinancialReturns,
							Name = "Doesn't matter"
						},
						new NtiWarningLetterConditionDto
						{
							Id = (int)NtiWarningLetterCondition.ActionPlan,
							Name = "Doesn't matter"
						}
					}
				);


			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockNtiWLStatusesService, mockLogger, mockNtiWLConditionsService);
			var routeData = pageModel.RouteData.Values;
			pageModel.CaseUrn = 1;

			pageModel.NtiWarningLetterStatus = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.NtiWarningLetterStatus.SelectedId = 1;

			pageModel.SentDate = _fixture.Create<OptionalDateTimeUiComponent>();
			pageModel.SentDate.Date = new OptionalDateModel() { Day = "2", Month = "9", Year = "2022" };

			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
				{ "reason", new StringValues("1") }
			});

			// act
			var pageResponse = await pageModel.OnPostAsync(pageModel.ActionForAddConditionsButton);

			// assert
			mockNtiWLModelService.Verify(ntims => ntims.StoreWarningLetter(
				It.Is<NtiWarningLetterModel>(wl => wl.Conditions.Any(
					c => c.Id == (int)NtiWarningLetterCondition.FinancialReturns)), It.IsAny<string>()
			));
		}

		private static AddPageModel SetupAddPageModel(Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService,
			Mock<INtiWarningLetterReasonsCachedService> mockNtiWarningLetterReasonsCachedService,
			Mock<INtiWarningLetterStatusesCachedService> mockNtiWarningLetterStatusesCachedService,
			Mock<ILogger<AddPageModel>> mockLogger,
			Mock<INtiWarningLetterConditionsCachedService> mockNtiWarningLetterConditionsCachedService = null,
			bool isAuthenticated = false)
		{
			mockNtiWarningLetterConditionsCachedService ??= new Mock<INtiWarningLetterConditionsCachedService>();

			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockNtiWarningLetterStatusesCachedService.Object, mockNtiWarningLetterReasonsCachedService.Object,
				mockNtiWarningLetterModelService.Object, mockNtiWarningLetterConditionsCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}

}