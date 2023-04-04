using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;

[BindProperties]
public class FPlanBasePage : AbstractPageModel
{
	[BindProperty(SupportsGet = true, Name="urn")] public int CaseUrn { get; set; }
	[BindProperty(SupportsGet = true, Name = "financialPlanId")] public int financialPlanId { get; set; }

	public OptionalDateTimeUiComponent DatePlanRequested { get; set; } = BuildDatePlanRequestedComponent();
	public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();

	protected void ResetPageComponentsOnValidationError()
	{
		DatePlanRequested = BuildDatePlanRequestedComponent(DatePlanRequested.Date);
		Notes = BuildNotesComponent(Notes.Text.StringContents);
	}


	private static OptionalDateTimeUiComponent BuildDatePlanRequestedComponent([CanBeNull] OptionalDateModel selectedDate = default)
		=> new("date-plan-requested", nameof(DatePlanRequested), "Date financial plan requested")
		{
			Date = selectedDate
		};

	private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("financial-plan-notes", nameof(Notes), "Notes (optional)", "Case owners can record any information they want that feels relevant to the action")
		{
			Text = new ValidateableString()
			{
				MaxLength = FinancialPlanConstants.MaxNotesLength,
				StringContents = contents,
				DisplayName = "Notes (optional)"
			}
		};
}