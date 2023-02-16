using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models;

public class FindTrustModel
{
	public string Nonce { get; set; }

	public string SelectedTrustUkprn { get; set; }

	public string SelectedCompaniesHouseNumber { get; set; }
}