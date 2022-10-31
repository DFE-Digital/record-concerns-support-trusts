using ConcernsCaseWork.Service.NtiWarningLetter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterReasonsCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiWarningLetterReasonDto>> GetAllReasonsAsync();
	}
}
