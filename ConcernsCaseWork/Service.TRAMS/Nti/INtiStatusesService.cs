using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public interface INtiStatusesService
	{
		Task<ICollection<NtiStatusDto>> GetAllStatuses();
	}
}
