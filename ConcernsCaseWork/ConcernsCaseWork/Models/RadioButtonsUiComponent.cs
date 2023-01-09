using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public record RadioButtonsUiComponent(string ElementRootId, string Name) : BaseUiComponent(ElementRootId, Name)
{
	public IEnumerable<RadioItem> RadioItems { get; set; } = new List<RadioItem>();
}