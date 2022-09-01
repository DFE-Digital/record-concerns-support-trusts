using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public interface INtiConditionsCachedService
	{
		public Task<ICollection<NtiConditionDto>> GetAllConditionsAsync();
	}
}
