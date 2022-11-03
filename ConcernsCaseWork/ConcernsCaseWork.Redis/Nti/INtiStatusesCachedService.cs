using ConcernsCaseWork.Service.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public interface INtiStatusesCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiStatusDto>> GetAllStatusesAsync();
	}
}
