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
	public class ClosePageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<ClosePageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;

		public ClosePageModel(
			IFinancialPlanModelService financialPlanModelService, IFinancialPlanStatusCachedService financialPlanStatusService, ILogger<ClosePageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_financialPlanStatusCachedService = financialPlanStatusService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::ClosePageModel::OnGetAsync");

			try
			{
				var caseUrn = GetRequestedCaseUrn();
				var financialPlanId = GetRequestedFinancialPlanId();
				var loggedInUserName = GetLoggedInUserName();
				
				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, loggedInUserName);
				
				var currentStatusName = FinancialPlanModel.Status?.Name;
				FinancialPlanStatuses = await GetStatusOptionsAsync(currentStatusName);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::ClosePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::ClosePageModel::OnPostAsync");
			
			try
			{
				var caseUrn = GetRequestedCaseUrn();
				var financialPlanId = GetRequestedFinancialPlanId();
				var statusName = GetRequestedStatus();
				var status = await GetRequiredStatusByNameAsync(statusName);
				var notes = GetRequestedNotes();
				var loggedInUserName = GetLoggedInUserName();

				var patchFinancialPlanModel = new PatchFinancialPlanModel
				{
					Id = financialPlanId,
					CaseUrn = caseUrn,
					StatusId = status.Id,
					Notes = notes,
					// todo: closed date is currently set to server date across the system. This should ideally be converted to UTC
					ClosedAt = DateTime.Now
				};

				await _financialPlanModelService.PatchFinancialById(patchFinancialPlanModel, loggedInUserName);
	
				return Redirect($"/case/{caseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
				
				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(GetRequestedCaseUrn(), GetRequestedFinancialPlanId(), GetLoggedInUserName());
				
				var currentStatusName = FinancialPlanModel.Status?.Name;
				FinancialPlanStatuses = await GetStatusOptionsAsync(currentStatusName);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::ClosePageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}
		
		protected override async Task<IList<FinancialPlanStatusDto>> GetAvailableStatusesAsync()
			=> await _financialPlanStatusCachedService.GetClosureFinancialPlansStatusesAsync();
	}
}