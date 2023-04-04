using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.Services.FinancialPlan;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.FinancialPlan
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			var validStatuses = GetListValidStatuses();
			
			mockFinancialPlanStatusService.Setup(fp => fp.GetOpenFinancialPlansStatusesAsync())
				.ReturnsAsync(validStatuses);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 4);

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.IsInstanceOf<PageResult>(response);
		}

		[Test]
		public async Task WhenOnPostAsync_MissingCaseUrn_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<PageResult>());
			var page = pageResponse as PageResult;

			Assert.That(page, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}

	
		[Test]
		public async Task WhenOnPostAsync_WithValidDatePlanRequested_Succeeds()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var caseUrn = 1;
			var dateRequested = new DateTime(2022, 04, 02);

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			pageModel.CaseUrn = caseUrn;
			pageModel.DatePlanRequested   = new OptionalDateTimeUiComponent("", "", "") { Date = new OptionalDateModel(dateRequested) };

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			mockFinancialPlanModelService.Verify(f => f.PostFinancialPlanByCaseUrn(It.Is<CreateFinancialPlanModel>(fpm =>
					fpm.DatePlanRequested == dateRequested)), Times.Once);
			
			Assert.IsNotNull(pageResponse);
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}
				
		[Test]
		public async Task WhenOnPostAsync_SetsUpdatedAtValue_Succeeds()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var statuses = FinancialPlanStatusFactory.BuildListOpenFinancialPlanStatusDto();
			var caseUrn = 1;
			var dateRequested = new DateTime(2022, 03, 03);

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.DatePlanRequested = new OptionalDateTimeUiComponent("", "", "") { Date = new OptionalDateModel(dateRequested) };

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			mockFinancialPlanModelService.Verify(f => 
				f.PostFinancialPlanByCaseUrn(It.Is<CreateFinancialPlanModel>(fpm => 
					fpm.UpdatedAt > DateTime.Now.AddMinutes(-1) && fpm.UpdatedAt <= DateTime.Now)), Times.Once);
			
			Assert.Multiple(() =>
			{
				Assert.That(pageResponse, Is.Not.Null);
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		private static AddPageModel SetupAddPageModel(
			IFinancialPlanModelService mockFinancialPlanModelService, 
			IFinancialPlanStatusCachedService mockFinancialPlanStatusService,
			ILogger<AddPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			var result = new AddPageModel(mockFinancialPlanModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};

			return result;
		}
		
		private static List<FinancialPlanStatusDto> GetListValidStatuses() => FinancialPlanStatusFactory.BuildListOpenFinancialPlanStatusDto().ToList();
	}

}