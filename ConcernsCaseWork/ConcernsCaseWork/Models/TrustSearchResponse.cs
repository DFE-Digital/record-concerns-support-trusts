using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public class TrustSearchResponse
{
	public TrustSearchResponse()
	{
		Data = new List<TrustSearchModel>();
	}
	public string Nonce { get; set; }
	public IList<TrustSearchModel> Data { get; set; }
}