using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public record RadioButtonsUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading)
{
	public IEnumerable<SimpleRadioItem> RadioItems { get; set; } = new List<SimpleRadioItem>();
	public int? SelectedId { get; set; }
}