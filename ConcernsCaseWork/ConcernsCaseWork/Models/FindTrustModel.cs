using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models;

public class FindTrustModel
{
	public string Nonce { get; set; }

	[Required(ErrorMessage = "A trust is required", AllowEmptyStrings = false)]
	public string SelectedTrustUkprn { get; set; }
}