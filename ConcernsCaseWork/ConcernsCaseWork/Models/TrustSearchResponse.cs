using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public class TrustSearchResponse
{
	public TrustSearchResponse()
	{
		Data = new List<TrustSearchModel>();
	}

	public IList<TrustSearchModel> Data { get; set; }
	public bool IsMoreDataOnServer { get; set; }
	public string Nonce { get; set; }
	public int TotalMatchesFromApi { get; set; }
}