using ConcernsCasework.Service.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public interface INtiConditionsCachedService
	{
		public Task<ICollection<NtiConditionDto>> GetAllConditionsAsync();
	}
}
