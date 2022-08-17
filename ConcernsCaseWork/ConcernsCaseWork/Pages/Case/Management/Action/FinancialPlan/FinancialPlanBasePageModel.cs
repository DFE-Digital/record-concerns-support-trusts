using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCasework.Service.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan
{
	public abstract class FinancialPlanBasePageModel : AbstractPageModel
	{
		public readonly int NotesMaxLength = 2000;
		public FinancialPlanModel FinancialPlanModel { get; set; }
		public IEnumerable<RadioItem> FinancialPlanStatuses { get; set; } = new List<RadioItem>();
		
		protected async Task<IEnumerable<RadioItem>> GetStatusOptionsAsync(string selectedStatusName = null)
			=> (await GetAvailableStatusesAsync())
				.Select(s => new RadioItem
				{
					Id = s.Name, 
					Text = s.Description,
					IsChecked = selectedStatusName == s.Name
				});

		protected virtual async Task<FinancialPlanStatusModel> GetOptionalStatusByNameAsync(string statusName)
		{
			var status = (await GetAvailableStatusesAsync())
				.FirstOrDefault(s => s.Name.Equals(statusName));
			
			return status is null ? null : FinancialPlanStatusMapping.MapDtoToModel(status);
		}
		
		protected async Task<FinancialPlanStatusModel> GetRequiredStatusByNameAsync(string statusName)
		{
			var status = await GetOptionalStatusByNameAsync(statusName);
			return status ?? throw new InvalidOperationException($"Please select a reason for closing the Financial Plan");
		}

		protected string GetRequestedStatus() => GetFormValue("status");
		
		protected string GetRequestedNotes() => GetFormValue("financial-plan-notes");
		
		protected string GetLoggedInUserName() => User.Identity.Name.GetValueOrNullIfWhitespace();

		protected long GetRequestedCaseUrn()
		{
			var caseUrnValue = GetRouteValue("urn");

			if (caseUrnValue == null || !long.TryParse(caseUrnValue, out long caseUrn) || caseUrn == 0)
			{
				throw new Exception("CaseUrn is null or invalid to parse");
			}

			return caseUrn;
		}
		
		protected long GetRequestedFinancialPlanId()
		{
			var financialPlanIdValue = GetRouteValue("financialplanid");

			if (!long.TryParse(financialPlanIdValue, out long financialPlanId) || financialPlanId == 0)
			{
				throw new Exception("FinancialId is 0 or invalid to parse");
			}

			return financialPlanId;
		}
		
		protected DateTime? GetRequestedPlanRequestedDate()
		{
			var dateRequestedDay = Request.Form["dtr-day-plan-requested"].ToString();
			var dateRequestedMonth = Request.Form["dtr-month-plan-requested"].ToString();
			var dateRequestedYear = Request.Form["dtr-year-plan-requested"].ToString();
			
			var planRequestedDateStr = CreateDateString(dateRequestedDay, dateRequestedMonth, dateRequestedYear);
			
			if (string.IsNullOrEmpty(planRequestedDateStr)) return null;
			
			if (DateTimeHelper.TryParseExact(planRequestedDateStr, out var requestedDate))
			{
				return requestedDate;
			}
			
			throw new InvalidOperationException($"Plan requested {planRequestedDateStr} is an invalid date");
		}
		
		protected DateTime? GetRequestedViablePlanReceivedDate()
		{
			var dateViablePlanDay = Request.Form["dtr-day-viable-plan"].ToString();
			var dateViablePlanMonth = Request.Form["dtr-month-viable-plan"].ToString();
			var dateViablePlanYear = Request.Form["dtr-year-viable-plan"].ToString();
			
			var viablePlanReceivedDtString = CreateDateString(dateViablePlanDay, dateViablePlanMonth, dateViablePlanYear);

			if (string.IsNullOrEmpty(viablePlanReceivedDtString)) return null;
			
			if (DateTimeHelper.TryParseExact(viablePlanReceivedDtString, out var receivedDate))
			{
				return receivedDate;
			}
			
			throw new InvalidOperationException($"Viable plan {viablePlanReceivedDtString} is an invalid date");
		}
		
		protected string CreateDateString(string day, string month, string year)
			=> (string.IsNullOrEmpty(day) && string.IsNullOrEmpty(month) && string.IsNullOrEmpty(year)) ? String.Empty : $"{day}-{month}-{year}";

		protected abstract Task<IList<FinancialPlanStatusDto>> GetAvailableStatusesAsync();
	}
}