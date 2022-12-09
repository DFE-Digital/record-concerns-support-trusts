using System.ComponentModel.DataAnnotations;

public class FindTrustModel
{
	public string Nonce { get; set; }

	[Required]
	public string SelectedTrustUkprn { get; set; }
}