using ConcernsCaseWork.Models.Validatable;

namespace ConcernsCaseWork.Models;

public record OptionalDateTimeUiComponent(string ElementRootId, string Name)
{
	public string ElementRootId { get; set; } = ElementRootId;
	public string Name { get; set; } = Name;
	public OptionalDateModel Date { get; set; } = new ();
}