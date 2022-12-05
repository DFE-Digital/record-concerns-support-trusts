using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;

public class CloseDecisionRequest
{
	private const int _maxSupportingNotesLength = 2000;

	[StringLength(_maxSupportingNotesLength, ErrorMessage  = "Notes must be 2000 characters or less", MinimumLength = 0)]
	public string SupportingNotes { get; set; }

	public bool IsValid()
	{
		return (SupportingNotes?.Length ?? 0) <= _maxSupportingNotesLength;
	}
}