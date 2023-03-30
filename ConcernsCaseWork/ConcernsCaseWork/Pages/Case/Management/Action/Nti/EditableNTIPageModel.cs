using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti;

[BindProperties]
public class EditableNTIPageModel : AbstractPageModel
{
	[BindProperty(SupportsGet = true, Name="Urn")] public int CaseUrn { get; set; }
	[BindProperty(SupportsGet = true, Name = "ntiId")] public int NTIId { get; set; }
	public OptionalDateTimeUiComponent DateNTILifted { get; set; } = BuildDateNTILiftedComponent();
	public OptionalDateTimeUiComponent DateNTIClosed { get; set; } = BuildDateNTIClosedComponent();
	public TextBoxUiComponent DecisionID { get; set; } = BuildDecisionIDComponent();
	public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();

	protected void ResetLiftPageComponentsOnValidationError()
	{
		DateNTILifted = BuildDateNTILiftedComponent(DateNTILifted.Date);
		DecisionID = BuildDecisionIDComponent(DecisionID.Text.StringContents);
		Notes = BuildNotesComponent(Notes.Text.StringContents);
	}

	protected void ResetClosePageComponentsOnValidationError()
	{
		DateNTIClosed = BuildDateNTIClosedComponent(DateNTIClosed.Date);
		Notes = BuildNotesComponent(Notes.Text.StringContents);
	}

	private static TextBoxUiComponent BuildDecisionIDComponent(string contents = "")
		=> new("submission-decision-id", nameof(DecisionID), "Submission Decision ID")
		{
			Text = new ValidateableString()
			{
				StringContents = contents,
				DisplayName = "Submission Decision ID"
			}
		};

	private static OptionalDateTimeUiComponent BuildDateNTILiftedComponent([CanBeNull] OptionalDateModel selectedDate = default)
		=> new("date-nti-lifted", nameof(DateNTILifted), "Date NTI lifted")
		{
			Date = selectedDate
		};

	private static OptionalDateTimeUiComponent BuildDateNTIClosedComponent([CanBeNull] OptionalDateModel selectedDate = default)
	=> new("date-nti-closed", nameof(DateNTIClosed), "Date NTI closed")
	{
		Date = selectedDate
	};

	private static TextAreaUiComponent BuildNotesComponent(string contents = "")
		=> new("nti-notes", nameof(Notes), "Finalise notes (optional)")
		{
			Text = new ValidateableString()
			{
				MaxLength = NTIConstants.MaxNotesLength,
				StringContents = contents,
				DisplayName = "Finalise notes (optional)"
			}
		};
}