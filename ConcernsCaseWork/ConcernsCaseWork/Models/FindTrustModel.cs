using System.ComponentModel.DataAnnotations;

public class FindTrustModel
{
	public string Nonce { get; set; }

	[Required(ErrorMessage = "A trust is required")]
	public string SelectedTrustUkprn { get; set; }
}