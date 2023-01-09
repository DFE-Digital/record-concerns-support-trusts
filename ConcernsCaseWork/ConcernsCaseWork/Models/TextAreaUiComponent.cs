namespace ConcernsCaseWork.Models;

public record TextAreaUiComponent
{
	public string ElementRootId { get; set; }
	public int MaxLength { get; set; }
	public string Heading { get; set; }
	public string Name { get; set; }
	public string Contents { get; set; }
}