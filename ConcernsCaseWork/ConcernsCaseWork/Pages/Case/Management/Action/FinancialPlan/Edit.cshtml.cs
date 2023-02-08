using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.FinancialPlan;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Service.FinancialPlan;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<EditPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;

		public EditPageModel(
			IFinancialPlanModelService financialPlanModelService, ILogger<EditPageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::EditPageModel::OnGetAsync");

			try
			{
				var caseUrn = GetRequestedCaseUrn();
				var financialPlanId = GetRequestedFinancialPlanId();
				
				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId);

				if (FinancialPlanModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/financialplan/{financialPlanId}/closed");
				}
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::EditPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::EditPageModel::OnPostAsync");
			
			try
			{
				var caseUrn = GetRequestedCaseUrn();
				var financialPlanId = GetRequestedFinancialPlanId();

				var patchFinancialPlanModel = new PatchFinancialPlanModel
				{
					Id = financialPlanId,
					CaseUrn = caseUrn,
					StatusId = FinancialPlanModel.Status?.Id,
					DatePlanRequested = GetRequestedPlanRequestedDate(),
					Notes = FinancialPlanModel.Notes,
					UpdatedAt = DateTime.Now
				};

				await _financialPlanModelService.PatchFinancialById(patchFinancialPlanModel);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
				
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::EditPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}
	}
}