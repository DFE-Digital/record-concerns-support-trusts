using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiUnderConsideration
{
	public interface INtiUnderConsiderationStatusesService
	{
		Task<ICollection<NtiUnderConsiderationStatusDto>> GetAllStatuses();
	}
}
