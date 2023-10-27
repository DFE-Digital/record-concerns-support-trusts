using AutoFixture;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiWL
{
	[Parallelizable(ParallelScope.All)]
	public class EditPageModelTests
	{
		private Fixture _fixture;

		public EditPageModelTests()
		{
			_fixture = new Fixture();
		}

		[Test]
		public async Task WhenOnGetAsync_WithWarningLetterId_Calls_API()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.WarningLetterId = 1;

			// act
			var result = await pageModel.OnGetAsync();

			result.Should().BeAssignableTo<PageResult>();
		}

		[Test]
		public async Task WhenOnPostAsync_WhenMovingToConditions_StoresModelToCache()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockNtiWLConditionsService = new Mock<INtiWarningLetterConditionsCachedService>();

			mockNtiWLConditionsService.Setup(cs => cs.GetAllConditionsAsync()).ReturnsAsync(
					BuildConditions()
				);

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockLogger, mockNtiWLConditionsService);

			pageModel.CaseUrn = 1;

			SetComponents(pageModel);

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
			});

			// act
			await pageModel.OnPostAsync(pageModel.ActionForAddConditionsButton);

			// assert
			mockNtiWLModelService.Verify(
					m => m.StoreWarningLetter(It.IsAny<NtiWarningLetterModel>(),
					It.Is<string>(continuationId => continuationId.StartsWith(pageModel.CaseUrn.ToString()))),
					Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_WhenClickedContinue_ForNewWarningLetter_CreatesModelInDb()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();
			var mockNtiWLConditionsService = new Mock<INtiWarningLetterConditionsCachedService>();

			mockNtiWLConditionsService.Setup(cs => cs.GetAllConditionsAsync()).ReturnsAsync(
					BuildConditions()
				);

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockLogger, mockNtiWLConditionsService);

			pageModel.CaseUrn = 1;

			SetComponents(pageModel);

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
			});

			// act
			await pageModel.OnPostAsync(pageModel.ActionForContinueButton);

			// assert
			mockNtiWLModelService.Verify(
				m => m.CreateNtiWarningLetter(It.Is<NtiWarningLetterModel>(wl => wl.CaseUrn == pageModel.CaseUrn)),
				Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_WhenClickedContinue_ForExistingWarningLetter_UpdatesModelInDb()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var warningLetterId = 1;

			mockNtiWLModelService.Setup(ms => ms.GetNtiWarningLetterId(warningLetterId)).Returns(Task.FromResult(new NtiWarningLetterModel { Id = warningLetterId }));

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.WarningLetterId = warningLetterId;

			SetComponents(pageModel);

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
			});

			// act
			await pageModel.OnPostAsync(pageModel.ActionForContinueButton);

			// assert
			mockNtiWLModelService.Verify(m => m.GetNtiWarningLetterId((long)pageModel.WarningLetterId), Times.Once);

			mockNtiWLModelService.Verify(m => m.PatchNtiWarningLetter(
				It.Is<NtiWarningLetterModel>(wl => wl.CaseUrn == pageModel.CaseUrn && wl.Id == pageModel.WarningLetterId)),
				Times.Once());
		}

		private static AddPageModel SetupAddPageModel(Mock<INtiWarningLetterModelService> mockNtiWarningLetterModelService,
			Mock<INtiWarningLetterReasonsCachedService> mockNtiWarningLetterReasonsCachedService,
			Mock<ILogger<AddPageModel>> mockLogger,
			Mock<INtiWarningLetterConditionsCachedService> mockNtiWarningLetterConditionsCachedService = null,
			bool isAuthenticated = false)
		{
			mockNtiWarningLetterConditionsCachedService ??= new Mock<INtiWarningLetterConditionsCachedService>();

			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockNtiWarningLetterReasonsCachedService.Object,
				mockNtiWarningLetterModelService.Object, mockNtiWarningLetterConditionsCachedService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}

		private void SetComponents(AddPageModel pageModel)
		{
			pageModel.NtiWarningLetterStatus = _fixture.Create<RadioButtonsUiComponent>();
			pageModel.NtiWarningLetterStatus.SelectedId = 1;

			pageModel.SentDate = _fixture.Create<OptionalDateTimeUiComponent>();
			pageModel.SentDate.Date = new OptionalDateModel() { Day = "2", Month = "9", Year = "2022" };

			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();
		}

		private static NtiWarningLetterConditionDto[] BuildConditions()
		{
			return new NtiWarningLetterConditionDto[]
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
			};
		}
	}

}