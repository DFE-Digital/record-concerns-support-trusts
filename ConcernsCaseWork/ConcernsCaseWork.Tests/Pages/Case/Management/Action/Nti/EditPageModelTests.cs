using AutoFixture;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.Nti;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Services.Nti;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Nti
{
	[Parallelizable(ParallelScope.All)]
	public class EditPageModelTests
	{
		private static Fixture _fixture = new();

		[Test]
		public async Task WhenOnGetAsync_WithNtiId_Calls_API()
		{
			// arrange
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			pageModel.CaseUrn = 1;
			pageModel.NtiId = 123;

			// act
			var pageResult = await pageModel.OnGetAsync();

			mockNtiModelService.Verify(
					m => m.GetNtiByIdAsync((long)pageModel.NtiId),
					Times.Once());

			pageResult.Should().BeAssignableTo<PageResult>();
		}
		
		[Test]
		public async Task WhenOnGetAsync_WithClosedNti_RedirectsToClosedPage()
		{
			// arrange
			var ntiId = 123;
			var caseUrn = 1;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);
			var nti = NTIFactory.BuildClosedNTIModel();

			mockNtiModelService.Setup(s => s.GetNtiByIdAsync(ntiId)).ReturnsAsync(nti);

			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			mockNtiModelService.Verify(
				m => m.GetNtiByIdAsync(ntiId),
				Times.Once());
			
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/nti/{ntiId}"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_WhenMovingToConditions_StoresModelToCache()
		{
			// arrange
			var caseUrn = 111;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			pageModel.CaseUrn = caseUrn;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
					{ "status", new StringValues("1") },
			});

			pageModel.DateIssued = _fixture.Create<OptionalDateTimeUiComponent>();
			pageModel.DateIssued.Date = new OptionalDateModel() { Day = "9", Month = "2", Year = "2022" };
			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

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
			var caseUrn = 111;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiStatusesService, mockLogger);

			pageModel.CaseUrn = caseUrn;

			pageModel.DateIssued = _fixture.Create<OptionalDateTimeUiComponent>();
			pageModel.DateIssued.Date = new OptionalDateModel() { Day = "9", Month = "2", Year = "2022" };
			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
					{ "status", new StringValues("1") },
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
			var caseUrn = 111;
			var ntiId = 123;
			var mockNtiModelService = new Mock<INtiModelService>();
			var mockNtiReasonsService = new Mock<INtiReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			mockNtiModelService.Setup(ms => ms.GetNtiByIdAsync(ntiId)).Returns(Task.FromResult(new NtiModel { Id = ntiId }));
			
			var pageModel = SetupAddPageModel(mockNtiModelService, mockNtiReasonsService, mockNtiWLStatusesService, mockLogger);

			pageModel.CaseUrn = caseUrn;
			pageModel.NtiId = ntiId;

			pageModel.HttpContext.Request.Form = new FormCollection(
			new Dictionary<string, StringValues>
			{
					{ "reason", new StringValues("1") },
					{ "status", new StringValues("1") },
			});

			pageModel.DateIssued = _fixture.Create<OptionalDateTimeUiComponent>();
			pageModel.DateIssued.Date = new OptionalDateModel() { Day = "9", Month = "2", Year = "2022" };
			pageModel.Notes = _fixture.Create<TextAreaUiComponent>();

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