using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.Pages.Case.Management.Action.FinancialPlan;

[BindProperties]
public class FinancialPlanBasePageModel : AbstractPageModel
{
	[BindProperty(SupportsGet = true, Name="urn")] public int CaseUrn { get; set; }
	[BindProperty(SupportsGet = true, Name = "financialPlanId")] public int financialPlanId { get; set; }

	public OptionalDateTimeUiComponent DatePlanRequested { get; set; } = BuildDatePlanRequestedComponent();
	public OptionalDateTimeUiComponent DateViablePlanReceived { get; set; } = BuildDateViablePlanReceivedComponent();
	public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();

	[BindProperty]
	public RadioButtonsUiComponent FinancialPlanClosureStatus { get; set; } = BuildFinancialPlanClosureStatusComponent();

	protected void ResetPageComponentsOnValidationError()
	{
		FinancialPlanClosureStatus = BuildFinancialPlanClosureStatusComponent(FinancialPlanClosureStatus.SelectedId);
		DatePlanRequested = BuildDatePlanRequestedComponent(DatePlanRequested.Date);
		DateViablePlanReceived = BuildDatePlanRequestedComponent(DateViablePlanReceived.Date);
		Notes = BuildNotesComponent(Notes.Text.StringContents);
	}

	private static RadioButtonsUiComponent BuildFinancialPlanClosureStatusComponent(int? selectedId = null)
	=> new(ElementRootId: "reason-for-closure", Name: nameof(FinancialPlanClosureStatus), "Reason for closure")
	{
		RadioItems = new SimpleRadioItem[]
			{
					new (API.Contracts.Enums.FinancialPlanClosureStatus.ViablePlanReceived.Description(), (int)API.Contracts.Enums.FinancialPlanClosureStatus.ViablePlanReceived),
					new (API.Contracts.Enums.FinancialPlanClosureStatus.Abandoned.Description(), (int)API.Contracts.Enums.FinancialPlanClosureStatus.Abandoned),
			},
		SelectedId = selectedId,
	};

	private static OptionalDateTimeUiComponent BuildDatePlanRequestedComponent([CanBeNull] OptionalDateModel selectedDate = default)
		=> new("date-plan-requested", nameof(DatePlanRequested), "Date financial plan requested")
		{
			Date = selectedDate
		};

	private static OptionalDateTimeUiComponent BuildDateViablePlanReceivedComponent([CanBeNull] OptionalDateModel selectedDate = default)
		=> new("date-viable-plan-received", nameof(DateViablePlanReceived), "Date viable plan received")
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