using ConcernsCaseWork.Extensions;
using System.Collections.Generic;
using System.Text;

namespace ConcernsCaseWork.Models
{
	public class TrustSearchModelPageResponseData
	{
		public int TotalMatchesFromApi {get;set; }
		public bool IsMoreDataOnServer {get;set; }
	}
}