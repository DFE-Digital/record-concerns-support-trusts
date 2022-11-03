using ConcernsCaseWork.Service.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Status
{
	public interface IStatusCachedService
	{
		Task ClearData();
		Task<IList<StatusDto>> GetStatuses();
		Task<StatusDto> GetStatusByName(string name);
	}
}