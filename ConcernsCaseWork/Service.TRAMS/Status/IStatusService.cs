using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Type
{
	public interface IStatusService
	{
		Task<IList<StatusDto>> GetStatuses();
	}
}