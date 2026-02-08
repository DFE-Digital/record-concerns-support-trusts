using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public record SimpleRadioItem
{
	public SimpleRadioItem(string label, int id)
	{
		Label = label;
		Id = id;
		SubRadioItems = new List<SubRadioItem>();
		SubCheckboxItems = new List<SubCheckboxItem>();
	}

	public List<SubRadioItem> SubRadioItems { get; set; }
	public List<SubCheckboxItem> SubCheckboxItems { get; set; }
	public SubTextAreaItem SubTextAreaItem { get; set; }

	public string Label { get; set; }

	public int? Id { get; set; }

	public bool? Disabled { get; set; }

	/// <summary>
	/// Need an identifier for each radio option
	/// We can bind to the name, but that changes and it means the tests keep breaking
	/// Recommended to use the enum string value
	/// </summary>
	public string? TestId { get; set; }

	public string? HintText { get; set; }
	public string? HintLink { get; set; }
	public string? HintLinkTitle { get; set; }

	/// <summary>
	/// Whether our label is HTML
	/// E.g. rag ratings is coloured span elements
	/// </summary>
	public bool? IsHtmlLabel { get; set; }
}