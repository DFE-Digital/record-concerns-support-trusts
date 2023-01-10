using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public record RadioButtonsUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading)
{
	public IEnumerable<SimpleRadioItem> RadioItems { get; set; } = new List<SimpleRadioItem>();
	public string SelectedId { get; set; }
}

public record SimpleRadioItem(string Label, string Id);