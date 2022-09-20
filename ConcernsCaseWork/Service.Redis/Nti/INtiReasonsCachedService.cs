using ConcernsCasework.Service.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public interface INtiReasonsCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiReasonDto>> GetAllReasonsAsync();
	}
}
