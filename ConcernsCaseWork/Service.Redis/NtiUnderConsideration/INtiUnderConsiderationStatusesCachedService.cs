using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationStatusesCachedService
	{
		Task<ICollection<NtiStatusDto>> GetAllStatuses();
	}
}
