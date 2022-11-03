using ConcernsCaseWork.Service.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public interface INtiReasonsCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiReasonDto>> GetAllReasonsAsync();
	}
}
