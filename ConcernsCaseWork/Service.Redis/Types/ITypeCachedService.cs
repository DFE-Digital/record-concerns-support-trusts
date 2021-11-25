using Service.TRAMS.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Types
{
	public interface ITypeCachedService
	{
		Task ClearData();
		Task<IList<TypeDto>> GetTypes();
		Task<TypeDto> GetTypeByNameAndDescription(string name, string description);
	}
}