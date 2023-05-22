namespace ConcernsCaseWork.Models;

public record BaseUiComponent
{
	public BaseUiComponent(string ElementRootId, string Name, string Heading)
	{
		this.ElementRootId = ElementRootId;
		this.Name = Name;
		this.Heading = Heading;
	}

	public string Heading { get;set; }

	public string ElementRootId { get; set; }

	public string Name { get; set; }

	/// <summary>
	/// The display name, this is the name used to identify the field when reporting information to the user
	/// This is different from the heading because the heading might not be suitable, if it contains words or punctuation
	/// </summary>
	public string DisplayName { get; set; }

	public bool? Required { get; set; }

	public string ErrorTextForRequiredField { get; set; }

	public string? HintText { get; set; }
}