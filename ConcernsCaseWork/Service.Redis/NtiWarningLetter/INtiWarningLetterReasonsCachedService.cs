using ConcernsCasework.Service.NtiWarningLetter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterReasonsCachedService
	{
		public Task ClearData();
		public Task<ICollection<NtiWarningLetterReasonDto>> GetAllReasonsAsync();
	}
}
