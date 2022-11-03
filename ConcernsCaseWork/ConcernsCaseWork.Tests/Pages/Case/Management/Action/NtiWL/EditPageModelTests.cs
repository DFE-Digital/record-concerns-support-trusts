using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiWL
{
	[Parallelizable(ParallelScope.All)]
	public class EditPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_WithWarningLetterId_Calls_API()
		{
			// arrange
			var warningLetterId = 123L;
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockNtiWLStatusesService, mockLogger);

			pageModel.RouteData.Values["urn"] = 1;
			pageModel.RouteData.Values["warningLetterId"] = warningLetterId;

			// act
			await pageModel.OnGetAsync();

			// assert
			mockNtiWLModelService.Verify(
					m => m.GetNtiWarningLetterId(warningLetterId),
					Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_WhenMovingToConditions_StoresModelToCache()
		{
			// arrange
			var caseUrn = 111L;
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

			pageModel.RouteData.Values["urn"] = caseUrn;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
					{ "status", new StringValues("1") },
					{ "dtr-day", new StringValues("2") },
					{ "dtr-month", new StringValues("9") },
					{ "dtr-year", new StringValues("2022") }
			});

			// act
			await pageModel.OnPostAsync(pageModel.ActionForAddConditionsButton);

			// assert
			mockNtiWLModelService.Verify(
					m => m.StoreWarningLetter(It.IsAny<NtiWarningLetterModel>(),
					It.Is<string>(continuationId => continuationId.StartsWith(caseUrn.ToString()))),
					Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_WhenClickedContinue_ForNewWarningLetter_CreatesModelInDb()
		{
			// arrange
			var caseUrn = 111L;
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

			pageModel.RouteData.Values["urn"] = caseUrn;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
					{ "status", new StringValues("1") },
					{ "dtr-day", new StringValues("2") },
					{ "dtr-month", new StringValues("9") },
					{ "dtr-year", new StringValues("2022") }
			});

			// act
			await pageModel.OnPostAsync(pageModel.ActionForContinueButton);

			// assert
			mockNtiWLModelService.Verify(
				m => m.CreateNtiWarningLetter(It.Is<NtiWarningLetterModel>(wl => wl.CaseUrn == caseUrn)),
				Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_WhenClickedContinue_ForExistingWarningLetter_UpdatesModelInDb()
		{
			// arrange
			var caseUrn = 111L;
			var warningLetterId = 123L;
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			mockNtiWLModelService.Setup(ms => ms.GetNtiWarningLetterId(warningLetterId)).Returns(Task.FromResult(new NtiWarningLetterModel { Id = warningLetterId }));

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockNtiWLStatusesService, mockLogger);

			pageModel.RouteData.Values["urn"] = caseUrn;
			pageModel.RouteData.Values["warningLetterId"] = warningLetterId;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
					{ "status", new StringValues("1") },
					{ "dtr-day", new StringValues("2") },
					{ "dtr-month", new StringValues("9") },
					{ "dtr-year", new StringValues("2022") }
			});

			// act
			await pageModel.OnPostAsync(pageModel.ActionForContinueButton);

			// assert
			mockNtiWLModelService.Verify(m => m.GetNtiWarningLetterId(warningLetterId), Times.Once);

			mockNtiWLModelService.Verify(m => m.PatchNtiWarningLetter(
				It.Is<NtiWarningLetterModel>(wl => wl.CaseUrn == caseUrn && wl.Id == warningLetterId)),
				Times.Once());

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