using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
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
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

			var routeData = pageModel.RouteData.Values;
			routeData.Add("urn", 4);

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.IsInstanceOf<PageResult>(response);
		}

		[Test]
		public async Task WhenOnPostAsync_WithValidDatePlanRequested_Succeeds()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var caseUrn = 1;
			var dateRequested = new DateTime(2022, 04, 02);

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);
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
			var mockLogger = new Mock<ILogger<AddPageModel>>();

			var statuses = FinancialPlanStatusFactory.BuildListOpenFinancialPlanStatusDto();
			var caseUrn = 1;
			var dateRequested = new DateTime(2022, 03, 03);

			var pageModel = SetupAddPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

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