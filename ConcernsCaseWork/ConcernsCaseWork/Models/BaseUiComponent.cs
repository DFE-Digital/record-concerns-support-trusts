namespace ConcernsCaseWork.Models;

public record BaseUiComponent(string ElementRootId, string Name, string Heading)
{
	public string ElementRootId { get; set; } = ElementRootId;
	public string Name { get; set; } = Name;
	public string Heading { get; set; } = Heading;
}