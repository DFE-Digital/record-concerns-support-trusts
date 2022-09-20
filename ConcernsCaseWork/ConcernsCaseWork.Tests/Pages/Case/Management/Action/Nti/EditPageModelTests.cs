using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Services.Nti;
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
using Service.Redis.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class EditPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_WithNtiId_Calls_API()
		{
			// arrange
			var ntiId = 123L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			pageModel.RouteData.Values["urn"] = 1;
			pageModel.RouteData.Values["NtiId"] = ntiId;

			// act
			await pageModel.OnGetAsync();

			// assert
			mockNtiModelService.Verify(
					m => m.GetNtiByIdAsync(ntiId),
					Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_WhenMovingToConditions_StoresModelToCache()
		{
			// arrange
			var caseUrn = 111L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

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
			mockNtiModelService.Verify(
					m => m.StoreNtiAsync(It.IsAny<NtiModel>(),
					It.Is<string>(continuationId => continuationId.StartsWith(caseUrn.ToString()))),
					Times.Once());
		}

		[Test]
		public async Task WhenOnPostAsync_WhenClickedContinue_ForNewWarningLetter_CreatesModelInDb()
		{
			// arrange
			var caseUrn = 111L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

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
			mockNtiModelService.Verify(
				m => m.CreateNtiAsync(It.Is<NtiModel>(wl => wl.CaseUrn == caseUrn)),
				Times.Once());

		}

		[Test]
		public async Task WhenOnPostAsync_WhenClickedContinue_ForExistingWarningLetter_UpdatesModelInDb()
		{
			// arrange
			var caseUrn = 111L;
			var ntiId = 123L;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));
			
			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiWLStatusesService, mockLogger);

			pageModel.RouteData.Values["urn"] = caseUrn;
			pageModel.RouteData.Values["NtiId"] = ntiId;

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
			mockNtiModelService.Verify(m => m.GetNtiByIdAsync(ntiId), Times.Once);

			mockNtiModelService.Verify(m => m.PatchNtiAsync(
				It.Is<NtiModel>(wl => wl.CaseUrn == caseUrn && wl.Id == ntiId)),
				Times.Once());
		}

		private static AddPageModel SetupAddPageModel(Mock<INtiModelService> mockNtiModelService,
			Mock<INtiReasonsCachedService> mockNtiReasonsCachedService,
			Mock<INtiStatusesCachedService> mockNtiStatusesCachedService,
			Mock<ILogger<AddPageModel>> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new AddPageModel(mockNtiStatusesCachedService.Object, mockNtiReasonsCachedService.Object,
				mockNtiModelService.Object, mockLogger.Object)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}


}