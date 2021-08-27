using ConcernsCaseWork.Models;
using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Trust
{
	public sealed class TrustModelService :ITrustModelService
	{
		
		
		public Task<IList<TrustModel>> GetTrustsBySearchCriteria(TrustSearch trustSearch)
		{
			throw new System.NotImplementedException();
		}
	}
}