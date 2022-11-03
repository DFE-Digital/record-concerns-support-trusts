using ConcernsCaseWork.Service.NtiWarningLetter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterStatusesCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync();
	}
}
