namespace ConcernsCaseWork.Models;

public record BaseUiComponent(string ElementRootId, string Name, string Heading)
{
	/// <summary>
	/// The display name, this is the name used to identify the field when reporting information to the user
	/// This is different from the heading because the heading might not be suitable, if it contains words or punctuation
	/// </summary>
	public string DisplayName { get; set; }

	public bool? Required { get; set; }
}