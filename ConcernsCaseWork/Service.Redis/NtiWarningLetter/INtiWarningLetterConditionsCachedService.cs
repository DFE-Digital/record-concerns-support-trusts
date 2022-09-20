using ConcernsCasework.Service.NtiWarningLetter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterConditionsCachedService
	{
		public Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync();
	}
}
