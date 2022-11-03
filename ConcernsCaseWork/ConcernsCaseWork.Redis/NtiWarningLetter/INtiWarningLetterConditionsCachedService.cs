using ConcernsCaseWork.Service.NtiWarningLetter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterConditionsCachedService
	{
		public Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync();
	}
}
