using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;
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
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.FinancialPlan;

[Parallelizable(ParallelScope.All)]
public class IndexPageTests
{
	[Test]
	public async Task WhenOnGetAsync_ReturnsPageModel()
	{
		// arrange
		var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
		var mockLogger = new Mock<ILogger<IndexPageModel>>();
			
		var caseUrn = 4;
		var financialPlanId = 6;
                        
		mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
			.ReturnsAsync(SetupFinancialPlanModel(financialPlanId, caseUrn, null));
	
		var pageModel = SetupIndexPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);
			
		var routeData = pageModel.RouteData.Values;
		routeData.Add("urn", caseUrn);
		routeData.Add("financialplanid", financialPlanId);

		// act
		var response = await pageModel.OnGetAsync();

		// assert
		mockLogger.Verify(
			m => m.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::IndexPageModel::OnGetAsync")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);
			
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<PageResult>());
			Assert.That(pageModel.FinancialPlanModel, Is.Not.Null);
			Assert.That(financialPlanId, Is.EqualTo(pageModel.FinancialPlanModel.Id));
			Assert.That(caseUrn, Is.EqualTo(pageModel.FinancialPlanModel.CaseUrn));
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		});
	}

	[Test]
	public async Task WhenOnGetAsync_WhenFinancialPlanIsClosed_RedirectsToClosedPage()
	{
		// arrange
		var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
		var mockLogger = new Mock<ILogger<IndexPageModel>>();
			
		var caseUrn = 4;
		var financialPlanId = 6;			
		var financialPlan = SetupFinancialPlanModel(financialPlanId, caseUrn, null);
		financialPlan.ClosedAt = DateTime.Now;
                        
		mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
			.ReturnsAsync(financialPlan);
			
		var pageModel = SetupIndexPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);
			
		var routeData = pageModel.RouteData.Values;
		routeData.Add("urn", caseUrn);
		routeData.Add("financialplanid", financialPlanId);

		// act
		var response = await pageModel.OnGetAsync();

		// assert
		mockLogger.Verify(
			m => m.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Case::Action::FinancialPlan::IndexPageModel::OnGetAsync")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);
			
		Assert.Multiple(() =>
		{
			Assert.That(response, Is.InstanceOf<RedirectResult>());
			Assert.That(((RedirectResult)response).Url, Is.EqualTo($"/case/{caseUrn}/management/action/financialplan/{financialPlanId}/closed"));
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
		});
	}
	
	private static FinancialPlanModel SetupFinancialPlanModel(long planId, long caseUrn, string statusName = "")
		=> new FinancialPlanModel(planId, 
			caseUrn, 
			DateTime.Now, 
			null, 
			null, 
			String.Empty, 
			new FinancialPlanStatusModel(statusName, 1, false), 
			null);
	
	private static IndexPageModel SetupIndexPageModel(
		IFinancialPlanModelService mockFinancialPlanModelService,
		ILogger<IndexPageModel> mockLogger,
		bool isAuthenticated = false)
	{
		(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

		return new IndexPageModel(mockFinancialPlanModelService, mockLogger)
		{
			PageContext = pageContext,
			TempData = tempData,
			Url = new UrlHelper(actionContext),
			MetadataProvider = pageContext.ViewData.ModelMetadata
		};
	}
}