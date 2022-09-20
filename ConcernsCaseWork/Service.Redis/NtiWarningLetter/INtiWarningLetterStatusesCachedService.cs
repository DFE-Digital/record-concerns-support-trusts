using ConcernsCasework.Service.NtiWarningLetter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterStatusesCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync();
	}
}
