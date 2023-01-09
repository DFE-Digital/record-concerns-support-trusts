using ConcernsCaseWork.Models.Validatable;

namespace ConcernsCaseWork.Models;

public record OptionalDateTimeUiComponent(string ElementRootId, string Name) : BaseUiComponent(ElementRootId, Name)
{
	public OptionalDateModel Date { get; set; } = new ();
}