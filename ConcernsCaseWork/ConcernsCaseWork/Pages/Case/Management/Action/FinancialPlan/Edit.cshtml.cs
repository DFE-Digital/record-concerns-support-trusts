using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.FinancialPlan;
using Service.Redis.FinancialPlan;
using Service.TRAMS.FinancialPlan;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<EditPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;

		public EditPageModel(
			IFinancialPlanModelService financialPlanModelService, IFinancialPlanStatusCachedService financialPlanStatusService, ILogger<EditPageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_financialPlanStatusCachedService = financialPlanStatusService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::EditPageModel::OnGetAsync");

			try
			{
				var caseUrn = GetRequestedCaseUrn();
				var financialPlanId = GetRequestedFinancialPlanId();
				var loggedInUser = GetLoggedInUserName();
				
				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, loggedInUser);

				if (FinancialPlanModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/financialplan/{financialPlanId}/closed");
				}
				
				FinancialPlanStatuses = await GetStatusOptionsAsync(FinancialPlanModel.Status?.Name);
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
				FinancialPlanStatuses = await GetStatusOptionsAsync();
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
				var loggedInUserName = GetLoggedInUserName();
				
				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, loggedInUserName);

				if (FinancialPlanModel is null)
				{
					throw new InvalidOperationException($"FinancialPlanModel with Id {financialPlanId} could not be found");
				}
				
				var planRequestedDate = GetRequestedPlanRequestedDate() ?? FinancialPlanModel.DatePlanRequested;
				var viablePlanReceivedDate = GetRequestedViablePlanReceivedDate() ?? FinancialPlanModel.DateViablePlanReceived;
				var notes = GetRequestedNotes() ?? FinancialPlanModel.Notes;
				var statusName = GetRequestedStatus() ?? FinancialPlanModel.Status?.Name;
				var selectedStatus = await GetOptionalStatusByNameAsync(statusName);

				var patchFinancialPlanModel = new PatchFinancialPlanModel
				{
					Id = financialPlanId,
					CaseUrn = caseUrn,
					StatusId = selectedStatus.Id,
					DatePlanRequested = planRequestedDate,
					DateViablePlanReceived = viablePlanReceivedDate,
					Notes = notes
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
				_logger.LogError("Case::FinancialPlan::EditPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}
		
		protected override async Task<IList<FinancialPlanStatusDto>> GetAvailableStatusesAsync()
			=> await _financialPlanStatusCachedService.GetOpenFinancialPlansStatusesAsync();
		
	}
}