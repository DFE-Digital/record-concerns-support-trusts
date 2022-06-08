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
using Service.Redis.Models;
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
		public IEnumerable<RadioItem> FinancialPlanStatuses => getStatuses();
		public FinancialPlanEditMode EditMode { get; private set; }

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

			long caseUrn = 0;
			long financialPlanId = 0;
			var editModeEnum = FinancialPlanEditMode.Unknown;

			try
			{
				(caseUrn, financialPlanId, editModeEnum) = GetRouteData();

				FinancialPlanModel = await _financialPlanModelService.GetFinancialPlansModelById(caseUrn, financialPlanId, User.Identity.Name);

				EditMode = editModeEnum;

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
			long caseUrn = 0;
			long financialPlanId = 0;
			var editModeEnum = FinancialPlanEditMode.Unknown;

			try
			{
				(caseUrn, financialPlanId, editModeEnum) = GetRouteData();
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
				var requested_dtString = CreateDateString(dateRequestedDay, dateRequestedMonth, dateRequestedYear);
				var viablePlanReceviced_dtString = CreateDateString(dateViablePlanDay, dateViablePlanMonth, dateViablePlanYear);

				// Check if dates entered are valid
				var financialPlanDates = ParseFinancialPlanDates(requested_dtString, viablePlanReceviced_dtString);

				// Fetch statuses from cache and compare them to the selected status
				var statuses = await _financialPlanStatusCachedService.GetFinancialPlanStatuses();
				var selecetedStatus = statuses.FirstOrDefault(s => s.Name.Equals(statusString));
				var selecetedStatusId = selecetedStatus != null ? selecetedStatus.Id : (long?)null;

				var patchFinancialPlanModel = new PatchFinancialPlanModel()
				{
					Id = financialPlanId,
					CaseUrn = caseUrn,
					StatusId = selecetedStatusId,
					DatePlanRequested = financialPlanDates.planRequestedDate,
					DateViablePlanReceived = financialPlanDates.viablePlanReceivedDate,
					Notes = notes
				};

				if(editModeEnum == FinancialPlanEditMode.Close)
				{
					// todo: closed date is currently set to server date across the system. This should ideally be converted to UTC
					patchFinancialPlanModel.ClosedAt = DateTimeOffset.Now;
				}

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

		private IEnumerable<RadioItem> getStatuses()
		{
			var statuses = (FinancialPlanStatus[])Enum.GetValues(typeof(FinancialPlanStatus));

			return statuses.Where(f => f != FinancialPlanStatus.Unknown)
						   .Select(f => new RadioItem
						   {
							   Id = f.ToString(),
							   Text = EnumHelper.GetEnumDescription(f)
						   });
		}

		private (long, long, FinancialPlanEditMode) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			var financialPlanIdValue = RouteData.Values["finanicialplanid"];
			var editMode = RouteData.Values["editMode"];

			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("CaseUrn is null or invalid to parse");
			}

			if (!long.TryParse(financialPlanIdValue.ToString(), out long financialPlanId) || financialPlanId == 0)
			{
				throw new Exception("FinancialId is 0 or invalid to parse");
			}

			if(!Enum.TryParse<FinancialPlanEditMode>(Convert.ToString(editMode), ignoreCase:true, out var editModeEnum))
			{
				throw new InvalidOperationException("Edit more could not be resolved");
			}

			return (caseUrn, financialPlanId, editModeEnum);
		}

		private void ValidateFinancialPlan()
		{
			var dateRequestedDay = Request.Form["dtr-day-plan-requested"].ToString();
			var dateRequestedMonth = Request.Form["dtr-month-plan-requested"].ToString();
			var dateRequestedYear = Request.Form["dtr-year-plan-requested"].ToString();
			var dateViablePlanDay = Request.Form["dtr-day-viable-plan"].ToString();
			var dateViablePlanMonth = Request.Form["dtr-month-viable-plan"].ToString();
			var dateViablePlanYear = Request.Form["dtr-year-viable-plan"].ToString();
			var financial_plan_notes = Request.Form["financial-plan-notes"].ToString();

			var requested_dtString = CreateDateString(dateRequestedDay, dateRequestedMonth, dateRequestedYear);
			var viablePlanReceviced_dtString = CreateDateString(dateViablePlanDay, dateViablePlanMonth, dateViablePlanYear);

			if (!string.IsNullOrEmpty(requested_dtString))
			{
				if (!DateTimeHelper.TryParseExact(requested_dtString, out DateTime requestedDate))
				{
					throw new InvalidOperationException($"Plan requested {requested_dtString} is an invalid date");
				}
			}

			if (!string.IsNullOrEmpty(viablePlanReceviced_dtString))
			{
				if (!DateTimeHelper.TryParseExact(viablePlanReceviced_dtString, out DateTime viablePlanReceivedDate))
				{
					throw new InvalidOperationException($"Viable Plan {viablePlanReceviced_dtString} is an invalid date");
				}
			}

			if (!string.IsNullOrEmpty(financial_plan_notes))
			{
				var notes = financial_plan_notes.ToString();
				if (notes.Length > NotesMaxLength)
				{
					throw new InvalidOperationException($"Notes provided exceed maximum allowed length ({NotesMaxLength} characters).");
				}
			}
		}

		private string CreateDateString(string day, string month, string year)
		{
			var dtString = (string.IsNullOrEmpty(day) && string.IsNullOrEmpty(month) && string.IsNullOrEmpty(year)) ? String.Empty : $"{day}-{month}-{year}";

			return dtString;
		}

		private (DateTime? planRequestedDate, DateTime? viablePlanReceivedDate) ParseFinancialPlanDates(string planRequestedString, string viablePlanReceivedString)
		{
			DateTime? planRequested_date = null;
			DateTime? viablePlanReceived_date = null;

			if (DateTimeHelper.TryParseExact(planRequestedString, out var requestedDate))
			{
				planRequested_date = requestedDate;
			}

			if (DateTimeHelper.TryParseExact(viablePlanReceivedString, out var receivedDate))
			{
				viablePlanReceived_date = receivedDate;
			}

			return (planRequested_date, viablePlanReceived_date);
		}
	}
}