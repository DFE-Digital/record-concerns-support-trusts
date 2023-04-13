using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
using ConcernsCaseWork.Redis.FinancialPlan;
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
	public class ClosePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_MissingCaseUrn_ThrowsException_ReturnPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();

			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			// act
			await pageModel.OnGetAsync();

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnGetPage));
		}

		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			var validStatuses = GetListValidStatuses();
			
			var caseUrn = 4;
			var financialPlanId = 6;
							
			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(SetupFinancialPlanModel(financialPlanId, caseUrn, null));

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;

			
			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.IsInstanceOf<PageResult>(response);
			
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenFinancialPlanIsClosed_ReturnsRedirectToClosedPage()
		{
		// arrange
		var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);
			
			var caseUrn = 4;
			var financialPlanId = 6;
			var now = DateTime.Now;
			var financialPlan = SetupFinancialPlanModel(financialPlanId, caseUrn, null);
			financialPlan.ClosedAt = now;


			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(financialPlan);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;


			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(response, Is.InstanceOf<RedirectResult>());
				Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/financialplan/{financialPlanId}/closed"));
				Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			});
		}

		[Test]
		public async Task WhenOnPostAsync_Valid_Calls_Patch_Method()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 1;
			var financialPlanId = 1;

			var financialPlanModel = SetupFinancialPlanModel(financialPlanId, caseUrn, null);


			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(financialPlanModel);

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;


			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var result = pageResponse as RedirectResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Url, Is.EqualTo($"/case/{caseUrn}/management"));
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			
			mockFinancialPlanModelService.Verify(f => f.PatchFinancialById(It.Is<PatchFinancialPlanModel>(fpm =>
				fpm.ClosedAt != null)), Times.Once);
		}

		[Test]
		public async Task WhenOnPostAsync_Valid_SetsUpdatedAt()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockFinancialPlanStatusService = new Mock<IFinancialPlanStatusCachedService>();
			var mockLogger = new Mock<ILogger<ClosePageModel>>();

			var caseUrn = 1;
			var financialPlanId = 2;

			mockFinancialPlanStatusService.Setup(fp => fp.GetClosureFinancialPlansStatusesAsync())
				.ReturnsAsync(GetListValidStatuses());

			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(SetupFinancialPlanModel(caseUrn, financialPlanId));

			var pageModel = SetupClosePageModel(mockFinancialPlanModelService.Object, mockFinancialPlanStatusService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.InstanceOf<RedirectResult>());
			var result = pageResponse as RedirectResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Url, Is.EqualTo($"/case/{caseUrn}/management"));
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			
			mockFinancialPlanModelService.Verify(f => f.PatchFinancialById(It.Is<PatchFinancialPlanModel>(fpm =>
				fpm.UpdatedAt > DateTime.Now.AddMinutes(-1) && fpm.UpdatedAt <= DateTime.Now)), Times.Once);
		}
		
		private static ClosePageModel SetupClosePageModel(
			IFinancialPlanModelService mockFinancialPlanModelService,
			IFinancialPlanStatusCachedService mockFinancialPlanStatusService,
			ILogger<ClosePageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			return new ClosePageModel(mockFinancialPlanModelService, mockFinancialPlanStatusService, mockLogger)
			{
				PageContext = pageContext, TempData = tempData, Url = new UrlHelper(actionContext), MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
		
		private static List<FinancialPlanStatusDto> GetListValidStatuses() => FinancialPlanStatusFactory.BuildListClosureFinancialPlanStatusDto().ToList();
		
		private static IEnumerable<string> GetListValidStatusNames() => GetListValidStatuses().Select(dto => dto.Name);
		
		private static FinancialPlanModel SetupFinancialPlanModel(long planId, long caseUrn, string statusName = "")
			=> new FinancialPlanModel(planId, 
				caseUrn, 
				DateTime.Now, 
				null, 
				null, 
				String.Empty, 
				new FinancialPlanStatusModel(statusName, 1, false), 
				null,
				DateTime.Now);
	} 
}