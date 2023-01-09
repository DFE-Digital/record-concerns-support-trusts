namespace ConcernsCaseWork.Models;

public record BaseUiComponent(string ElementRootId, string Name)
{
	public string ElementRootId { get; set; } = ElementRootId;
	public string Name { get; set; } = Name;
}