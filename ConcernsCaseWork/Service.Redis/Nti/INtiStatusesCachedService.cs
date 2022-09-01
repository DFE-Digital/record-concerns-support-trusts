using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public interface INtiStatusesCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiStatusDto>> GetAllStatusesAsync();
	}
}
