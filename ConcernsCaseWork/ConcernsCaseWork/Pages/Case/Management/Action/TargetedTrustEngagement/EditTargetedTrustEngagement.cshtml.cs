using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement
{
	public class EditTargetedTrustEngagementModel : AbstractPageModel
	{
		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty(Name = "activity-id")]
		public string ActivityId { get; set; }

		[BindProperty(Name = "budget-forecast-sub-activities")]
		public RadioButtonsUiComponent BudgetForecastSubActivities { get; set; }

		[BindProperty(Name = "executive-pay-engagement-sub-activities")]
		public RadioButtonsUiComponent ExecutivePayEngagementSubActivities { get; set; }

		[BindProperty(Name = "financial-returns-assurance-sub-activities")]
		public RadioButtonsUiComponent FinancialReturnsAssuranceSubActivities { get; set; }

		[BindProperty(Name = "reserves-oversight-assurance-sub-activities")]
		public RadioButtonsUiComponent ReservesOversightAssuranceSubActivities { get; set; }

		public void OnGet()
		{
			BudgetForecastSubActivities = BuildBudgetForecastSubActivities();
			ExecutivePayEngagementSubActivities = BuildExecutivePayEngagementSubActivities();
			FinancialReturnsAssuranceSubActivities = BuildFinancialReturnsAssuranceSubActivities();
			ReservesOversightAssuranceSubActivities = BuildReservesOversightAssuranceSubActivities();
		}

		public void OnPost()
		{

		}

		private static RadioButtonsUiComponent BuildBudgetForecastSubActivities()
		{
			return new RadioButtonsUiComponent(ElementRootId: "budget-forecast-sub-activities", Name: "budget-forecast-sub-activities", "")
			{
				RadioItems = TrustEngagementActivity.BudgetForecast
					.GetSubActivities()
					.Select(a => new SimpleRadioItem(a.Description(), (int)a)).ToArray(),
			};
		}

		private static RadioButtonsUiComponent BuildExecutivePayEngagementSubActivities()
		{
			return new RadioButtonsUiComponent(ElementRootId: "executive-pay-engagement-sub-activities", Name: "executive-pay-engagement-sub-activities", "")
			{
				RadioItems = TrustEngagementActivity.ExecutivePayEngagement
					.GetSubActivities()
					.Select(a => new SimpleRadioItem(a.Description(), (int)a)).ToArray(),
			};
		}

		private static RadioButtonsUiComponent BuildFinancialReturnsAssuranceSubActivities()
		{
			return new RadioButtonsUiComponent(ElementRootId: "financial-returns-assurance-sub-activities", Name: "financial-returns-assurance-sub-activities", "")
			{
				RadioItems = TrustEngagementActivity.FinanceReturnsAssurance
					.GetSubActivities()
					.Select(a => new SimpleRadioItem(a.Description(), (int)a)).ToArray(),
			};
		}

		private static RadioButtonsUiComponent BuildReservesOversightAssuranceSubActivities()
		{
			return new RadioButtonsUiComponent(ElementRootId: "reserves-oversight-assurance-sub-activities", Name: "reserves-oversight-assurance-sub-activities", "")
			{
				RadioItems = TrustEngagementActivity.ReservesOversightAssurance
					.GetSubActivities()
					.Select(a => new SimpleRadioItem(a.Description(), (int)a)).ToArray(),
			};
		}
	}
}
