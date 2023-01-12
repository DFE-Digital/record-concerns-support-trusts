using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.Models;

public record TextAreaUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading)
{
	public int MaxLength { get; set; }
	public string Contents { get; set; }
}