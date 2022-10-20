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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.NtiWL
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange

			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockNtiWLStatusesService, mockLogger);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
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

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 1);

			// act
			await pageModel.OnGetAsync();

			// assert
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::NTI-WL::AddPageModel::OnGetAsync")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}

		[Test]
		public async Task WhenOnGetAsync_When_NTI_WL_Is_Already_Closed_ThrowsException_ReturnPage()
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
			routeData.Add("urn", 1); 
			routeData.Add("warningLetterId", 1);


			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred loading the page, please try again. If the error persists contact the service administrator."));
		}

		[Test]
		public async Task WhenOnPostAsync_MissingRouteData_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockNtiWLModelService = new Mock<INtiWarningLetterModelService>();
			var mockNtiWLReasonsService = new Mock<INtiWarningLetterReasonsCachedService>();
			var mockNtiWLStatusesService = new Mock<INtiWarningLetterStatusesCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockNtiWLModelService, mockNtiWLReasonsService, mockNtiWLStatusesService, mockLogger);

			// act
			var pageResponse = await pageModel.OnPostAsync(String.Empty);

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo("An error occurred posting the form, please try again. If the error persists contact the service administrator."));
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
			routeData.Add("urn", 1);

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