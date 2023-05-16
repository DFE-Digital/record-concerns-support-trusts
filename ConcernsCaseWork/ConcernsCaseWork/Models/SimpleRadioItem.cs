namespace ConcernsCaseWork.Models;

public record SimpleRadioItem(string Label, int Id)
{
	/// <summary>
	/// Need an identifier for each radio option
	/// We can bind to the name, but that changes and it means the tests keep breaking
	/// Recommended to use the enum string value
	/// </summary>
	public string? TestId { get; set; }

	public string? HintText { get; set; }
}