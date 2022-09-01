using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public interface INtiReasonsCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiReasonDto>> GetAllReasonsAsync();
	}
}
