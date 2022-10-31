using ConcernsCaseWork.Service.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Types
{
	public interface ITypeCachedService
	{
		Task ClearData();
		Task<IList<TypeDto>> GetTypes();
	}
}