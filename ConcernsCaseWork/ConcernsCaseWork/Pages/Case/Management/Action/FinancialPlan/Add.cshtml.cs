using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Redis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;

		[BindProperty]
		public CreateFinancialPlanModel CreateFinancialPlanModel { get; set; }

		public AddPageModel(IFinancialPlanModelService financialPlanModelService, IFinancialPlanStatusCachedService financialPlanStatusService, ILogger<AddPageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_financialPlanStatusCachedService = financialPlanStatusService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::AddPageModel::OnGetAsync");

			try
			{
				CreateFinancialPlanModel = new CreateFinancialPlanModel();

				FinancialPlanStatuses = await GetStatusOptionsAsync();
				
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::AddPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				var caseUrn = GetRequestedCaseUrn();
				var planRequestedDate = GetRequestedPlanRequestedDate();
				var viablePlanReceivedDate = GetRequestedViablePlanReceivedDate();
				var currentUser = GetLoggedInUserName();

				var model = new CreateFinancialPlanModel
				{
					CaseUrn = caseUrn,
					CreatedAt = DateTime.Now,
					DatePlanRequested = planRequestedDate,
					DateViablePlanReceived = viablePlanReceivedDate,
					StatusId = CreateFinancialPlanModel.StatusId,
					CreatedBy = currentUser,
					Notes = CreateFinancialPlanModel.Notes
				};

				await _financialPlanModelService.PostFinancialPlanByCaseUrn(model, currentUser);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
				FinancialPlanStatuses = await GetStatusOptionsAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return Page();
		}

		protected override async Task<IList<FinancialPlanStatusDto>> GetAvailableStatusesAsync()
			=> await _financialPlanStatusCachedService.GetOpenFinancialPlansStatusesAsync();
	}
}