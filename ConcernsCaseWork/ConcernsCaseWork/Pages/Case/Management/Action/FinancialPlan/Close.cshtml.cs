using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.FinancialPlan;
using Service.Redis.FinancialPlan;
using Service.TRAMS.FinancialPlan;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly ILogger<ClosePageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;
		private readonly Func<Task<IList<FinancialPlanStatusDto>>> _getFinancialPlanStatusesAsync;

		public FinancialPlanModel FinancialPlanModel { get; set; }
		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> FinancialPlanStatuses { get; set; } = new List<RadioItem>();

		public ClosePageModel(
			IFinancialPlanModelService financialPlanModelService, IFinancialPlanStatusCachedService financialPlanStatusService, ILogger<ClosePageModel> logger)
		{
			_financialPlanModelService = financialPlanModelService;
			_financialPlanStatusCachedService = financialPlanStatusService;
			_logger = logger;
			
			_getFinancialPlanStatusesAsync = async () => await _financialPlanStatusCachedService.GetClosureFinancialPlansStatuses();
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::ClosePageModel::OnGetAsync");

			try
			{
				long caseUrn;
				long financialPlanId;
				
				(caseUrn, financialPlanId) = GetRouteData();

				FinancialPlanStatuses = await GetStatusesAsync();

				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, User.Identity.Name);

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
				long caseUrn;
				long financialPlanId;
				
				(caseUrn, financialPlanId) = GetRouteData();
				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, User.Identity.Name);

				ValidateFinancialPlan();

				// Store form data
				var statusString = Request.Form["status"].ToString();
				var notes = Request.Form["financial-plan-notes"].ToString();
				var currentUser = User.Identity.Name;

				// Fetch statuses and compare them to the selected status
				var statuses = await _getFinancialPlanStatusesAsync();
				var selectedStatus = statuses.FirstOrDefault(s => s.Name.Equals(statusString));
				if (selectedStatus is null)
				{
					throw new InvalidOperationException($"Status {statusString} is invalid");
				}

				var patchFinancialPlanModel = new PatchFinancialPlanModel
				{
					Id = financialPlanId,
					CaseUrn = caseUrn,
					StatusId = selectedStatus.Id,
					Notes = notes,
					// todo: closed date is currently set to server date across the system. This should ideally be converted to UTC
					ClosedAt = DateTime.Now
				};

				await _financialPlanModelService.PatchFinancialById(patchFinancialPlanModel, currentUser);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::ClosePageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private async Task<IEnumerable<RadioItem>> GetStatusesAsync()
			=> (await _getFinancialPlanStatusesAsync())
				.Select(s => new RadioItem
				{
					Id = s.Name, 
					Text = s.Description, 
					// todo : set ischecked
					IsChecked = false
				});

		private (long, long) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			var financialPlanIdValue = RouteData.Values["financialplanid"];

			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("CaseUrn is null or invalid to parse");
			}

			if (!long.TryParse(financialPlanIdValue.ToString(), out long financialPlanId) || financialPlanId == 0)
			{
				throw new Exception("FinancialId is 0 or invalid to parse");
			}

			return (caseUrn, financialPlanId);
		}

		private void ValidateFinancialPlan()
		{
			var financialPlanNotes = Request.Form["financial-plan-notes"].ToString();

			if (financialPlanNotes?.Length > NotesMaxLength)
			{
				throw new InvalidOperationException($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
			}
		}

		private string CreateDateString(string day, string month, string year)
		{
			var dtString = (string.IsNullOrEmpty(day) && string.IsNullOrEmpty(month) && string.IsNullOrEmpty(year)) ? String.Empty : $"{day}-{month}-{year}";

			return dtString;
		}

		private (DateTime? planRequestedDate, DateTime? viablePlanReceivedDate) ParseFinancialPlanDates(string planRequestedString, string viablePlanReceivedString)
		{
			DateTime? planRequestedDate = null;
			DateTime? viablePlanReceivedDate = null;

			if (DateTimeHelper.TryParseExact(planRequestedString, out var requestedDate))
			{
				planRequestedDate = requestedDate;
			}

			if (DateTimeHelper.TryParseExact(viablePlanReceivedString, out var receivedDate))
			{
				viablePlanReceivedDate = receivedDate;
			}

			return (planRequestedDate, viablePlanReceivedDate);
		}
	}
}