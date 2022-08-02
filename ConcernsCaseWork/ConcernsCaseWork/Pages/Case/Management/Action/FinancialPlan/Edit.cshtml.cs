using ConcernsCaseWork.Enums;
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

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditPageModel : AbstractPageModel
	{
		private readonly ILogger<EditPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;

		public FinancialPlanModel FinancialPlanModel { get; set; }
		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> FinancialPlanStatuses { get; set; } = new List<RadioItem>();

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

			long caseUrn;
			long financialPlanId;

			try
			{
				(caseUrn, financialPlanId) = GetRouteData();

				FinancialPlanStatuses = await GetStatusesAsync();

				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, User.Identity.Name);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::FinancialPlan::EditPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogInformation("Case::Action::FinancialPlan::EditPageModel::OnPostAsync");
			
			try
			{
				long caseUrn;
				long financialPlanId;
				
				(caseUrn, financialPlanId) = GetRouteData();
				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, User.Identity.Name);

				ValidateFinancialPlan();

				// Store form data
				var statusString = Request.Form["status"].ToString();
				var dateRequestedDay = Request.Form["dtr-day-plan-requested"].ToString();
				var dateRequestedMonth = Request.Form["dtr-month-plan-requested"].ToString();
				var dateRequestedYear = Request.Form["dtr-year-plan-requested"].ToString();
				var dateViablePlanDay = Request.Form["dtr-day-viable-plan"].ToString();
				var dateViablePlanMonth = Request.Form["dtr-month-viable-plan"].ToString();
				var dateViablePlanYear = Request.Form["dtr-year-viable-plan"].ToString();
				var notes = Request.Form["financial-plan-notes"].ToString();
				var currentUser = User.Identity.Name;

				// Check if date, month or year values have been entered
				var requestedDtString = CreateDateString(dateRequestedDay, dateRequestedMonth, dateRequestedYear);
				var viablePlanReceivedDtString = CreateDateString(dateViablePlanDay, dateViablePlanMonth, dateViablePlanYear);

				// Check if dates entered are valid
				var financialPlanDates = ParseFinancialPlanDates(requestedDtString, viablePlanReceivedDtString);

				// Fetch statuses and compare them to the selected status
				var statuses = await _financialPlanStatusCachedService.GetOpenFinancialPlansStatuses();
				var selectedStatus = statuses.FirstOrDefault(s => s.Name.Equals(statusString));
				if (selectedStatus is null)
				{
					throw new InvalidOperationException($"Status {statusString} is invalid");
				}

				var patchFinancialPlanModel = new PatchFinancialPlanModel()
				{
					Id = financialPlanId,
					CaseUrn = caseUrn,
					StatusId = selectedStatus.Id,
					DatePlanRequested = financialPlanDates.planRequestedDate,
					DateViablePlanReceived = financialPlanDates.viablePlanReceivedDate,
					Notes = notes
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
				_logger.LogError("Case::FinancialPlan::EditPageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private async Task<IEnumerable<RadioItem>> GetStatusesAsync()
			=> (await _financialPlanStatusCachedService.GetOpenFinancialPlansStatuses())
				.Select(s => new RadioItem
				{
					Id = s.Name, 
					Text = s.Description
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
			var dateRequestedDay = Request.Form["dtr-day-plan-requested"].ToString();
			var dateRequestedMonth = Request.Form["dtr-month-plan-requested"].ToString();
			var dateRequestedYear = Request.Form["dtr-year-plan-requested"].ToString();
			var dateViablePlanDay = Request.Form["dtr-day-viable-plan"].ToString();
			var dateViablePlanMonth = Request.Form["dtr-month-viable-plan"].ToString();
			var dateViablePlanYear = Request.Form["dtr-year-viable-plan"].ToString();
			var financialPlanNotes = Request.Form["financial-plan-notes"].ToString();

			var requestedDtString = CreateDateString(dateRequestedDay, dateRequestedMonth, dateRequestedYear);
			var viablePlanReceivedDtString = CreateDateString(dateViablePlanDay, dateViablePlanMonth, dateViablePlanYear);

			if (!string.IsNullOrEmpty(requestedDtString) && !DateTimeHelper.TryParseExact(requestedDtString, out DateTime _))
			{
				throw new InvalidOperationException($"Plan requested {requestedDtString} is an invalid date");
			}

			if (!string.IsNullOrEmpty(viablePlanReceivedDtString) && !DateTimeHelper.TryParseExact(viablePlanReceivedDtString, out DateTime _))
			{
				throw new InvalidOperationException($"Viable Plan {viablePlanReceivedDtString} is an invalid date");
			}

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