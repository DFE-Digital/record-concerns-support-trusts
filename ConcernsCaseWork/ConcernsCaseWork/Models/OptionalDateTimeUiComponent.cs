using ConcernsCaseWork.Models.Validatable;

namespace ConcernsCaseWork.Models;

public record OptionalDateTimeUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading)
{
	public OptionalDateModel Date { get; set; } = new ();
}