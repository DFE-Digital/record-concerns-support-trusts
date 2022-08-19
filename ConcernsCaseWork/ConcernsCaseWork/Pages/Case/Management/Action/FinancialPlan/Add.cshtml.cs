using ConcernsCaseWork.Models.CaseActions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcernsCaseWork.Services.FinancialPlan;
using Service.Redis.Models;
using Service.Redis.FinancialPlan;
using Service.TRAMS.FinancialPlan;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : FinancialPlanBasePageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;

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
				var notes = GetRequestedNotes();
				var statusName = GetRequestedStatus();
				var selectedStatus = await GetOptionalStatusByNameAsync(statusName);
				var currentUser = GetLoggedInUserName();

				var createFinancialPlanModel = new CreateFinancialPlanModel
				{
					CaseUrn = caseUrn,
					CreatedAt = DateTime.Now,
					DatePlanRequested = planRequestedDate,
					DateViablePlanReceived = viablePlanReceivedDate,
					StatusId = selectedStatus?.Id,
					CreatedBy = currentUser,
					Notes = notes
				};

				await _financialPlanModelService.PostFinancialPlanByCaseUrn(createFinancialPlanModel, currentUser);

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