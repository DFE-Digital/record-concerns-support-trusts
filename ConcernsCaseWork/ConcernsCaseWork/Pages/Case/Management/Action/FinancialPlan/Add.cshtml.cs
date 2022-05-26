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
using ConcernsCaseWork.Services.FinancialPlan;
using Service.Redis.Models;
using Service.Redis.FinancialPlan;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly IFinancialPlanStatusCachedService _financialPlanStatusCachedService;

		public int NotesMaxLength => 2000;
		public IEnumerable<RadioItem> FinancialPlanStatuses => getStatuses();

		public AddPageModel(
			IFinancialPlanModelService financialPlanModelService, IFinancialPlanStatusCachedService financialPlanStatusService, ILogger<AddPageModel> logger)
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
				GetRouteData();

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
				long caseUrn = GetRouteData();

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

				var createFinancialPlanModel = new CreateFinancialPlanModel
				{
					CaseUrn = caseUrn,
					CreatedAt = DateTime.Now,
					DatePlanRequested = financialPlanDates.planRequestedDate,
					DateViablePlanReceived = financialPlanDates.viablePlanReceivedDate,
					StatusId = selecetedStatusId,
					CreatedBy = currentUser,
					Notes = notes
				};

				await _financialPlanModelService.PostFinancialPlanByCaseUrn(createFinancialPlanModel, currentUser);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (InvalidOperationException ex)
			{
				TempData["FinancialPlan.Message"] = ex.Message;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::AddPageModel::OnPostAsync::Exception - {Message}", ex.Message);

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

		private long GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];

			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("CaseUrn is null or invalid to parse");
			}

			return caseUrn;
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