﻿using ConcernsCaseWork.Helpers;
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
		public IEnumerable<RadioItem> FinancialPlanStatuses { get; set; }

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
				GetRouteData();

				FinancialPlanStatuses = await GetStatusesAsync();
				
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
				var requestedDtString = CreateDateString(dateRequestedDay, dateRequestedMonth, dateRequestedYear);
				var viablePlanReceivedDtString = CreateDateString(dateViablePlanDay, dateViablePlanMonth, dateViablePlanYear);

				// Check if dates entered are valid
				var financialPlanDates = ParseFinancialPlanDates(requestedDtString, viablePlanReceivedDtString);

				// Fetch statuses from cache and compare them to the selected status
				var statuses = await _financialPlanStatusCachedService.GetOpenFinancialPlansStatuses();
				var selectedStatus = statuses.FirstOrDefault(s => s.Name.Equals(statusString));
				if (selectedStatus is null)
				{
					throw new InvalidOperationException($"Status {statusString} not found");
				}
				
				var createFinancialPlanModel = new CreateFinancialPlanModel
				{
					CaseUrn = caseUrn,
					CreatedAt = DateTime.Now,
					DatePlanRequested = financialPlanDates.planRequestedDate,
					DateViablePlanReceived = financialPlanDates.viablePlanReceivedDate,
					StatusId = selectedStatus.Id,
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

		private async Task<IEnumerable<RadioItem>> GetStatusesAsync()
			=> (await _financialPlanStatusCachedService.GetOpenFinancialPlansStatuses())
				.Select(s => new RadioItem
				{
					Id = s.Name, 
					Text = s.Description, 
					IsChecked = false
				});

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
			var financialPlanNotes = Request.Form["financial-plan-notes"].ToString();

			var requestedDtString = CreateDateString(dateRequestedDay, dateRequestedMonth, dateRequestedYear);
			var viablePlanReceivedDtString = CreateDateString(dateViablePlanDay, dateViablePlanMonth, dateViablePlanYear);

			if (!string.IsNullOrEmpty(requestedDtString))
			{
				if (!DateTimeHelper.TryParseExact(requestedDtString, out DateTime _))
				{
					throw new InvalidOperationException($"Plan requested {requestedDtString} is an invalid date");
				}
			}

			if (!string.IsNullOrEmpty(viablePlanReceivedDtString))
			{
				if (!DateTimeHelper.TryParseExact(viablePlanReceivedDtString, out DateTime _))
				{
					throw new InvalidOperationException($"Viable Plan {viablePlanReceivedDtString} is an invalid date");
				}
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