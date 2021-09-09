using Service.TRAMS.Cases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Status
{
	public interface IStatusService
	{
		Task<IList<StatusDto>> GetStatuses();
	}
}