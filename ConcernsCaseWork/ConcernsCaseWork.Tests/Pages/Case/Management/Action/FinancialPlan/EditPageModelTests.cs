using ConcernsCaseWork.API.Contracts.FinancialPlan;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.FinancialPlan
{
	[Parallelizable(ParallelScope.All)]
	public class EditPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsPageModel()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockLogger = new Mock<ILogger<EditPageModel>>();
			
			var caseUrn = 4;
			var financialPlanId = 6;
                        
			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(SetupFinancialPlanModel(financialPlanId, caseUrn, null));
			
			var pageModel = SetupEditPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;
			

			// act
			var response = await pageModel.OnGetAsync();

			// assert
			Assert.IsInstanceOf<PageResult>(response);		
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}
		
		[Test]
		public async Task WhenOnGetAsync_WhenFinancialPlanIsClosed_RedirectsToClosedPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockLogger = new Mock<ILogger<EditPageModel>>();
			
			var caseUrn = 4;
			var financialPlanId = 6;			
			var financialPlan = SetupFinancialPlanModel(financialPlanId, caseUrn, null);
			financialPlan.ClosedAt = DateTime.Now;
                        
			mockFinancialPlanModelService.Setup(fp => fp.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(financialPlan);
			
			var pageModel = SetupEditPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

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
		public async Task WhenOnPostAsync_Invalid_DatePlanRequested_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockLogger = new Mock<ILogger<EditPageModel>>();
			
			var caseUrn = 1;
			var financialPlanId = 2;
				
			var pageModel = SetupEditPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;
			pageModel.DatePlanRequested = new OptionalDateTimeUiComponent("", "", "") { Date = new OptionalDateModel()
			{
				Day = "32",
				Month = "01",
				Year = "3023"
			} };

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}

		[Test]
		public async Task WhenOnPostAsync_Partial_DatePlanRequested_ThrowsException_ReturnsPage()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockLogger = new Mock<ILogger<EditPageModel>>();

			var caseUrn = 1;
			var financialPlanId = 2;

			var pageModel = SetupEditPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;
			pageModel.DatePlanRequested = new OptionalDateTimeUiComponent("", "", "")
			{
				Date = new OptionalDateModel()
				{
					Day = "14",
					Month = "01",
				}
			};

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			Assert.That(pageResponse, Is.Not.Null);
			Assert.That(pageModel.TempData, Is.Not.Null);
			Assert.That(pageModel.TempData["Error.Message"], Is.EqualTo(ErrorConstants.ErrorOnPostPage));
		}
		
		[Test]
		public async Task WhenOnPostAsync_WithValidDatePlanRequested_Succeeds()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockLogger = new Mock<ILogger<EditPageModel>>();

			var caseUrn = 1;
			var financialPlanId = 2;
			
			var existingFinancialPlanModel = SetupFinancialPlanModel(financialPlanId, caseUrn);
			
			mockFinancialPlanModelService
				.Setup(m => m.GetFinancialPlansModelById(caseUrn, financialPlanId))
				.ReturnsAsync(existingFinancialPlanModel);
			
			var pageModel = SetupEditPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;

			var dateRequested = new DateTime(2022, 04, 02);
			pageModel.DatePlanRequested = new OptionalDateTimeUiComponent("", "", "") { Date = new OptionalDateModel(dateRequested) };


			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			mockFinancialPlanModelService.Verify(f => f.PatchFinancialById(It.Is<PatchFinancialPlanModel>(fpm =>
					fpm.ClosedAt == null && 
					fpm.DatePlanRequested == dateRequested &&
					fpm.DateViablePlanReceived == null)), Times.Once);
			
			Assert.IsNotNull(pageResponse);
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}
			
		[Test]
		public async Task WhenOnPostAsync_ValidRequest_SetsUpdatedAt()
		{
			// arrange
			var mockFinancialPlanModelService = new Mock<IFinancialPlanModelService>();
			var mockLogger = new Mock<ILogger<EditPageModel>>();

			var caseUrn = 1;
			var financialPlanId = 2;
			
			var existingFinancialPlanModel = SetupFinancialPlanModel(financialPlanId, caseUrn);
				

			var pageModel = SetupEditPageModel(mockFinancialPlanModelService.Object, mockLogger.Object);

			pageModel.CaseUrn = caseUrn;
			pageModel.financialPlanId = financialPlanId;

			pageModel.Notes.Text.StringContents = "notes";

			// act
			var pageResponse = await pageModel.OnPostAsync();

			// assert
			mockFinancialPlanModelService.Verify(f => f.PatchFinancialById(It.Is<PatchFinancialPlanModel>(fpm =>
				fpm.UpdatedAt > DateTime.Now.AddMinutes(-1) && fpm.UpdatedAt <= DateTime.Now)), Times.Once);
			
			Assert.IsNotNull(pageResponse);
			Assert.IsNull(pageModel.TempData["Error.Message"]);
		}


		private static EditPageModel SetupEditPageModel(
			IFinancialPlanModelService mockFinancialPlanModelService,
			ILogger<EditPageModel> mockLogger,
			bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);

			var result = new EditPageModel(mockFinancialPlanModelService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};

			return result;
		}

		private static FinancialPlanModel SetupFinancialPlanModel(long planId, long caseUrn, string statusName = "")
			=> new FinancialPlanModel(planId, 
				caseUrn, 
				DateTime.Now, 
				null, 
				null, 
				String.Empty, 
				FinancialPlanStatus.AwaitingPlan,
				null,
				DateTime.Now);
	}
}
