using ConcernsCaseWork.Service.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public interface INtiConditionsCachedService
	{
		public Task<ICollection<NtiConditionDto>> GetAllConditionsAsync();
	}
}
