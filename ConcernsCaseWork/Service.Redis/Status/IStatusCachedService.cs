using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Status
{
	public interface IStatusCachedService
	{
		Task ClearData();
		Task<IList<StatusDto>> GetStatuses();
		Task<StatusDto> GetStatusByName(string name);
	}
}