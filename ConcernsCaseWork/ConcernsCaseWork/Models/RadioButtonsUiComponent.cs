using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public record RadioButtonsUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading)
{
	public IEnumerable<RadioItem> RadioItems { get; set; } = new List<RadioItem>();
}