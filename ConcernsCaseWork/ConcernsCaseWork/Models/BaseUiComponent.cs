namespace ConcernsCaseWork.Models;

public record BaseUiComponent
{
	public BaseUiComponent(string elementRootId, string name, string heading)
	{
		ElementRootId = elementRootId;
		Name = name;
		Heading = heading;
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

	/// <summary>
	/// For some reason the order of errors is not always the order the properties are added
	/// Could not find any explanation for why this happens and have not been able to find a solution in the framework
	/// Allows us to force a specific order when validation errors are displayed out of order
	/// </summary>
	public int? SortOrder { get; set; }
}