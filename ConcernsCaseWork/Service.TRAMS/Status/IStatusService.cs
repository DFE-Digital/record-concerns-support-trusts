using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Status
{
	public interface IStatusService
	{
		Task<IList<StatusDto>> GetStatuses();
	}
}